/* Copyright (C) 2016 Kevin Boronka
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

using sar.Tools;

namespace sar.Base
{
	public abstract class Program
	{
		#region logger
		
		#region members
		
		public static string LogFilename;
		private static ErrorLogger errorLog;
		private static FileLogger debugLog;
		
		#endregion

		public static ErrorLogger ErrorLog
		{
			get
			{

				if (String.IsNullOrEmpty(LogFilename))
				{
					LogFilename = (System.Environment.UserInteractive) ? "" : "s.";
					LogFilename += "log";
				}

				if (errorLog == null) errorLog = new ErrorLogger("error." + LogFilename);
				return errorLog;
			}
		}
		
		public static FileLogger DebugLog
		{
			get
			{
				if (String.IsNullOrEmpty(LogFilename))
				{
					LogFilename = (System.Environment.UserInteractive) ? "" : "s.";
					LogFilename += "log";
				}
				 
				if (debugLog == null) debugLog = new FileLogger("debug." + LogFilename, true);
				return debugLog;
			}
		}
		
		public static void LogInfo()
		{
			try
			{
				bool logTimestamps = DebugLog.LogTime;
				DebugLog.LogTime = false;
				DebugLog.WriteLine(AssemblyInfo.Name + " v" + AssemblyInfo.Version);
				DebugLog.WriteLine("Path = " + ApplicationInfo.ApplicationPath);
				DebugLog.WriteLine("Environment.UserInteractive = " + Environment.UserInteractive.ToString());
				DebugLog.WriteLine("Username = " + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
				DebugLog.WriteLine(ConsoleHelper.HR);
				
				DebugLog.LogTime = logTimestamps;
			}
			catch
			{
				
			}
		}
		
		public static void FlushLogs()
		{
			try
			{
				errorLog.FlushFile();
				debugLog.FlushFile();
			}
			catch
			{
				
			}
		}

		protected static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				Log((Exception)e.ExceptionObject);
			}
			catch
			{
				
			}
		}
	
		public static void Log(Exception ex)
		{
			try
			{
				Log(ex.GetType().ToString() + ": " + ex.Message);
				ErrorLog.Write(ex);
				FlushLogs();
			}
			catch
			{
				
			}
		}
		
		public static void Log(string message)
		{
			try
			{	
				#if DEBUG
				System.Diagnostics.Debug.WriteLine(message);
				#endif
				DebugLog.WriteLine(message);
			}
			catch
			{

			}
		}
		
		#endregion
	}
}