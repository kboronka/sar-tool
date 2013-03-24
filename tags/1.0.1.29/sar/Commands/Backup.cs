/*
 * User: kboronka
 */

using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class Backup : BaseCommand
	{
		public Backup() : base("Backup File",
		                       new List<string> { "backup", "bk" },
		                       @"-backup <FilePath> <backup_location>",
		                       new List<string> { "-backup backup.zip \"c:\\backups\\\"" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			
			string filepattern = args[1];
			string archiveroot = args[2];
			if (archiveroot.Substring(archiveroot.Length - 1) != "\\") archiveroot = archiveroot + '\\';
			
			#if DEBUG
			ConsoleHelper.WriteLine("filepattern: " + filepattern);
			ConsoleHelper.WriteLine("archiveroot: " + archiveroot);
			#endif
			
			
			List<string> filelist = IO.GetAllFiles(filepattern);
			
			if (!Directory.Exists(archiveroot))
			{
				throw new DirectoryNotFoundException("unable to find storage folder " + archiveroot);
			}
			
			if (filelist.Count == 0)
			{
				throw new FileNotFoundException("unable to find a file that matches pattern " + filepattern);
			}
			
			string filepath = filelist[0];
			
			string filepathNew = archiveroot + filepath.Substring(filepath.LastIndexOf('\\') + 1);
			
			// original file must exits
			if (!File.Exists(filepath))
			{
				throw new FileNotFoundException("file not found. \"" + filepath + "\"");
			}
			
			
			#if DEBUG
			ConsoleHelper.WriteLine("filepath: " + filepath);
			ConsoleHelper.WriteLine("filepathNew: " + filepathNew);
			#endif
			
			FileInfo originalFile = new FileInfo(filepath);
			FileInfo oldestFile = IO.GetNewestFile(archiveroot);
			
			
			#if DEBUG
			ConsoleHelper.WriteLine("originalFile.Length: " + originalFile.Length.ToString());
			if (oldestFile != null)
			{
				ConsoleHelper.WriteLine("oldestFile.Length: " + oldestFile.Length.ToString());
			}
			#endif
			
			if (oldestFile != null)
			{
				if (oldestFile.Length == originalFile.Length)
				{
					ConsoleHelper.WriteLine("Backup Not Required", ConsoleColor.DarkYellow);
					
					return Program.EXIT_OK;
				}
			}
			
			#if !DEBUG
			File.Move(filepath, filepathNew);
			#endif
			
			#if DEBUG
			File.Copy(filepath, filepathNew);
			#endif
			
			ConsoleHelper.WriteLine("Backup Successfully Completed", ConsoleColor.DarkYellow);
			
			return Program.EXIT_OK;
		}
	}
}
