using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

using skylib.Tools;

namespace skylib.sar
{
	public class WindowsRestart : BaseCommand
	{
		public WindowsRestart() : base("Windows Restart",
		                               new List<string> { "windows.restart", "win.restart" },
		                               @"-windows.restart <ip | computername> <domain/username> <password>",
		                               new List<string> { "-windows.restart 192.168.0.244 admin-username mypassword" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 4 || args.Length > 5)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string uncPath = args[1];
			if (uncPath.Substring(0,2) != @"\\") uncPath = @"\\" + uncPath;
			
			string userName = args[2];
			string password = args[3];
			
			int timeout = 5 * 60 * 1000;
			Int32.TryParse(args[4], out timeout);
			
			int exitcode;
			
			exitcode = ConsoleHelper.Shell("net", @"use " + uncPath + @" /DELETE");
			exitcode = ConsoleHelper.Shell("net", @"use " + uncPath + @" /USER:" + userName + " " + password);
			
			if (exitcode != 0)
			{
				ConsoleHelper.WriteLine("Login Failed", ConsoleColor.DarkYellow);
				return Program.EXIT_ERROR;
			}

			exitcode = ConsoleHelper.Shell("shutdown", @"/r /m " + uncPath + @" /t 0 /f");

			if (exitcode != 0)
			{
				ConsoleHelper.WriteLine(StringHelper.TrimStart(uncPath, 2) + " failed to restart", ConsoleColor.DarkYellow);
				return Program.EXIT_ERROR;
			}

			ConsoleHelper.WriteLine(StringHelper.TrimStart(uncPath, 2) + " is restarting", ConsoleColor.DarkYellow);
			
			Stopwatch timer = new Stopwatch();
			timer.Start();
			Thread.Sleep(100);
			
			exitcode = 0;
			while (exitcode == 0)
			{
				
				if (timer.ElapsedMilliseconds > timeout)
				{
					timer.Stop();
					ConsoleHelper.WriteLine(StringHelper.TrimStart(uncPath, 2) + " failed to restart", ConsoleColor.DarkYellow);
					return Program.EXIT_ERROR;
				}
				
				Thread.Sleep(1000);
				exitcode = ConsoleHelper.Shell("net", @"use " + uncPath + @" /DELETE");
				exitcode = ConsoleHelper.Shell("net", @"use " + uncPath + @" /USER:" + userName + " " + password + "aaa" + " /PERSISTENT:NO");
				exitcode = ConsoleHelper.Shell("net", @"use " + uncPath + @" /USER:" + userName + " " + password + " /PERSISTENT:NO");
			}
			
			exitcode = 1;
			while (exitcode != 0)
			{
				if (timer.ElapsedMilliseconds > timeout)
				{
					timer.Stop();
					ConsoleHelper.WriteLine(StringHelper.TrimStart(uncPath, 2) + " failed to restart", ConsoleColor.DarkYellow);
					return Program.EXIT_ERROR;
				}
				
				Thread.Sleep(1000);
				ConsoleHelper.Shell("net", @"use " + uncPath + @" /DELETE");
				exitcode = ConsoleHelper.Shell("net", @"use " + uncPath + @" /DELETE");
				exitcode = ConsoleHelper.Shell("net", @"use " + uncPath + @" /USER:" + userName + " " + password + "aaa" + " /PERSISTENT:NO");
				exitcode = ConsoleHelper.Shell("net", @"use " + uncPath + @" /USER:" + userName + " " + password + " /PERSISTENT:NO");
			}
			
			timer.Stop();
			ConsoleHelper.WriteLine(StringHelper.TrimStart(uncPath, 2) + " is ready", ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}
