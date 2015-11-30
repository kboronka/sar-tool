using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using sar.Tools;
using sar.Base;


namespace sar.Commands
{
	public class svnAddExternals : Command
	{
		public svnAddExternals(Base.CommandHub parent) : base(parent, "svn Get Externals",
		                                            new List<string> { "svn.AddExternals" },
		                                            @"svn.AddExternals <svn/path> <filepattern>",
		                                            new List<string> { @"-svn.AddExternals http://svnserver/trunk/ *.cpp" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			string destinationRoot = Directory.GetCurrentDirectory();
			string externalRoot = args[1];
			string filePattern = args[2];
			
			// find svn executiable
			Progress.Message = "finding svn.exe";
			var svn = IO.FindApplication("svn.exe", @"TortoiseSVN\bin");
			if (!File.Exists(svn)) throw new ApplicationException("svn.exe not found");

			// create temp folder used to checkout all files from svn repo
			string tempFolder = ApplicationInfo.DataDirectory + Guid.NewGuid().ToString();
			Directory.CreateDirectory(tempFolder);


			// svn checkout --depth empty http://svnserver/trunk/proj
			Progress.Message = "checking out files";
			ConsoleHelper.Run(svn, " export --depth files --force " + externalRoot + @" """ + tempFolder + @"""");
			
			// get list of files 
			IO.IncludeSubFolders = false;
			string root = tempFolder;
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			

			string output = "";
			string error = "";
			
			ConsoleHelper.Run(svn, @" propget svn:externals", out output, out error);
			var externals = new List<string>();
			
			if (!output.Contains("'svn:externals' not found"))
			{
				foreach (var line in output.ToLines())
				{
					if (!string.IsNullOrEmpty(StringHelper.RemoveEmptyLines(line))) externals.Add(line);
				}
			}
			
			foreach (var file in files)
			{
				var filename = IO.GetFilename(file);
				var line = filename + " " + externalRoot + @"/" + filename;
				var duplicate = false;
				
				for (var i=0; i<externals.Count; i++)
				{
					if (externals[i].StartsWith(filename + " "))
					{
						duplicate = true;
						externals[i] = line;
						break;
					}
				}

				if (!duplicate) externals.Add(line);
			}
			
			Progress.Message = "generating externals file";
			var tempFile = tempFolder + @"\externals.txt";
			externals.Sort();
			IO.WriteFileLines(tempFile, externals, Encoding.ASCII);
			IO.WriteFileLines(@"C:\Users\kboronka\Documents\Jobs\OmniTrak-Sim\test.txt", externals, Encoding.ASCII);
			
			//svn propset svn:externals -F b:\externals.txt
			Progress.Message = "propset running";			
			ConsoleHelper.Run(svn, @" propset svn:externals -F """ + tempFile + @""" .");
			
		
			// delete tempFolder
			if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);
			
			ConsoleHelper.WriteLine("svn externals set", ConsoleColor.Yellow);
			return ConsoleHelper.EXIT_ERROR;
		}
	}
}
