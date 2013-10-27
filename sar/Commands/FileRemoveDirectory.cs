
using System;
using sar.Tools;
using System.Collections.Generic;
using System.IO;

namespace sar.Tools
{
	public class FileRemoveDirectory : BaseCommand
	{
		public FileRemoveDirectory() : base("File - Remove Directory",
		                                    new List<string> { "file.removedirectory", "f.rd", "d.d" },
		                                    "-f.d [filepattern]",
		                                    new List<string> { "-f.rd \"C:\\Temp\"" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Searching";
			string root = args[1];
			List<string> directories = IO.GetAllDirectories(root);
			
			if (!Directory.Exists(root)) throw new ApplicationException("Directory does not exist");
			
			
			if (!Program.NoWarning)
			{
				foreach (string directory in directories)
				{
					ConsoleHelper.Write("found: ", ConsoleColor.Cyan);
					ConsoleHelper.WriteLine(StringHelper.TrimStart(directory, root.Length));
				}

				ConsoleHelper.WriteLine(directories.Count.ToString() + " file" + ((directories.Count != 1) ? "s" : "") + " found");
			}
			
			if (directories.Count > 0)
			{
				if (Program.NoWarning || ConsoleHelper.Confirm("Destroy " + directories.Count.ToString() + " director" + ((directories.Count != 1) ? "ies" : "y") + "? (y/n)"))
				{
					Progress.Message = "Destroying " + root;
					
					try
					{
						Directory.Delete(root, true);
					}
					catch (Exception ex)
					{
						ConsoleHelper.Write("failed: ", ConsoleColor.Red);
						ConsoleHelper.WriteLine(root);
						
						if (Program.Debug)
						{
							ConsoleHelper.WriteException(ex);
						}
					}
				}
			}
			
			ConsoleHelper.WriteLine(directories.Count.ToString() + " Director" + ((directories.Count != 1) ? "ies" : "y") + " Destroyed", ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}