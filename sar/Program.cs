/* Copyright (C) 2013 Kevin Boronka
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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using skylib.Tools;


// binary download: https://sar-tool.googlecode.com/svn/trunk/release/sar.exe
// release download: https://sar-tool.googlecode.com/svn/tags/

namespace skylib.sar
{
	public class Program
	{
		public const int EXIT_OK = 0;
		public const int EXIT_ERROR = 1;
		
		public static bool NoWarning = false;
		public static bool Debug = false;
		public static bool IncludeSVN = false;
		public static bool IncludeSubFolders = true;

		public static int Main(string[] args)
		{
			//FIXME: no error handling here
			Progress progressBar = new Progress();
			Thread backgroundThread = new Thread(new ThreadStart(progressBar.Enable));
			
			try
			{
				// load all command modules
				List<BaseCommand> allCommands = new List<BaseCommand>() {
					new Help(),
					new BuildCHM(),
					new BuildNSIS(),
					new BuildSLN(),
					new CodeReIndent(),
					new AssemblyInfoVersion(),
					new Kill(),
					new AppShutdownWait(),
					new LabviewVersion(),
					new VboxManage(),
					new FileBackup(),
					new FileSearchAndReplace(),
					new FileTimestamp(),
					new FileEncode(),
					new FileFind(),
					new FileDestory(),
					new FileBsdHeader(),
					new FileMirror(),
					new FileCopy(),
					new FileLock(),
					new DirectoryTimestamp(),
					new WindowsLogin(),
					new WindowsMapDrive(),
					new WindowsRearm(),
					new WindowsRestart(),
					new NetListAddaptors(),
					new SkyUpdaterUpdate(),
					new SkyUpdaterGenerate(),
					new SkyUpdaterAdd(),
					new Delay()
				};

				// process command line arguments
				bool commandlineActive = false;
				int exitCode = EXIT_OK;
				
				while (!commandlineActive)
				{
					try
					{
						if (args.Length == 0)
						{
							commandlineActive = false;
							args = new string[1];
							
							Help.WriteTitle();
							ConsoleHelper.Write("> ", ConsoleColor.White);
							args = StringHelper.ParseString(ConsoleHelper.ReadLine(), " ");
							args = RemoveGlobalArgs(args);
							
							if (args.Length == 0)
							{
								throw new ArgumentException("too few arguments");
							}
						}
						else
						{
							args = RemoveGlobalArgs(args);
							commandlineActive = true;
						}
						
						string command = args[0].ToLower();
						if (command[0] == '-' || command[0] == '/')
						{
							command = command.Substring(1);
						}
						
						// Execute Command
						if (command != "exit")
						{

							backgroundThread = new Thread(new ThreadStart(progressBar.Enable));
							backgroundThread.Name = "RunningIndicator";
							backgroundThread.IsBackground = true;
							backgroundThread.Start();
							exitCode = CommandHub.Execute(command, args);
							args = new string[0];
							backgroundThread.Abort();
						}
					}
					catch (Exception ex)
					{
						try
						{
							backgroundThread.Abort();
							Progress.UpdateTimer.Enabled = false;

							ConsoleHelper.WriteException(ex);

							if (Program.Debug && commandlineActive)
							{
								Thread.Sleep(2000);
							}
							
							args = new string[0];
						}
						catch
						{
							
						}
						
						exitCode = EXIT_ERROR;
					}
				}
				
				Progress.UpdateTimer.Enabled = false;
				backgroundThread.Abort();

				return exitCode;
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteException(ex);
				Thread.Sleep(2000);

				Progress.UpdateTimer.Enabled = false;
				backgroundThread.Abort();
				
				if (Program.Debug)
				{
					ConsoleHelper.WriteLine("Press anykey to continue", ConsoleColor.Yellow);
					ConsoleHelper.ReadKey();
				}
				
				return EXIT_ERROR;
			}
		}
		
		public static string[] RemoveGlobalArgs(string[] args)
		{
			Program.NoWarning = false;
			Program.Debug = false;
			Program.IncludeSVN = false;
			Program.IncludeSubFolders = true;
			
			#if DEBUG
			Program.Debug = true;
			#endif
			
			List<string> result = new List<string>();
			
			foreach (string arg in args)
			{
				if (arg.Length > 1 && arg.Substring(0, 1) == "/")
				{
					switch (arg.ToLower())
					{
						case "/q":
							Program.NoWarning = true;
							break;
						case "/d":
							Program.Debug = true;
							break;
						case "/svn":
							Program.IncludeSVN = true;
							break;
						case "/nosubfolders":
							Program.IncludeSubFolders = false;
							break;
						default:
							result.Add(arg);
							break;
					}
				}
				else
				{
					result.Add(arg);
				}
			}
			
			ConsoleHelper.ShowDebug = Program.Debug;
			IO.IncludeSubFolders = Program.IncludeSubFolders;
			
			ConsoleHelper.DebugWriteLine("/q (quite)= " + Program.NoWarning.ToString());
			ConsoleHelper.DebugWriteLine("/d (debug) = " + Program.Debug.ToString());
			ConsoleHelper.DebugWriteLine("/svn (include .svn folders) = " + Program.IncludeSVN.ToString());
			ConsoleHelper.DebugWriteLine("/nosubfolders = " + (!Program.IncludeSubFolders).ToString());
			
			return result.ToArray();
		}
	}
}