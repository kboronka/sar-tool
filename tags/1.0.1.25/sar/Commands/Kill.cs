/*
 * User: kboronka
 */

using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace skylib.sar
{
	public class Kill : BaseCommand
	{
		public Kill() : base("Kill Process",
		                     new List<string> { "kill", "k" },
		                     @"-kill <FilePath> <backup_location>",
		                     new List<string> { "-backup backup.zip \"c:\\backups\\\"" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("wrong number of arguments");
			}
			
			string processName = args[1];
			
			Process[] foundProcess = Process.GetProcessesByName(processName);
			if (foundProcess.Length != 0)
			{

				foreach (Process process in foundProcess)
				{
					Console.ForegroundColor = ConsoleColor.DarkYellow;
					Console.WriteLine(process.ProcessName + " killed");
					Console.ResetColor();
					process.Kill();
				}
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(processName + " not running");
				Console.ResetColor();
			}
			
			return Program.EXIT_OK;
		}
	}
}
