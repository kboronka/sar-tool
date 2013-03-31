using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class DirectoryTimestamp : BaseCommand
	{
		public DirectoryTimestamp() : base("Timestamp Directory", 
		                                new List<string> { "dir.timestamp", "d.t" },
		                                @"-dir.timestamp [FilePath] [date/time format]",
		                               new List<string> { "-dir.timestamp backup.zip \"yyyy.MM.dd-HH.mm\"" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 2 || args.Length > 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string timestampFormat = "yyyy.MM.dd-HH.mm.ss";
			if (args.Length == 3)
			{
				timestampFormat = args[2];
			}
			
			string dirpath = args[1];
			
			// original file must exits
			if (!Directory.Exists(dirpath))
			{
				throw new FileNotFoundException("directory not found. \"" + dirpath + "\"");
			}
			
			string datetimestamp = DateTime.Now.ToString(timestampFormat);
			string dirpathNew = dirpath + "." + datetimestamp;
			
			if (Directory.Exists(dirpathNew))
			{
				throw new FileLoadException("directory already exists. \"" + dirpathNew + "\"");
			}
			
			Directory.Move(dirpath, dirpathNew);
			
			ConsoleHelper.WriteLine("directory renamed to " + dirpathNew, ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}
