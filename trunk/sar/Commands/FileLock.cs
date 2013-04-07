
using System;
using skylib.Tools;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace skylib.sar
{
	public class FileLock : BaseCommand
	{
		public FileLock() : base("File - Lock",
		                         new List<string> { "file.lock", "f.lock" },
		                         "-file.find [filepattern] <timeout>",
		                         new List<string> { "-file.find \"*.exe\" 10000" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Waiting for lock";
			string filePattern = args[1];
			
			int timeout = 5 * 60 * 1000;
			Int32.TryParse(args[2], out timeout);
			
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = new List<string>();
			bool error = false;
			Stopwatch timer = new Stopwatch();
			timer.Start();
			
			do
			{
				try
				{
					files = IO.GetAllFiles(root, filePattern);
					error = false;
				}
				catch
				{
					Thread.Sleep(200);
					error = true;
				}
			} while (error && !(timer.ElapsedMilliseconds > timeout));
			
			
			if (error)
			{
				ConsoleHelper.WriteLine("Files not found", ConsoleColor.DarkYellow);
				return Program.EXIT_ERROR;
			}
			else
			{
				ConsoleHelper.WriteLine("Files Found " + files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : ""), ConsoleColor.DarkYellow);
			}
			
			return Program.EXIT_OK;
		}
	}
}
