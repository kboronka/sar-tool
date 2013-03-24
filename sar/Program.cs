/*
 * User: Kevin Boronka (kboronka@gmail.com)
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

		public static int Main(string[] args)
		{
			#if DEBUG
			Program.Debug = true;
			#endif
			
			//FIXME: no error handling here
			Progress progressBar = new Progress();
			Thread backgroundThread = new Thread(new ThreadStart(progressBar.Enable));
			
			try
			{
				// load all command modules
				List<BaseCommand> allCommands = new List<BaseCommand>() {
					new Backup(),
					new BuildCHM(),
					new BuildNSIS(),
					new BuildSLN(),
					new Help(),
					new Kill(),
					new LabviewVersion(),
					new SearchAndReplace(),
					new TimestampFilename(),
					new NetLogin(),
					new VboxManage(),
					new FileFind(),
					new FileDestory(),
					new WindowsRearm()
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
							
							if (args.Length == 0)
							{
								throw new ArgumentException("too few arguments");
							}
						}
						else
						{
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
						args = new string[0];
						backgroundThread.Abort();
						
						ConsoleHelper.WriteException(ex);

						if (Program.Debug && commandlineActive)
						{
							Thread.Sleep(2000);
						}
						
						exitCode = EXIT_ERROR;
					}
				}
				
				return exitCode;
			}
			catch (Exception ex)
			{
				backgroundThread.Abort();
				ConsoleHelper.WriteException(ex);
				
				if (Program.Debug)
				{
					ConsoleHelper.WriteLine("Press anykey to continue", ConsoleColor.Yellow);
					ConsoleHelper.ReadKey();
				}
				
				return EXIT_ERROR;
			}
		}
	}
}