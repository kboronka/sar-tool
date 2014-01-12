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
		public static void LogInfo()
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
		
		public static string LogFilename;
		
		#region logger
		
		private static ErrorLogger errorLog;
		public static ErrorLogger ErrorLog
		{
			get
			{
				if (String.IsNullOrEmpty(Program.LogFilename)) Program.LogFilename = "log";
				if (Program.errorLog == null) Program.errorLog = new ErrorLogger("error." + Program.LogFilename);
				return Program.errorLog;
			}
		}
		
		private static FileLogger debugLog;
		public static FileLogger DebugLog
		{
			get
			{
				if (String.IsNullOrEmpty(Program.LogFilename)) Program.LogFilename = "log";
				if (Program.debugLog == null)Program.debugLog = new FileLogger("debug." + Program.LogFilename, true);
				return Program.debugLog;
			}
		}
		
		public static void Log(Exception ex)
		{
			try
			{
				Program.ErrorLog.Write(ex);
			}
			catch
			{
				
			}
		}
		
		public static void Log(string message)
		{
			try
			{
				Program.DebugLog.WriteLine(message);
			}
			catch
			{

			}
		}
		
		#endregion
	}
}
