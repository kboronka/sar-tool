/*
 * User: Kevin Boronka (kboronka@gmail.com)
 * Date: 2/7/2012
 * Time: 11:57 PM
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
					case "r":
						SearchReplace(args);
						break;
					case "lv_ver":
						LabVIEW_Version(args);
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
			Console.WriteLine("Usage:");
			Console.WriteLine("\t -replace <file_search_pattern> <search_text> <replace_text>");
			Console.WriteLine("\t -lv_ver <lvproj_file> <version>");
			Console.WriteLine("Examples:");
			Console.WriteLine("\t sar -replace \"AssemblyInfo.cs\" \"0.0.0.0\" \"1.0.0.0\"");
			Console.WriteLine("\t sar -lv_ver \"*.lvproj_file\" \"1.0.2.1\"");
			
			#if DEBUG
			string content = "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">9</Property>";
			string search = "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">*</Property>";
			string replace = "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">1</Property>";
			string newcontent = Regex.Replace(content, IO.WildcardToRegex(search), replace);
			Console.WriteLine("content = " + content);
			Console.WriteLine("search = " + search);
			Console.WriteLine("replace " + replace);
			Console.WriteLine("Result: ");
			Console.WriteLine("\t" + Regex.Replace(content, IO.WildcardToRegex(search), replace));
			
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
		
		public static void LabVIEW_Version(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				Usage();
				return;
			}
			
			string filePattern = args[1];
			string[] version = args[2].Split('.');
			
			if (version.Length != 4)
			{
				Usage();
				return;
			}
			
			string root = Directory.GetCurrentDirectory();
			
			/*
			<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">9</Property>
			<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">9</Property>
			<Property Name=\"TgtF_fileVersion.minor\" Type=\"Int\">9</Property>
			<Property Name=\"TgtF_fileVersion.patch\" Type=\"Int\">9</Property>
			 */
			
			List<string> changedFiles = new List<string>();
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">*</Property>", "<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">" + version[0] + "</Property>"));
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.minor\" Type=\"Int\">*</Property>", "<Property Name=\"TgtF_fileVersion.minor\" Type=\"Int\">" + version[1] + "</Property>"));
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.patch\" Type=\"Int\">*</Property>", "<Property Name=\"TgtF_fileVersion.patch\" Type=\"Int\">" + version[2] + "</Property>"));
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">*</Property>", "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">" + version[3] + "</Property>"));
			
			// remove duplicates
			changedFiles.Sort();
			Int32 index = 0;
			while (index < changedFiles.Count - 1)
			{
				if (changedFiles[index] == changedFiles[index + 1])
					changedFiles.RemoveAt(index);
				else
					index++;
			}


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