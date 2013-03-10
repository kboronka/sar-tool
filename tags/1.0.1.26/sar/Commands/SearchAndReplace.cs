/*
 * User: kboronka
 */

using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class SearchAndReplace : BaseCommand
	{
		public SearchAndReplace() : base("Search And Replace", 
		                                new List<string> { "replace", "r" },
		                                "-replace <file_search_pattern> <search_text> <replace_text>",
		                               new List<string> { 
		                                	"-r \"AssemblyInfo.cs\" \"0.0.0.0\" \"1.0.0.0\"",
		                                	"-r AssemblyInfo.* ((Version)\\(\\\"\\d+\\.\\d+\\.\\d+\\.\\d+\\\"\\)) \"Version(\\\"%VERSION%\\\")\"" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 4)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string filePattern = args[1];
			string searchString = args[2];
			string replaceString = args[3];
			
			
			string root = Directory.GetCurrentDirectory();
			List<string> changedFiles = IO.SearchAndReplaceInFiles(root, filePattern, searchString, replaceString);

			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine("Replacments made in " + changedFiles.Count.ToString() + " file" + ((changedFiles.Count != 1) ? "s" : ""));
			Console.ResetColor();
			
			return Program.EXIT_OK;
		}
	}
}
