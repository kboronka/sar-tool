/*
 * Created by SharpDevelop.
 * User: Boronka
 * Date: 5/22/2013
 * Time: 7:03 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class AssemblyInfoVersion : BaseCommand
	{
		public AssemblyInfoVersion() : base("Set AssemblyInfo version number",
		                                    new List<string> { "assembly.version", "assy.ver" },
		                                    "-assembly.version [AssemblyInfo file] [version]",
		                                    new List<string> { "-assembly.version \"AssemblyInfo.cs\" \"1.0.2.1\"" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2 && args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string version = args[1];
			string filePattern = "AssemblyInfo.*";
			
			if (args.Length == 3)
			{
				version = args[2];
				filePattern = args[1];
			}

			string[] versionNumbers = version.Split('.');
			
			if (versionNumbers.Length != 4)
			{
				throw new ArgumentException("incorrect version format");
			}
			
			foreach (string number in versionNumbers)
			{
				int val;
				if (!int.TryParse(number, out val))
				{
					throw new ArgumentException("incorrect version format");
				}
			}

			Progress.Message = "Searching";
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			List<string> changedFiles = new List<string>();
			
			foreach (string file in files)
			{
				if (IO.SearchAndReplaceInFile(file, "((Version)\\(\\\"\\d+\\.\\d+\\.\\d+\\.\\d+\\\"\\))", "Version(\"" + version + "\")"))
				{
					changedFiles.Add(file);
				}
				
				if (IO.SearchAndReplaceInFile(file, "((Version)\\(\\\"\\d+\\.\\d+\\.\\*+\\\"\\))", "Version(\"" + version + "\")"))
				{
					changedFiles.Add(file);
				}
			}
			
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
				ConsoleHelper.WriteLine("Replacments made in the following file" + ((changedFiles.Count > 1) ? "s" : ""));
				foreach (string file in changedFiles)
				{
					ConsoleHelper.WriteLine(file.Replace(root, ""));
				}
			}
			else
			{
				ConsoleHelper.WriteLine("search string was not found");
			}
			
			return Program.EXIT_OK;
		}
	}
}
