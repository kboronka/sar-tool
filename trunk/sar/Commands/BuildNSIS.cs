/*
 * User: kboronka
 */

using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class BuildNSIS : BaseCommand
	{
		public BuildNSIS() : base("Build NSIS installer",
		                          new List<string> { "build.nsis", "b.nsis" },
		                          "-b.nsis <nsis_filepath>",
		                          new List<string> { @"-b.nsis src\Installer\chesscup.nsi" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 2)
			{
				throw new ArgumentException("too few arguments");
			}
			
			string nsiFile = args[1];
			
			// get list of makensis.exe file locations availble
			List<String> files = IO.GetAllFiles(IO.ProgramFilesx86, "makensis.exe");
			if (files.Count == 0)
			{
				files = IO.GetAllFiles(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles), "makensis.exe");
			}
			
			// sanity - nsis not installed
			if (files.Count == 0)
			{
				throw new FileNotFoundException("sar unable to locate makensis.exe");
			}
			
			string nsisPath = files[0];
			
			// sanity - solution file exists
			if (!File.Exists(nsiFile))
			{
				throw new FileNotFoundException(nsiFile + " nsis file not found");
			}
			

			string arguments = "";
			
			for (int i = 2; i < args.Length; i++)
			{
				arguments += " " + args[i];
			}
			
			arguments += " " + nsiFile;
			
			#if DEBUG
			ConsoleHelper.WriteLine(nsisPath + " " + arguments);
			#endif

			string output;
			int exitcode = ConsoleHelper.Shell(nsisPath + " " + arguments, out output);
			
			if (exitcode != 0)
			{
				ConsoleHelper.WriteLine("Build Failed", ConsoleColor.DarkYellow);
				ConsoleHelper.WriteLine(output, ConsoleColor.DarkCyan);
				return exitcode;
			}
			else
			{
				ConsoleHelper.WriteLine("Build Successfully Completed", ConsoleColor.DarkYellow);
				return Program.EXIT_OK;
			}
		}
	}
}
