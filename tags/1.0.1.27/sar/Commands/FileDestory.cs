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
	public class FileDestory : BaseCommand
	{
		public FileDestory() : base("Destory Files",
		                            new List<string> { "file.destroy", "f.d" },
		                            "-f.d <filepattern>",
		                            new List<string> { "-f.d \"*.vmdk\"" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string filePattern = args[1];
			
			string root = Directory.GetCurrentDirectory();
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			foreach (string file in files)
			{
				try
				{
					File.SetAttributes(file, FileAttributes.Normal);
					StreamWriter sw = new StreamWriter(file);
					sw.WriteLine("file corrupt");
					sw.Close();
					File.Delete(file);
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.Write("passed: ");
					Console.ResetColor();
					Console.WriteLine(file.Substring(root.Length + 1));
				}
				catch (Exception ex)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write("failed: ");
					Console.ResetColor();
					Console.WriteLine(file.Substring(root.Length + 1));
					
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("error: " + ex.Message);
					Console.ResetColor();
				}
			}

			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine("Files Found " + files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : ""));
			Console.ResetColor();
			
			return Program.EXIT_OK;
		}
	}
}
