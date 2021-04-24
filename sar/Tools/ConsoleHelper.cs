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

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace sar.Tools
{
	public class ConsoleHelper
	{
		#region ConsoleShutdownEvent

		[DllImport("Kernel32")]
		public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

		// A delegate type to be used as the handler routine
		// for SetConsoleCtrlHandler.
		public delegate bool HandlerRoutine(CtrlTypes CtrlType);

		// An enumerated type for the control messages
		// sent to the handler routine.
		public enum CtrlTypes
		{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT,
			CTRL_CLOSE_EVENT,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT
		}

		#endregion

		public const int EXIT_OK = 0;
		public const int EXIT_ERROR = 1;

		private static bool progressVisible = false;
		private static object consoleLock = new object();

		#region static properties

		private static bool showDebug = false;
		public static bool ShowDebug
		{
			get
			{
#if DEBUG
				return true;
#else
				return showDebug;
#endif
			}
			set { showDebug = value; }
		}

		private static bool consoleRunning;
		private static bool consoleRunningChecked;
		public static bool ConsoleRunning
		{
			get
			{
				if (!consoleRunningChecked)
				{
					consoleRunning = Console.OpenStandardInput(1) != Stream.Null;
					consoleRunningChecked = true;
				}

				return consoleRunning;
			}
		}

		#endregion

		public static void Write(string text)
		{
			try
			{
				if (Environment.UserInteractive)
				{
					bool timerenabled = Progress.Enabled;
					Progress.Enabled = false;

					if (ConsoleHelper.progressVisible)
					{
						Console.Write("\r" + new String(' ', 79) + "\r");
						ConsoleHelper.progressVisible = false;
					}

					Console.Write(text);
					Progress.Enabled = timerenabled;
				}
			}
			catch
			{

			}
		}

		public static void WriteProgress(string text, ConsoleColor colour)
		{
			lock (consoleLock)
			{
				ConsoleHelper.Write(text, colour);
				ConsoleHelper.progressVisible = true;
			}
		}

		public static void Write(string text, ConsoleColor foreColour)
		{
			ConsoleHelper.Write(text, foreColour, ConsoleColor.Black);
		}

		public static void Write(string text, ConsoleColor foreColour, ConsoleColor backColour)
		{
			if (Environment.UserInteractive && ConsoleHelper.ConsoleRunning)
			{
				Console.ForegroundColor = foreColour;
				Console.BackgroundColor = backColour;
				// utf8 does not work on WindowsXP
				//Console.OutputEncoding = System.Text.Encoding.UTF8;
				ConsoleHelper.Write(text);
				Console.ResetColor();
			}
		}

		public static void WriteLine()
		{
			ConsoleHelper.WriteLine("");
		}

		public static void WriteLine(string text)
		{
			ConsoleHelper.Write(text + "\n");
		}

		public static void WriteLine(string text, ConsoleColor colour)
		{
			ConsoleHelper.Write(text + "\n", colour);
		}

		public static void WriteException(Exception ex)
		{
			ex = ExceptionHelper.GetInner(ex);

			lock (consoleLock)
			{
				ConsoleHelper.Write("error: ", ConsoleColor.Red);
				ConsoleHelper.WriteLine(ex.Message);

				if (ShowDebug)
				{
					ConsoleHelper.WriteLine(ExceptionHelper.GetStackTrace(ex), ConsoleColor.DarkCyan);
				}
			}
		}

		public static void WritePassFail(bool ok)
		{
			lock (consoleLock)
			{
				ConsoleHelper.Write("[", ConsoleColor.White);

				if (ok)
					ConsoleHelper.Write("OK", ConsoleColor.DarkGreen);
				if (!ok)
					ConsoleHelper.Write("FAIL", ConsoleColor.Red);

				ConsoleHelper.Write("]", ConsoleColor.White);
			}
		}

		public static void ApplicationTitle()
		{
			ConsoleHelper.Write(AssemblyInfo.Product + " v" + AssemblyInfo.Version, ConsoleColor.Yellow);
			ConsoleHelper.WriteLine("  " + AssemblyInfo.Copyright);

			string copyright = " \r\n";
			copyright += "this software is distributed under the BSD 2-clause Simplified License\r\n";
			copyright += "\r\n";
			copyright += "THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\"\r\n";
			copyright += "AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE\r\n";
			copyright += "IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE\r\n";
			copyright += "ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE\r\n";
			copyright += "LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR\r\n";
			copyright += "CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF\r\n";
			copyright += "SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS\r\n";
			copyright += "INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN\r\n";
			copyright += "CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)\r\n";
			copyright += "ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE\r\n";
			copyright += "POSSIBILITY OF SUCH DAMAGE.\r\n";

			ConsoleHelper.WriteLine(copyright, ConsoleColor.DarkGreen);
			ConsoleHelper.DebugWriteLine("warning: this is a debug version\n");
		}

		public static void ApplicationShortTitle()
		{
			ConsoleHelper.Write(AssemblyInfo.Product + " v" + AssemblyInfo.Version, ConsoleColor.Yellow);
			ConsoleHelper.WriteLine("  " + AssemblyInfo.Copyright);

			ConsoleHelper.DebugWriteLine("warning: this is a debug version\n");
		}

		public static void DebugWriteLine(string text)
		{
			if (ConsoleHelper.ShowDebug)
			{
				lock (consoleLock)
				{
					Thread.Sleep(20);
					ConsoleHelper.Write(" ", ConsoleColor.Black, ConsoleColor.Black);
					ConsoleHelper.Write("*", ConsoleColor.White, ConsoleColor.Red);
					ConsoleHelper.Write("*", ConsoleColor.Black, ConsoleColor.Yellow);
					ConsoleHelper.Write("*", ConsoleColor.White, ConsoleColor.Blue);
					ConsoleHelper.WriteLine(" " + text, ConsoleColor.Yellow);
					Thread.Sleep(20);
				}
			}
		}

		public static ConsoleKeyInfo ReadKey(string text)
		{
			ConsoleHelper.Write(text);
			return ConsoleHelper.ReadKey();
		}

		public static ConsoleKeyInfo ReadKey()
		{
			if (Environment.UserInteractive)
			{
				bool timerenabled = Progress.Enabled;
				Progress.Enabled = false;
				ConsoleKeyInfo key = Console.ReadKey(true);
				Progress.Enabled = timerenabled;
				return key;
			}
			else
			{
				return new ConsoleKeyInfo();
			}
		}

		public static string ReadLine()
		{
			if (Environment.UserInteractive)
			{
				bool timerenabled = Progress.Enabled;
				Progress.Enabled = false;
				string line = Console.ReadLine();
				Progress.Enabled = timerenabled;
				return line;
			}
			else
			{
				return null;
			}
		}

		public static bool Confirm()
		{
			return ConsoleHelper.Confirm("Are you sure?");
		}

		public static bool Confirm(string text)
		{
			string input = "";
			while (input != "y" && input != "yes" && input != "n" && input != "no")
			{
				ConsoleHelper.Write(text + " ", ConsoleColor.DarkYellow);
				input = ConsoleHelper.ReadLine().ToLower();
			}

			return (input == "y" || input == "yes");
		}

		public static string Input(string text)
		{
			ConsoleHelper.Write(text + " ", ConsoleColor.DarkYellow);
			return ConsoleHelper.ReadLine();
		}

		public static int TryRun(string filename)
		{
			try
			{
				return Run(filename);
			}
			catch
			{
				return ConsoleHelper.EXIT_ERROR;
			}
		}

		public static int Run(string filename)
		{
			string output;
			return ConsoleHelper.Run(filename, "", out output);
		}

		public static int TryRun(string filename, string arguments)
		{
			try
			{
				return Run(filename, arguments);
			}
			catch
			{
				return ConsoleHelper.EXIT_ERROR;
			}
		}

		public static int Run(string filename, string arguments)
		{
			string output;
			return ConsoleHelper.Run(filename, arguments, out output);
		}

		public static int Run(string filename, string arguments, string workingDirectory)
		{
			string output;
			string error;
			return ConsoleHelper.Run(filename, arguments, workingDirectory, out output, out error);
		}

		public static int TryRun(string filename, string arguments, out string output)
		{
			try
			{
				return Run(filename, arguments, out output);
			}
			catch
			{
				output = "";
				return ConsoleHelper.EXIT_ERROR;
			}
		}

		public static int Run(string filename, string arguments, out string output)
		{
			string error;

			int result = ConsoleHelper.Run(filename, arguments, out output, out error);
			output += "\n" + error;

			return result;
		}

		public static int TryRun(string filename, string arguments, out string output, out string error)
		{
			try
			{
				return Run(filename, arguments, out output, out error);
			}
			catch (Exception ex)
			{
				output = "Excpetion";
				error = ex.Message;
				return ConsoleHelper.EXIT_ERROR;
			}
		}

		public static int Run(string filename, string arguments, out string output, out string error)
		{
			int result = ConsoleHelper.Run(filename, arguments, Directory.GetCurrentDirectory(), out output, out error);
			output += "\n" + error;

			return result;
		}

		public static int Run(string filename, string arguments, string workingDirectory, out string output, out string error)
		{
			arguments = arguments.TrimWhiteSpace();

			if (ConsoleHelper.ShowDebug)
			{
				ConsoleHelper.Write(filename, ConsoleColor.White);
				ConsoleHelper.WriteLine(" " + arguments, ConsoleColor.Gray);
			}

			var shell = new Process();

			shell.StartInfo.WorkingDirectory = workingDirectory;
			shell.StartInfo.FileName = filename;
			shell.StartInfo.Arguments = arguments;
			shell.StartInfo.UseShellExecute = false;
			shell.StartInfo.RedirectStandardOutput = true;
			shell.StartInfo.RedirectStandardError = true;
			shell.StartInfo.CreateNoWindow = true;
			shell.Start();

			output = shell.StandardOutput.ReadToEnd();
			error = shell.StandardError.ReadToEnd();

			shell.WaitForExit();

			if (ConsoleHelper.ShowDebug && !String.IsNullOrEmpty(error))
			{
				ConsoleHelper.Write("error:", ConsoleColor.Red);
				ConsoleHelper.WriteLine(" " + error.TrimWhiteSpace(), ConsoleColor.Gray);
				return ConsoleHelper.EXIT_ERROR;
			}

			return shell.ExitCode;
		}

		public static Process Start(string filename)
		{
			return Start(filename, "");
		}

		public static Process Start(string filename, string arguments)
		{
			arguments = arguments.TrimWhiteSpace();

			ProcessStartInfo info = new ProcessStartInfo();
			info.FileName = filename;
			info.Arguments = arguments;
			return Process.Start(info);
		}

		public static Process StartAs(string filename, string arguments, string username, string password)
		{
			return StartAs(filename, arguments, System.Environment.MachineName, username, password);
		}

		public static Process StartAs(string filename, string arguments, string domain, string username, string password)
		{
			//ServiceHelper.ImpersonateUser(username, domain, password);
			arguments = arguments.TrimWhiteSpace();
			var shell = new Process();
			if (!String.IsNullOrEmpty(domain))
				shell.StartInfo.Domain = domain;
			shell.StartInfo.UserName = username;
			shell.StartInfo.Password = StringHelper.MakeSecureString(password);
			shell.StartInfo.FileName = filename;
			shell.StartInfo.Arguments = arguments;
			shell.StartInfo.UseShellExecute = false;
			shell.StartInfo.RedirectStandardOutput = true;
			shell.StartInfo.RedirectStandardError = true;
			shell.Start();
			return shell;
		}

		public static Process FindProcess(string processName)
		{
			var foundProcess = Process.GetProcessesByName(processName);
			if (foundProcess.Length != 0)
				return foundProcess[0];

			var processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				try
				{
					if (IO.GetFilename(process.MainModule.FileName).Contains(processName))
					{
						return process;
					}
				}
				catch
				{

				}
			}

			return null;
		}

		public static bool IsProcessRunning(string processName)
		{
			return (FindProcess(processName) != null);
		}

		public static void KillProcess(string processName)
		{
			bool found;
			do
			{
				found = false;
				var process = FindProcess(processName);
				if (process != null)
				{
					found = true;
					process.Kill();
				}
			} while (found);
		}

		public static void WaitForProcess_Start(string processName)
		{
			WaitForProcess_Start(processName, -1);
		}

		public static bool WaitForProcess_Start(string processName, int timeout)
		{
			var timer = new Stopwatch();
			timer.Start();

			while (!IsProcessRunning(processName) && (!(timer.ElapsedMilliseconds > timeout) || timeout == -1))
			{
				Thread.Sleep(10);
			};

			return !(timer.ElapsedMilliseconds > timeout);
		}

		public static void WaitForProcess_Shutdown(string processName)
		{
			WaitForProcess_Shutdown(processName, -1);
		}

		public static bool WaitForProcess_Shutdown(string processName, int timeout)
		{
			var timer = new Stopwatch();
			timer.Start();

			while (IsProcessRunning(processName) && (!(timer.ElapsedMilliseconds > timeout) || timeout == -1))
			{
				Thread.Sleep(10);
			};

			return !(timer.ElapsedMilliseconds > timeout);
		}

		public static string HR
		{
			get
			{
				return new String('-', 79);
			}
		}

		public static Color ChangeColorBrightness(Color color, float correctionFactor)
		{
			var red = (float)color.R;
			var green = (float)color.G;
			var blue = (float)color.B;

			if (correctionFactor < 0)
			{
				correctionFactor = 1 + correctionFactor;
				red *= correctionFactor;
				green *= correctionFactor;
				blue *= correctionFactor;
			}
			else
			{
				red = (255 - red) * correctionFactor + red;
				green = (255 - green) * correctionFactor + green;
				blue = (255 - blue) * correctionFactor + blue;
			}

			return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
		}
	}
}

