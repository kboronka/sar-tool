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
	public class VboxManage : BaseCommand
	{
		public VboxManage() : base("Vbox Manage Tool",
		                           new List<string> { "vbox.manage", "vb.manage" },
		                           @"-vbox.manage [arg1] [arg2] [arg3]",
		                           new List<string> { "-vbox.manage modifyhd \"WinXP-disk1.vdi\" --resize 3000" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 2)
			{
				throw new ArgumentException("too few arguments");
			}
			
			string command = args[1];
			
			// get list of VBoxManage.exe file locations availble
			List<String> files = IO.GetAllFiles(IO.ProgramFiles + @"\Oracle\VirtualBox", "VBoxManage.exe");
			if (files.Count == 0)
			{
				files = IO.GetAllFiles(IO.ProgramFilesx86 + @"\Oracle\VirtualBox", "VBoxManage.exe");
			}
			
			// sanity - vbox not installed
			if (files.Count == 0)
			{
				throw new FileNotFoundException("sar unable to locate VBoxManage.exe");
			}
			
			string vboxManagePath = files[0];
			
			// sanity - solution file exists
			if (!File.Exists(command))
			{
				throw new FileNotFoundException(command + " nsis file not found");
			}
			

			string arguments = "";
			
			for (int i = 2; i < args.Length; i++)
			{
				arguments += " " + args[i];
			}
			
			#if DEBUG
			ConsoleHelper.WriteLine(vboxManagePath + " " + arguments);
			#endif

			
			Process compiler = new Process();
			compiler.StartInfo.FileName = vboxManagePath;
			compiler.StartInfo.Arguments = arguments;
			compiler.StartInfo.UseShellExecute = false;
			compiler.StartInfo.RedirectStandardOutput = true;
			compiler.Start();
			string output = compiler.StandardOutput.ReadToEnd();
			compiler.WaitForExit();
			
			if (compiler.ExitCode != 0)
			{
				ConsoleHelper.WriteLine("Command Failed", ConsoleColor.DarkYellow);
				ConsoleHelper.WriteLine(output, ConsoleColor.DarkCyan);
				ConsoleHelper.WriteLine("exit code: " + compiler.ExitCode.ToString());
				return compiler.ExitCode;
			}
			else
			{
				ConsoleHelper.WriteLine("Command Successfully Completed", ConsoleColor.DarkYellow);
				return Program.EXIT_OK;
			}
		}
	}
}
