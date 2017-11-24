/* Copyright (C) 2017 Kevin Boronka
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace sar.ApplicationHelper
{
	/// <summary>
	/// Description of ApplicationDetails.
	/// </summary>
	public class ApplicationDetails
	{
		public AssemblyDetails AssemblyDetails { get; private set; }
		public string CommonDataDirectory { get; private set; }
		public string DataDirectory { get; private set; }
		public string CurrentDirectory { get; private set; }
		public string ApplicationPath { get; private set; }
		public string DesktopPath { get; private set; }
		public bool HasAdministrativeRight { get; private set; }
		public bool IsWinVistaOrHigher { get; private set; }
		public bool IsWinXPOrHigher { get; private set; }
		public bool IsWow64 { get; private set; }
		
		public ApplicationDetails(AssemblyDetails assembly)
		{
			this.AssemblyDetails = assembly;
			this.CommonDataDirectory = GetCommonDataDirectory();
			this.CommonDataDirectory = GetLocalDataDirectory();
			this.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
			this.ApplicationPath = this.AssemblyDetails.Assembly.Location;
			this.DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\";
			this.HasAdministrativeRight = GetHasAdministrativeRight();
			this.IsWinVistaOrHigher = GetWinVistaOrHigher();
			this.IsWinXPOrHigher = GetIsWinXPOrHigher();
			this.IsWow64 = GetIsWow64();
		}
		
		private string GetCommonDataDirectory()
		{
			var root = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			
			if (!String.IsNullOrEmpty(this.AssemblyDetails.Company))
			{
				root += @"\" + this.AssemblyDetails.Company;
			}

			return root + @"\" + this.AssemblyDetails.Product + @"\";
		}
		
		private string GetLocalDataDirectory()
		{
			var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			
			if (!String.IsNullOrEmpty(this.AssemblyDetails.Company))
			{
				root += @"\" + this.AssemblyDetails.Company;
			}

			return root + @"\" + this.AssemblyDetails.Product + @"\";
		}
		
		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWow64Process(
			[In] IntPtr hProcess,
			[Out] out bool wow64Process);

		private static bool GetIsWow64()
		{
			if (IntPtr.Size == 8)
			{
				return true;
			}
			else
			{
				if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major >= 6)
				{
					using (Process p = Process.GetCurrentProcess())
					{
						bool returnValue;
						if (!IsWow64Process(p.Handle, out returnValue))
						{
							return false;
						}
						else
						{
							return returnValue;
						}
					}
				}
				else
				{
					return false;
				}
			}
		}
		
		private static bool GetIsWinXPOrHigher()
		{
			var os = Environment.OSVersion;
			return (os.Platform == PlatformID.Win32NT) && ((os.Version.Major > 5) || ((os.Version.Major == 5) && (os.Version.Minor >= 1)));
		}
		
		private static bool GetWinVistaOrHigher()
		{
			var os = Environment.OSVersion;
			return (os.Platform == PlatformID.Win32NT) && (os.Version.Major >= 6);
		}

		private static bool GetHasAdministrativeRight()
		{
			try
			{
				var pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				return pricipal.IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch
			{
				return false;
			}
		}
	}
}
