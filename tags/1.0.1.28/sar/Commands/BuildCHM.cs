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
	public class BuildCHM : BaseCommand
	{
		public BuildCHM() : base("Build CHM help file",
		                         new List<string> { "build.chm", "b.chm" },
		                         "-b.chm <hhp_filepath>",
		                         new List<string> { @"-b.chm help\help.hhp" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 2)
			{
				throw new ArgumentException("too few arguments");
			}
			
			string hhpFile = args[1];
			
			// get list of hhc.exe file locations availble
			List<String> files = IO.GetAllFiles(IO.ProgramFilesx86, "hhc.exe");
			if (files.Count == 0)
			{
				files = IO.GetAllFiles(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles), "hhc.exe");
			}
			
			// sanity - hhc.exe not installed
			if (files.Count == 0)
			{
				throw new FileNotFoundException("sar unable to locate hhc.exe");
			}
			
			string hhcPath = files[0];
			
			// sanity - solution file exists
			if (!File.Exists(hhpFile))
			{
				throw new FileNotFoundException(hhpFile + " hhp file not found");
			}
			

			string arguments = hhpFile;
			
			for (int i = 2; i < args.Length; i++)
			{
				arguments += " " + args[i];
			}
			
			#if DEBUG
			ConsoleHelper.WriteLine(hhpFile + " " + arguments);
			#endif

			
			Process compiler = new Process();
			compiler.StartInfo.FileName = hhcPath;
			compiler.StartInfo.Arguments = arguments;
			compiler.StartInfo.UseShellExecute = false;
			compiler.StartInfo.RedirectStandardOutput = true;
			compiler.Start();
			string output = compiler.StandardOutput.ReadToEnd();
			compiler.WaitForExit();
			
			if (compiler.ExitCode != 1)
			{
				ConsoleHelper.WriteLine("Build Failed", ConsoleColor.DarkYellow);
				ConsoleHelper.WriteLine(output, ConsoleColor.DarkCyan);
				return Program.EXIT_ERROR;
			}
			else
			{
				ConsoleHelper.WriteLine("Build Successfully Completed", ConsoleColor.DarkYellow);
				return Program.EXIT_OK;
			}
		}
	}
}
