/* Copyright (C) 2021 Kevin Boronka
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

using sar.Tools;
using System;

namespace sar.ApplicationHelper
{
	/// <summary>
	/// Description of Log.
	/// </summary>
	public class Logger
	{
		public event LoggerEventHandler OnLog;

		private ApplicationDetails application;
		private string logFilename;
		private ErrorLogger errorLog;
		private FileLogger debugLog;
		public bool LogToConsole { get; set; }

		public Logger(ApplicationDetails application)
			: this(application, application.AssemblyDetails.Name)
		{

		}

		public Logger(ApplicationDetails application, string name)
			: this(application, name, application.CommonDataDirectory)
		{

		}

		public Logger(ApplicationDetails application, string name, string folder)
		{
			this.LogToConsole = false;
			this.application = application;
			this.logFilename = name;
			this.errorLog = new ErrorLogger(application.CommonDataDirectory, name + ".error.log");
			this.debugLog = new FileLogger(application.CommonDataDirectory, name + ".debug.log", true);
		}

		public void Log(Exception ex)
		{
			try
			{
				Log(ex.GetType().ToString() + ": " + ex.Message);
				this.errorLog.Write(ex);

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
			catch
			{

			}
		}

		public void Log(string message)
		{
			try
			{
				message = message.Replace("\r", @"\r");
				message = message.Replace("\n", @"\n");

				this.debugLog.WriteLine(message);

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
					ConsoleHelper.Write(logFilename, ConsoleColor.DarkYellow);
					ConsoleHelper.Write(" > ", ConsoleColor.Yellow);
					ConsoleHelper.WriteLine(message, ConsoleColor.White);
				}
			}
			catch
			{

			}
		}

		public void FlushLogs()
		{
			try
			{
				this.errorLog.FlushFile();
				this.debugLog.FlushFile();
			}
			catch
			{

			}
		}

		public void LogInfo()
		{
			try
			{
				bool logTimestamps = this.debugLog.LogTime;
				this.debugLog.LogTime = false;
				this.debugLog.WriteLine(this.application.AssemblyDetails.Name + " v" + this.application.AssemblyDetails.Version);
				this.debugLog.WriteLine("Path = " + this.application.ApplicationPath);
				this.debugLog.WriteLine("Environment.UserInteractive = " + Environment.UserInteractive.ToString());
				this.debugLog.WriteLine("Username = " + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
				this.debugLog.WriteLine("Wow64 = " + this.application.IsWow64.ToString());
				this.debugLog.WriteLine(ConsoleHelper.HR);

				this.debugLog.LogTime = logTimestamps;

				FlushLogs();
			}
			catch
			{

			}
		}
	}
}
