using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class CodeClean : BaseCommand
	{
		public CodeClean() : base("Code - Clean",
		                             new List<string> { "code.clean", "c.clean", "c.c" },
		                             @"-code.reindent [filepath/pattern]",
		                             new List<string> { "-code.clean *.vb" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Searching";
			string filePattern = args[1];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			ConsoleHelper.DebugWriteLine("pattern: " + filePattern);
			ConsoleHelper.DebugWriteLine("root: " + root);
			if (files.Count == 0) throw new FileNotFoundException("unable to find any files that match pattern: \"" + filePattern + "\" in root: \"" + root + "\"");

			int counter = 0;
			int changes = 0;
			foreach (string file in files)
			{
				try
				{
					Progress.Message = "Cleaning " + IO.GetFilename(file);
					
					switch (IO.GetFileExtension(file).ToLower())
					{
						case "vb":
							counter++;
							// fix the "_ Then" lines
							changes += IO.SearchAndReplaceInFile(file, @"(\s*([_*]\s*\r{1}\n{1}\s+)Then)", @" Then");
							
							// remove empty lines after "Then"
							changes += IO.SearchAndReplaceInFile(file, @"(Then\s*\r{0,1}\n{1}\s*\r{0,1}\n{1})", "Then\r\n");
							
							// remove the xml documentation
							changes += IO.SearchAndReplaceInFile(file, @"(\r{0,1}\n{1}\s*\'{3}\s*[^\r]+)", @"");
							
							break;
						default:
							break;
					}
				}
				catch (Exception ex)
				{
					ConsoleHelper.WriteException(ex);
				}
			}
			
			
			ConsoleHelper.WriteLine(changes.ToString() + " line" + ((changes != 1) ? "s" : "") + " cleaned", ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}
