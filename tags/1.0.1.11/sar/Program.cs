/*
 * User: Kevin Boronka (kboronka@gmail.com)
 * Date: 2/7/2012
 * Time: 11:57 PM
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using skylib.Tools;

namespace skylib.sar
{
	class Program
	{
		private const int EXIT_OK = 0;
		private const int EXIT_ERROR = 1;

		public static int Main(string[] args)
		{
			try
			{
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(AssemblyInfo.Name + "  v" + AssemblyInfo.Version + "  " + AssemblyInfo.Copyright);
				Console.WriteLine();
				Console.ResetColor();
				
				if (args.Length == 0)
				{
					Usage();
					return EXIT_ERROR;
				}
				
				string command = args[0].ToLower();

				if (command[0] != '-' && command[0] != '/' )
				{
					Usage();
					return EXIT_ERROR;
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
				int exitCode = EXIT_OK;
				
				switch (command)
				{
					case "replace":
					case "r":
						SearchReplace(args);
						break;
					case "lv_ver":
						LabVIEW_Version(args);
						break;
					case "build.net":
					case "b.net":
						exitCode = Build_DotNet(args);
						break;
					case "kill":
					case "k":
					case "shutdown":
					case "s":
						Kill(args);
						break;
					case "help":
					case "h":
					case "?":
						Usage();
						break;
					case "test":
						Test();
						break;
					default:
						Console.WriteLine("Unknown command");
						Console.WriteLine("");
						exitCode = EXIT_ERROR;
						Usage();
						break;
				}
				
				#if DEBUG
				Console.ReadKey();
				#endif
				return exitCode;
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;				
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				#if DEBUG
				Console.ReadKey();
				#endif
				return EXIT_ERROR;
			}
		}
		
		public static void Test()
		{
			Console.Out.Write("thistest");
		}
		
		public static void Usage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("\t -replace <file_search_pattern> <search_text> <replace_text>");
			Console.WriteLine("\t -lv_ver <lvproj_file> <version>");
			Console.WriteLine("\t -kill <process>");
			Console.WriteLine("Examples:");
			Console.WriteLine("\t sar -r \"AssemblyInfo.cs\" \"0.0.0.0\" \"1.0.0.0\"");
			//Console.WriteLine("\t sar -r AssemblyInfo.* ((Version)\\(\\\"\\d+\\.\\d+\\.\\d+\\.\\d+\\\"\\)) \"Version(\\\"%VERSION%\\\")\"");
			Console.WriteLine("\t sar -lv_ver \"*.lvproj_file\" \"1.0.2.1\"");
			Console.WriteLine("\t sar -kill notepad.exe");
			
			#if DEBUG
			string content = "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">9</Property><Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">99</Property>";
			string search = "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">[0-9]?[0-9]?[0-9]</Property>";
			string replace = "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">1</Property>";
			string newcontent = Regex.Replace(content, search, replace);
			Console.WriteLine("content = " + content);
			Console.WriteLine("search = " + search);
			Console.WriteLine("replace " + replace);
			Console.WriteLine("Result: ");
			Console.WriteLine("\t" + Regex.Replace(content, search, replace));
			
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
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">" + version[0] + "</Property>"));
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.minor\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.minor\" Type=\"Int\">" + version[1] + "</Property>"));
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.patch\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.patch\" Type=\"Int\">" + version[2] + "</Property>"));
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">" + version[3] + "</Property>"));
			
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

		public static void Kill(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				Usage();
				return;
			}
			
			string processName = args[1];
			
			Process[] foundProcess = Process.GetProcessesByName(processName);
			if (foundProcess.Length != 0)
			{

				foreach (Process process in foundProcess)
				{
					Console.WriteLine(process.ProcessName + " killed");
					process.Kill();
				}
			}
			else
			{
				Console.WriteLine(processName + " not running");
			}
		}
		
		public static int Build_DotNet(string[] args)
		{
			// http://stackoverflow.com/questions/536539/how-to-call-msbuild-from-c-sharp
			//:: Microsoft.NET v2.0.50727					http://www.microsoft.com/download/en/details.aspx?id=19
			//:: Microsoft.NET v4.0.30319					http://www.microsoft.com/en-us/download/confirmation.aspx?id=17718
			// %MSBUILD2% %SOLUTION% /p:Configuration=%CONFIG% /p:Platform="x86"
			// set MSBUILD2="%WinDir%\Microsoft.NET\Framework\v2.0.50727\msbuild.exe"
			// set MSBUILD4="%WinDir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
			
			// version
			// solution
			// other
			
			// sanity check
			if (args.Length <= 2)
			{
				Usage();
				return EXIT_ERROR;
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
				Console.WriteLine("Build Failed");
				Console.WriteLine(output);
				Console.WriteLine("exit code: " + compiler.ExitCode.ToString());
				return compiler.ExitCode;
			}
			else
			{
				Console.WriteLine("Build Successfully Completed");
				return EXIT_OK;
			}
		}
	}
}