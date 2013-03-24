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
					process.Kill();
					ConsoleHelper.WriteLine(process.ProcessName + " killed", ConsoleColor.DarkYellow);
				}
			}
			else
			{
				ConsoleHelper.WriteLine(processName + " not running", ConsoleColor.DarkYellow);
			}
			
			return Program.EXIT_OK;
		}
	}
}
