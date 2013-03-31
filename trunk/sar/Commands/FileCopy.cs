using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class FileCopy : BaseCommand
	{
		public FileCopy() : base("File Copy",
		                           new List<string> { "file.copy", "f.c" },
		                           @"-file.copy [root\filepattern] [destination] <speedlimt>",
		                           new List<string> { "-file.copy \"*.*\" \"\\\\10.242.211.57\transfer\\x\\\"" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 3 || args.Length > 4 )
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "searching";
			string destinationRoot = args[2];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref destinationRoot);

			string filePattern = args[1];
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			for (int i = 0; i < files.Count; i++)
			{
				// FIXME: the percentage complete string is not working
				Progress.Message = i.ToString() + " of " + files.Count.ToString() + " " + ((i / files.Count) * 100).ToString() + "% ";
				string filename = IO.GetFilename(files[i]);
				string destinationPath = IO.GetRoot(destinationRoot + files[i].Substring(root.Length));

				string sourceFile = files[i];
				string destinationFile = destinationPath + filename;
				
				long len = IO.FileSize(files[i]);
				ConsoleHelper.WriteLine(filename + ": " + len.ToString());
				//if (!Directory.Exists(StringHelper.TrimEnd(destinationPath))) Directory.CreateDirectory(StringHelper.TrimEnd(destinationPath));
				//if (!File.Exists(destinationFile)) File.Copy(sourceFile, destinationFile);
			}

			ConsoleHelper.WriteLine(files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : "") + " mirrored", ConsoleColor.DarkYellow);
			
			return Program.EXIT_OK;
		}
	}
}
