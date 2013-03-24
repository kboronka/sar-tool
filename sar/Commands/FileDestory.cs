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
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
						
			ConsoleHelper.WriteLine(files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : "") + " found");
			
			int count = 0;
			
			if (files.Count > 0)
			{
				if (Program.NoWarning || ConsoleHelper.Confirm("Destroy files? (y/n)"))
				{
					foreach (string file in files)
					{
						try
						{
							File.SetAttributes(file, FileAttributes.Normal);
							StreamWriter sw = new StreamWriter(file);
							sw.WriteLine("file corrupt");
							sw.Close();
							File.Delete(file);
							ConsoleHelper.Write("destroyed: ", ConsoleColor.Cyan);
							ConsoleHelper.WriteLine(file.Substring(root.Length + 1));
							count++;
						}
						catch (Exception ex)
						{
							ConsoleHelper.Write("failed: ", ConsoleColor.Red);
							ConsoleHelper.WriteLine(file.Substring(root.Length + 1));
							
							if (Program.Debug)
							{
								ConsoleHelper.WriteException(ex);
							}
						}
					}
				}
			}
			
			
			ConsoleHelper.WriteLine("Files destroyed: " + count.ToString() + " of " + files.Count.ToString());
			return Program.EXIT_OK;
		}
	}
}
