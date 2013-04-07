
using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class WindowsMapDrive : BaseCommand
	{
		public WindowsMapDrive() : base("Windows - Map Drive", new List<string> { "windows.map", "win.map" },
		                         "-windows.map [drive letter] [UNC path]",
		                         new List<string>() { @"-windows.login \\192.168.0.244\temp" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string drive = args[1];
			
			if (drive.Length != 1)
			{
				throw new ArgumentException("invalid drive letter");
			}
			
			// TODO: check drive letter
			
			string serverAddres = args[2];
			Progress.Message = "Logging into " + serverAddres;
			
			string uncPath = serverAddres;
			if (uncPath.Substring(0,2) != @"\\") uncPath = @"\\" + uncPath;
			
			
			int exitcode;
			
			exitcode = ConsoleHelper.Shell("net", @"use " + drive + @": /DELETE");
			exitcode = ConsoleHelper.Shell("net", @"use " + drive + @": " + uncPath);
			
			if (exitcode != 0)
			{
				ConsoleHelper.WriteLine("Login to " + serverAddres + " has failed", ConsoleColor.DarkYellow);
				return Program.EXIT_ERROR;
			}

			ConsoleHelper.WriteLine("Login to " + serverAddres + " was successful", ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}
