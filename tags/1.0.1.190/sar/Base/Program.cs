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
				if (String.IsNullOrEmpty(Program.LogFilename)) Program.LogFilename = "log";
				if (Program.errorLog == null) Program.errorLog = new ErrorLogger("error." + Program.LogFilename);
				return Program.errorLog;
			}
		}
		
		public static FileLogger DebugLog
		{
			get
			{
				if (String.IsNullOrEmpty(Program.LogFilename)) Program.LogFilename = "log";
				if (Program.debugLog == null) Program.debugLog = new FileLogger("debug." + Program.LogFilename, true);
				return Program.debugLog;
			}
		}
		
		public static void LogInfo()
		{
			try
			{
				bool logTimestamps = Program.DebugLog.LogTime;
				Program.DebugLog.LogTime = false;
				Program.DebugLog.WriteLine(AssemblyInfo.Name + " v" + AssemblyInfo.Version);
				Program.DebugLog.WriteLine("Path = " + ApplicationInfo.ApplicationPath);
				Program.DebugLog.WriteLine("Environment.UserInteractive = " + Environment.UserInteractive.ToString());
				Program.DebugLog.WriteLine("Username = " + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
				Program.DebugLog.WriteLine(ConsoleHelper.HR);
				
				Program.DebugLog.LogTime = logTimestamps;
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
				Program.Log((Exception)e.ExceptionObject);
			}
			catch
			{
				
			}
		}

		public static void Log(Exception ex)
		{
			try
			{
				Program.Log(ex.GetType().ToString() + ": " + ex.Message);
				Program.ErrorLog.Write(ex);
				Program.FlushLogs();
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
				Program.DebugLog.WriteLine(message);
				#endif				
			}
			catch
			{

			}
		}
		
		#endregion
	}
}