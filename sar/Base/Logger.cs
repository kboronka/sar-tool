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
	/// <summary>
	/// A static wrapper around the ApplicationHelper.Logger class.
	/// </summary>
	public static class Logger
	{
		private static ApplicationHelper.Logger logger;

		private static ApplicationHelper.Logger LoggerLocal
		{
			get
			{
				if (logger == null)
				{
					var assembly = ApplicationHelper.AssemblyDetails.GetAssembly();
					var assemblyDetail = new ApplicationHelper.AssemblyDetails(assembly);
					var app = new ApplicationHelper.ApplicationDetails(assemblyDetail);
					logger = new ApplicationHelper.Logger(app);
				}
				
				return logger;
			}
		}

		public static event ApplicationHelper.LoggerEventHandler OnLog
		{
			add
			{
				LoggerLocal.OnLog += value;
			}
			remove
			{
				LoggerLocal.OnLog -= value;				
			}
		}
		
		private static string logFilename;
		public static string LogFilename
		{
			get
			{
				
				if (String.IsNullOrEmpty(logFilename))
				{
					logFilename = (System.Environment.UserInteractive) ? "" : "s.";
					logFilename += "log";
				}
				
				return logFilename;
			}
			set
			{
				if (value != logFilename)
				{
					// TODO: close old file, open new file
					logFilename = value;
				}
			}
		}
		
		public static bool LogToConsole
		{
			get
			{
				return LoggerLocal.LogToConsole;
			}
			set
			{
				LoggerLocal.LogToConsole  = value;
			}
		}
		
		public static void Log(Exception ex)
		{
			LoggerLocal.Log(ex);
		}
		
		public static void Log(string message)
		{
			LoggerLocal.Log(message);
		}
		
		public static void FlushLogs()
		{
			LoggerLocal.FlushLogs();
		}
		
		public static void LogInfo()
		{
			LoggerLocal.LogInfo();
		}
	}
}
