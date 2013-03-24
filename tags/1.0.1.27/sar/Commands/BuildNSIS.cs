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
			Console.WriteLine(nsisPath + " " + arguments);
			#endif

			
			Process compiler = new Process();
			compiler.StartInfo.FileName = nsisPath;
			compiler.StartInfo.Arguments = arguments;
			compiler.StartInfo.UseShellExecute = false;
			compiler.StartInfo.RedirectStandardOutput = true;
			compiler.Start();
			string output = compiler.StandardOutput.ReadToEnd();
			compiler.WaitForExit();
			
			if (compiler.ExitCode != 0)
			{
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("Build Failed");
				Console.ResetColor();
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.WriteLine(output);
				Console.ResetColor();
				Console.WriteLine("exit code: " + compiler.ExitCode.ToString());
				return compiler.ExitCode;
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("Build Successfully Completed");
				Console.ResetColor();
				return Program.EXIT_OK;
			}
		}
	}
}
