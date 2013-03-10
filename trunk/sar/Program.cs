/*
 * User: Kevin Boronka (kboronka@gmail.com)
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
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

		public static int Main(string[] args)
		{
			Progress progressBar = new Progress();
			Thread backgroundThread = new Thread(new ThreadStart(progressBar.DoWork));
			
			try
			{
				List<BaseCommand> allCommands = new List<BaseCommand>();

				allCommands.Add(new Backup());
				allCommands.Add(new BuildCHM());
				allCommands.Add(new BuildNSIS());
				allCommands.Add(new BuildSLN());
				allCommands.Add(new Help());
				allCommands.Add(new Kill());
				allCommands.Add(new LabviewVersion());
				allCommands.Add(new SearchAndReplace());
				allCommands.Add(new TimestampFilename());
				allCommands.Add(new NetLogin());
				

				backgroundThread.Name = "RunningIndicator";
				backgroundThread.IsBackground = true;
				backgroundThread.Start();
				
				if (args.Length == 0)
				{
					#if DEBUG
					System.Threading.Thread.Sleep(2000);
					#endif
					throw new ArgumentException("too few arguments");
				}
				
				string command = args[0].ToLower();

				if (command[0] != '-' && command[0] != '/')
				{
					throw new ArgumentException("first argument must begin with prefix \"-\" or \"/\" ");
				}
				
				command = command.Substring(1);			
				int exitCode = CommandHub.Execute(command, args);
				
				
				backgroundThread.Abort();
				#if DEBUG
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Press anykey to continue");
				Console.ReadKey();
				Console.ResetColor();
				#endif
				return exitCode;
			}
			catch (Exception ex)
			{
				backgroundThread.Abort();
				
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Error: " + ex.Message);
				Console.ResetColor();
				#if DEBUG
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Press anykey to continue");
				Console.ReadKey();
				Console.ResetColor();
				#endif
				return EXIT_ERROR;
			}
		}
	}
}