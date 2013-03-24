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
	public class FileFind : BaseCommand
	{
		public FileFind() : base("Find Files",
		                         new List<string> { "file.find", "f.f" },
		                         "-f.f <filepattern>",
		                         new List<string> { "-file.find \"*.vmdk\"" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string filePattern = args[1];
			
			string root = Directory.GetCurrentDirectory();
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			foreach (string file in files)
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write("found: ");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.ResetColor();
				Console.WriteLine(file.Substring(root.Length + 1));
			}

			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine("Files Found " + files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : ""));
			Console.ResetColor();
			
			return Program.EXIT_OK;
		}
	}
}
