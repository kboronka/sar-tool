/*
 * User: kboronka
 */

using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class TimestampFilename : BaseCommand
	{
		public TimestampFilename() : base("Timestamp Filename", 
		                                new List<string> { "timestamp", "t" },
		                                @"-timestamp <FilePath> [date/time format]",
		                               new List<string> { "-timestamp backup.zip \"yyyy.MM.dd-HH.mm\"" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 2 ||args.Length > 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string timestampFormat = "yyyy.MM.dd-HH.mm.ss";
			if (args.Length == 3)
			{
				timestampFormat = args[2];
			}
			
			string filepath = args[1];
			
			// original file must exits
			if (!File.Exists(filepath))
			{
				throw new FileNotFoundException("file not found. \"" + filepath + "\"");
			}
			
			string datetimestamp = DateTime.Now.ToString(timestampFormat);
			string fileExtension = filepath.Substring(filepath.LastIndexOf('.') + 1);
			string filepathNew = filepath.Substring(0, filepath.Length - fileExtension.Length - 1) + "." + datetimestamp + "." + fileExtension;
			
			if (File.Exists(filepathNew))
			{
				throw new FileLoadException("file already exists. \"" + filepathNew + "\"");
			}
			
			File.Move(filepath, filepathNew);
			
			return Program.EXIT_OK;
		}		
	}
}
