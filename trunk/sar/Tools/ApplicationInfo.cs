/* Copyright (C) 2015 Kevin Boronka
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
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace sar.Tools
{
	public class ApplicationInfo
	{
		private static string commonDataDirectory;
		public static string CommonDataDirectory
		{
			get
			{
				if (string.IsNullOrEmpty(ApplicationInfo.commonDataDirectory))
				{
					string root = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
					string product = AssemblyInfo.Product;
					string company = AssemblyInfo.Company;
					
					if (String.IsNullOrEmpty(product))
						throw new ArgumentNullException("Product name not specified in AssemblyInfo");

					if (!String.IsNullOrEmpty(company))
						root += "\\" + company;
					
					root += "\\" + product + "\\";
					
					if (!Directory.Exists(root))
						Directory.CreateDirectory(root);

					ApplicationInfo.commonDataDirectory = root;
				}
				
				return ApplicationInfo.commonDataDirectory;
			}
		}

		private static string localDataDirectory;
		public static string DataDirectory
		{
			get
			{
				if (string.IsNullOrEmpty(ApplicationInfo.localDataDirectory))
				{
					string root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
					string product = AssemblyInfo.Product;
					string company = AssemblyInfo.Company;
					
					if (String.IsNullOrEmpty(product))
						throw new ArgumentNullException("Product name not specified in AssemblyInfo");

					if (!String.IsNullOrEmpty(company))
						root += "\\" + company;
					
					root += "\\" + product + "\\";
					
					if (!Directory.Exists(root))
						Directory.CreateDirectory(root);

					ApplicationInfo.localDataDirectory = root;
				}
				
				return ApplicationInfo.localDataDirectory;
			}
		}

		private static string currentDirectory;
		public static string CurrentDirectory
		{
			get
			{
				if (string.IsNullOrEmpty(ApplicationInfo.currentDirectory))
				{
					ApplicationInfo.currentDirectory = Directory.GetCurrentDirectory();
				}
				return ApplicationInfo.currentDirectory;
			}
		}
		
		private static string applicationPath;
		public static string ApplicationPath
		{
			get
			{
				if (string.IsNullOrEmpty(applicationPath))
				{
					applicationPath = System.Reflection.Assembly.GetEntryAssembly().Location;
				}
				
				return applicationPath;
			}
		}
		
		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWow64Process(
			[In] IntPtr hProcess,
			[Out] out bool wow64Process
		);

		private static bool isWow64;
		private static bool isWow64completed;
		public static bool IsWow64
		{
			get
			{
				if (!isWow64completed)
				{
					if (IntPtr.Size == 8)
					{
						isWow64 = true;
					}
					else
					{
						if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major >= 6)
						{
							using (Process p = Process.GetCurrentProcess())
							{
								bool retVal;
								if (!IsWow64Process(p.Handle, out retVal))
								{
									isWow64 = false;
								}
								else
								{
									isWow64 = retVal;
								}
							}
						}
						else
						{
							isWow64 = false;
						}
					}
					
					ConsoleHelper.DebugWriteLine("Wow64 = " + isWow64.ToString());
					isWow64completed = true;
				}
				
				return isWow64;
			}
		}
		
		private static OperatingSystem os;
		public static bool IsWinXPOrHigher
		{
			get
			{
				if (os == null) os = Environment.OSVersion;
				return (os.Platform == PlatformID.Win32NT) && ((os.Version.Major > 5) || ((os.Version.Major == 5) && (os.Version.Minor >= 1)));
			}
		}
		
		public static bool IsWinVistaOrHigher
		{
			get
			{
				if (os == null) os = Environment.OSVersion;
				return (os.Platform == PlatformID.Win32NT) && (os.Version.Major >= 6);
			}
		}

		private static bool hasAdministrativeRight;
		private static bool hasAdministrativeRightcompleted;
		public static bool HasAdministrativeRight
		{
			get
			{
				if (hasAdministrativeRightcompleted) return hasAdministrativeRight;
				
				try
				{
					WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
					hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
					hasAdministrativeRightcompleted = true;
					
					return hasAdministrativeRight;
				}
				catch
				{
					return false;
				}
			}
		}
		
	}
}