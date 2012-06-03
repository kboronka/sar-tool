/*
 * User: Kevin Boronka (kboronka@gmail.com)
 * Date: 2/7/2012
 * Time: 11:57 PM
 */
using System;
using System.IO;
using System.Collections.Generic;
using SkylaLib.Tools;

namespace SkylaLib.sar
{
	class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				Console.WriteLine(AssemblyInfo.Name + "  v" + AssemblyInfo.Version + "  " + AssemblyInfo.Copyright);
				Console.WriteLine();
				
				if (args.Length == 0)
				{
					Usage();
					return;
				}
				
				string command = args[0].ToLower();

				if (command[0] != '-')
				{
					Usage();
					return;
				}
				
				command = command.Substring(1);

				#if DEBUG
				int i = 0;
				foreach (string arg in args)
				{
					Console.WriteLine("args[" + (i++).ToString() + "]=" + arg);
				}

				Console.WriteLine("Command = " + command);
				#endif
				
				switch (command)
				{
					case "replace":
						SearchReplace(args);
						break;
					case "help":
						Usage();
						break;
					default:
						Console.WriteLine("Unknown command");
						Console.WriteLine("");
						Usage();
						break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			
			#if DEBUG
			Console.ReadKey();
			#endif
		}
		
		public static void Usage()
		{
			Console.WriteLine("Usage: sar -replace <file_search_pattern> <search_text> <replace_text>");
			Console.WriteLine("sar -replace \"AssemblyInfo.cs\" \"0.0.0.0\" \"1.0.0.0\"");
			#if DEBUG
			Console.ReadKey();
			#endif
		}
		
		public static void SearchReplace(string[] args)
		{
			// sanity check
			if (args.Length != 4)
			{
				Usage();
				return;
			}
			
			string filePattern = args[1];
			string searchString = args[2];
			string replaceString = args[3];
			
			
			string root = Directory.GetCurrentDirectory();
			List<string> changedFiles = IO.SearchAndReplaceInFiles(root, filePattern, searchString, replaceString);
			
			if (changedFiles.Count > 0)
			{
				Console.WriteLine("Replacments made in the following file" + ((changedFiles.Count > 1) ? "s" : ""));
				foreach (string file in changedFiles)
				{
					Console.WriteLine(file.Replace(root, ""));
				}
			}
			else
			{
				Console.WriteLine("search string was not found");
			}
		}
	}
}