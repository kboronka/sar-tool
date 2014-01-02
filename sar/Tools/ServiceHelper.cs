/* Copyright (C) 2014 Kevin Boronka
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;

namespace sar.Tools
{
	public class ServiceHelper
	{
		public ServiceHelper()
		{
			
		}
		
		public static void LogEvent(string message)
		{
			DateTime dt = new DateTime();
			dt = System.DateTime.UtcNow;
			message = dt.ToLocalTime() + ": " + message;

			EventLog.WriteEntry(AssemblyInfo.Name, message);
		}
		
		public static void Install(string dotNetVersion, string serviceFilePath)
		{
			Install(dotNetVersion, serviceFilePath, "", "");
		}
		
		public static void Install(string dotNetVersion, string serviceFilePath, string username, string password)
		{
			if (String.IsNullOrEmpty(serviceFilePath)) throw new NullReferenceException("invalid filename");
			if (!File.Exists(serviceFilePath)) throw new FileNotFoundException(serviceFilePath + " not found");
			string serviceFileName = IO.GetFilename(serviceFilePath);
			string serviceName = StringHelper.TrimEnd(serviceFileName, IO.GetFileExtension(serviceFilePath).Length + 1);
			string dotNetFolder = IO.FindDotNetFolder(dotNetVersion);
			string installUtil = IO.FindFile(dotNetFolder, "Installutil.exe");
			ShellResults results;

			if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
			{
				results = Shell.Run(installUtil, "/i /user=" + username + " /password=" + password + " " + IO.GetFilename(serviceFilePath), IO.GetRoot(serviceFilePath));
			}
			else
			{
				results = Shell.Run(installUtil, "/i " + IO.GetFilename(serviceFilePath), IO.GetRoot(serviceFilePath));
			}
			
			if (results.ExitCode != Shell.EXIT_OK) throw new Exception ("Failed to install " + serviceName);
			
		}
		
		public static void Uninstall(string dotNetVersion, string serviceFilePath)
		{
			if (String.IsNullOrEmpty(serviceFilePath)) throw new NullReferenceException("invalid filename");
			if (!File.Exists(serviceFilePath)) throw new FileNotFoundException(serviceFilePath + " not found");
			
			string serviceFileName = IO.GetFilename(serviceFilePath);
			string serviceName = StringHelper.TrimEnd(serviceFileName, IO.GetFileExtension(serviceFilePath).Length + 1);
			
			string dotNetFolder = IO.FindDotNetFolder(dotNetVersion);
			string installUtil = IO.FindFile(dotNetFolder, "Installutil.exe");
			ShellResults results = Shell.Run(installUtil, "/u " + IO.GetFilename(serviceFilePath), IO.GetRoot(serviceFilePath));
			if (results.ExitCode != Shell.EXIT_OK) throw new Exception ("Failed to uninstall " + serviceName);
		}
		
		public static void TryUninstall(string serviceFilePath)
		{
			foreach (string dotNetVersion in IO.GetDotNetVersions())
			{
				try
				{
					Uninstall(dotNetVersion, serviceFilePath);
					return;
				}
				catch
				{
					
				}
			}
		}
		
		public static void TryUninstall(string dotNetVersion, string serviceFilePath)
		{
			try
			{
				Uninstall(dotNetVersion, serviceFilePath);
			}
			catch
			{
				
			}
		}
		
		public static void Start(string serviceFilePath)
		{
			if (String.IsNullOrEmpty(serviceFilePath)) throw new NullReferenceException("invalid filename");
			if (!File.Exists(serviceFilePath)) throw new FileNotFoundException(serviceFilePath + " not found");
			
			string serviceFileName = IO.GetFilename(serviceFilePath);
			string serviceName = StringHelper.TrimEnd(serviceFileName, IO.GetFileExtension(serviceFilePath).Length + 1);
			
			ShellResults results = Shell.Run(IO.System32 + @"net.exe", "START " + serviceName);
			if (results.ExitCode != Shell.EXIT_OK)
			{
				if (!String.IsNullOrEmpty(results.Output)) ConsoleHelper.DebugWriteLine("output: " + results.Output);
				if (!String.IsNullOrEmpty(results.Error)) ConsoleHelper.DebugWriteLine("output: " + results.Error);
				throw new Exception ("Failed to start " + serviceName);
			}
		}
		
		public static void TryStart(string serviceFilePath)
		{
			try
			{
				Start(serviceFilePath);
			}
			catch
			{
				
			}
		}

		public static void Stop(string serviceFilePath)
		{
			if (String.IsNullOrEmpty(serviceFilePath)) throw new NullReferenceException("invalid filename");
			if (!File.Exists(serviceFilePath)) throw new FileNotFoundException(serviceFilePath + " not found");
			
			string serviceFileName = IO.GetFilename(serviceFilePath);
			string serviceName = StringHelper.TrimEnd(serviceFileName, IO.GetFileExtension(serviceFilePath).Length + 1);
			
			ShellResults results = Shell.Run(IO.System32 + @"net.exe", "STOP " + serviceName);
			if (results.ExitCode != Shell.EXIT_OK)
			{
				ConsoleHelper.DebugWriteLine(results.Output);
				throw new Exception ("Failed to stop " + serviceName);
			}
		}
		
		public static void TryStop(string serviceFilePath)
		{
			try
			{
				Stop(serviceFilePath);
			}
			catch
			{
				
			}
		}

		
		// group type enum
		public enum SECURITY_IMPERSONATION_LEVEL : int
		{
			SecurityAnonymous = 0,
			SecurityIdentification = 1,
			SecurityImpersonation = 2,
			SecurityDelegation = 3
		}
		
		// obtains user token
		[DllImport("advapi32.dll", SetLastError=true)]
		public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,
		                                    int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		// closes open handes returned by LogonUser
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public extern static bool CloseHandle(IntPtr handle);

		// creates duplicate token handle
		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		public extern static bool DuplicateToken(IntPtr ExistingTokenHandle,
		                                         int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);
		
		// from: http://www.codeproject.com/Articles/4051/Windows-Impersonation-using-C
		public static WindowsImpersonationContext ImpersonateUser(string sUsername, string sDomain, string sPassword)
		{
			// initialize tokens
			IntPtr pExistingTokenHandle = new IntPtr(0);
			IntPtr pDuplicateTokenHandle = new IntPtr(0);
			pExistingTokenHandle = IntPtr.Zero;
			pDuplicateTokenHandle = IntPtr.Zero;
			
			// if domain name was blank, assume local machine
			if (sDomain == "")
				sDomain = System.Environment.MachineName;

			try
			{
				string sResult = null;

				const int LOGON32_PROVIDER_DEFAULT = 0;

				// create token
				const int LOGON32_LOGON_INTERACTIVE = 2;
				//const int SecurityImpersonation = 2;

				// get handle to token
				bool bImpersonated = LogonUser(sUsername, sDomain, sPassword,
				                               LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref pExistingTokenHandle);

				// did impersonation fail?
				if (false == bImpersonated)
				{
					int nErrorCode = Marshal.GetLastWin32Error();
					sResult = "LogonUser() failed with error code: " + nErrorCode + "\r\n";
				}

				// Get identity before impersonation
				sResult += "Before impersonation: " + WindowsIdentity.GetCurrent().Name + "\r\n";

				bool bRetVal = DuplicateToken(pExistingTokenHandle, (int)SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, ref pDuplicateTokenHandle);

				// did DuplicateToken fail?
				if (false == bRetVal)
				{
					int nErrorCode = Marshal.GetLastWin32Error();
					CloseHandle(pExistingTokenHandle); // close existing handle
					sResult += "DuplicateToken() failed with error code: " + nErrorCode + "\r\n";
					return null;
				}
				else
				{
					// create new identity using new primary token
					WindowsIdentity newId = new WindowsIdentity(pDuplicateTokenHandle);
					WindowsImpersonationContext impersonatedUser = newId.Impersonate();

					// check the identity after impersonation
					sResult += "After impersonation: " + WindowsIdentity.GetCurrent().Name + "\r\n";
					return impersonatedUser;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				// close handle(s)
				if (pExistingTokenHandle != IntPtr.Zero)
					CloseHandle(pExistingTokenHandle);
				if (pDuplicateTokenHandle != IntPtr.Zero)
					CloseHandle(pDuplicateTokenHandle);
			}
		}
	}
}
