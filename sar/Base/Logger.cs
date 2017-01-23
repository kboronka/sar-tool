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

using sar.Tools;

namespace sar
{
	public static class Logger
	{
		public static event LoggerEventHandler OnLog;
		public static bool LogToConsole { get; set; }
		public static string LogFilename;

		private static ErrorLogger errorLog;
		private static FileLogger debugLog;
		
		private static ErrorLogger ErrorLog
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
		
		private static FileLogger DebugLog
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
		
		public static void Log(Exception ex)
		{
			Log(ex.GetType().ToString() + ": " + ex.Message);
			ErrorLog.Write(ex);
			
			if (OnLog != null)
			{
				try
				{
					OnLog(new LoggerEventArgs(ex));
				}
				catch
				{
					
				}
			}
			
			if (LogToConsole)
			{
				ConsoleHelper.WriteException(ex);
			}
			
			FlushLogs();
		}
		
		public static void Log(string message)
		{
			#if DEBUG
			System.Diagnostics.Debug.WriteLine(message);
			#endif
			DebugLog.WriteLine(message);

			if (OnLog != null)
			{
				try
				{
					OnLog(new LoggerEventArgs(message));
				}
				catch
				{
					
				}
			}
			
			if (LogToConsole)
			{
				ConsoleHelper.WriteLine(message);
			}
		}
		
		public static void FlushLogs()
		{
			errorLog.FlushFile();
			debugLog.FlushFile();
		}
		
		public static void LogInfo()
		{
			bool logTimestamps = DebugLog.LogTime;
			DebugLog.LogTime = false;
			DebugLog.WriteLine(AssemblyInfo.Name + " v" + AssemblyInfo.Version);
			DebugLog.WriteLine("Path = " + ApplicationInfo.ApplicationPath);
			DebugLog.WriteLine("Environment.UserInteractive = " + Environment.UserInteractive.ToString());
			DebugLog.WriteLine("Username = " + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
			DebugLog.WriteLine(ConsoleHelper.HR);
			
			DebugLog.LogTime = logTimestamps;
			
			FlushLogs();
		}
	}
}
