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
	public class NetLogin : BaseCommand
	{
		public NetLogin() : base("Login", new List<string> { "net.login", "n.login" }, "-net.login <domain> <username> <password>", new List<string>() { @"-net.login \\192.168.0.244\temp test testpw" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 4)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string uncPath = args[1];
			if (uncPath.Substring(0,2) != @"\\") uncPath = @"\\" + uncPath;
			
			string userName = args[2];
			string password = args[3];
			
			
			Process compiler = new Process();
			compiler.StartInfo.FileName = "net";
			compiler.StartInfo.Arguments =  @"use " + uncPath + @" /USER:" + userName + " " + password;
			compiler.StartInfo.UseShellExecute = false;
			compiler.StartInfo.RedirectStandardOutput = true;
			compiler.Start();
			string output = compiler.StandardOutput.ReadToEnd();
			compiler.WaitForExit();
			
			
			if (compiler.ExitCode != 0)
			{
				ConsoleHelper.WriteLine("Connection Failed", ConsoleColor.DarkYellow);
				ConsoleHelper.WriteLine(output, ConsoleColor.DarkCyan);
				ConsoleHelper.WriteLine("output: " + output);
				ConsoleHelper.WriteLine("exit code: " + compiler.ExitCode.ToString());
				return Program.EXIT_ERROR;
			}
			else
			{
				ConsoleHelper.WriteLine("Connection Successful", ConsoleColor.DarkYellow);
				return Program.EXIT_OK;
			}
		}
	}
}
