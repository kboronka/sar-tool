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
	public class BuildSLN : BaseCommand
	{
		public BuildSLN() : base("Build .NET soultion",
		                            new List<string> { "build.net", "b.net" },
		                            "-b.net <.net version> <solution_path> <msbuild arguments>",
		                            new List<string> { "-b.net 3.5 sar.sln /p:Configuration=Release /p:Platform=\"x86\"" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 3)
			{
				throw new ArgumentException("too few arguments");
			}
			
			string netVersion = args[1];
			string netSoultion = args[2];
			
			// get list of msbuild versions availble

			string msbuildFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + @"\..\Microsoft.NET\Framework";
			Dictionary<string, string> msBuildFolders = new Dictionary<string, string>();
			
			foreach (string path in Directory.GetDirectories(msbuildFolder))
			{
				string version = path.Remove(0,path.LastIndexOf('\\')+1).Substring(1,3);
				string msBuildPath = path + "\\MSBuild.exe";
				if (File.Exists(msBuildPath))
				{
					msBuildFolders.Add(version, msBuildPath);
					#if DEBUG
					Console.WriteLine(version + " = " + msBuildPath);
					#endif
				}
			}
			
			// sanity - .net version installed
			if (!msBuildFolders.ContainsKey(netVersion))
			{
				throw new ArgumentOutOfRangeException(".net version");
			}
			
			// sanity - solution file exists
			if (!File.Exists(netSoultion))
			{
				throw new FileNotFoundException(netSoultion + " solution file not found");
			}
			
			string msbuildPath = msBuildFolders[netVersion];
			
			

			string arguments = netSoultion;
			
			for (int i = 3; i < args.Length; i++)
			{
				arguments += " " + args[i];
			}
			
			#if DEBUG
			Console.WriteLine(msbuildPath + " " + arguments);
			#endif

			
			Process compiler = new Process();
			compiler.StartInfo.FileName = msbuildPath;
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
