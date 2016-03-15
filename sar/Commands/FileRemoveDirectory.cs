/* Copyright (C) 2016 Kevin Boronka
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class FileRemoveDirectory : Command
	{
		public FileRemoveDirectory(Base.CommandHub parent) : base(parent, "File - Remove Directory",
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
			string filePattern = root.Substring(root.LastIndexOf(@"\") + 1, root.Length - root.LastIndexOf(@"\") - 1);
			root = root.Substring(0, root.LastIndexOf(@"\") + 1);
			
			if (!Directory.Exists(root)) throw new ApplicationException("Directory does not exist");

			var foundDirectories = new List<string>();
			foundDirectories.AddRange(GetDirectories(root, filePattern, !this.commandHub.IncludeSubFolders));
			
			if (foundDirectories.Count == 0) throw new ApplicationException("\"" + filePattern + "\" Folders not found in \"" + root + "\"");
			
			
			var subDirectories = new List<string>();
			var files = new List<string>();
			
			foreach (string directory in foundDirectories)
			{
				if (this.commandHub.Debug)
				{
					foreach (string subDirectory in IO.GetAllDirectories(directory))
					{
						ConsoleHelper.Write("found: " , ConsoleColor.Cyan);
						ConsoleHelper.WriteLine(StringHelper.TrimStart(subDirectory, root.Length));
					}
				}
				
				subDirectories.AddRange(IO.GetAllDirectories(directory));
				
				if (this.commandHub.Debug)
				{
					foreach (string file in IO.GetAllFiles(directory))
					{
						ConsoleHelper.Write("found: ", ConsoleColor.Cyan);
						ConsoleHelper.WriteLine(StringHelper.TrimStart(file, root.Length));
					}
				}
				
				files.AddRange(IO.GetAllFiles(directory));
			}


			if (!this.commandHub.NoWarning)
			{
				ConsoleHelper.WriteLine("");
				ConsoleHelper.Write("found: ", ConsoleColor.Yellow);
				ConsoleHelper.WriteLine(foundDirectories.Count.ToString() + " " + ((foundDirectories.Count != 1) ? "directories" : "directory"));
				ConsoleHelper.Write("containing: ", ConsoleColor.Yellow);
				ConsoleHelper.WriteLine(subDirectories.Count.ToString() + " " + ((subDirectories.Count != 1) ? "subdirectories" : "subdirectory"));
				ConsoleHelper.Write("containing: ", ConsoleColor.Yellow);
				ConsoleHelper.WriteLine(files.Count.ToString() + " " + ((subDirectories.Count != 1) ? "files" : "file"));
			}
			
			if (foundDirectories.Count > 0)
			{
				if (this.commandHub.NoWarning || ConsoleHelper.Confirm("Delete " + foundDirectories.Count.ToString() + " " + ((foundDirectories.Count != 1) ? "directories" : "directory") + "? (y/n)"))
				{
					Progress.Message = "Deleting " + root;
					
					try
					{
						foreach (string directory in foundDirectories)
						{
							filePattern = "*.*";
							root = directory;
							IO.CheckRootAndPattern(ref root, ref filePattern);
							files = IO.GetAllFiles(root, filePattern);
			
							// make every file not read-only
							foreach (string filepath in files)
							{
								File.SetAttributes(filepath, FileAttributes.Normal);
							}
							
							// delete the directory
							Directory.Delete(directory, true);
						}
					}
					catch (Exception ex)
					{
						ConsoleHelper.Write("failed: ", ConsoleColor.Red);
						ConsoleHelper.WriteLine(root);
						
						if (this.commandHub.Debug)
						{
							ConsoleHelper.WriteException(ex);
						}
					}
				}
			}
			
			ConsoleHelper.WriteLine(foundDirectories.Count.ToString() + " Director" + ((foundDirectories.Count != 1) ? "ies" : "y") + " deleted", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}

		public List<string> GetDirectories(string root, string filePattern)
		{
			return GetDirectories(root, filePattern, false);
		}
		
		public List<string> GetDirectories(string root, string filePattern, bool rootOnly)
		{
			var directories = new List<string>();
			
			try
			{
				foreach (var directory in Directory.GetDirectories(root))
				{
					directories.Add(directory);
					if (!rootOnly) directories.AddRange(GetDirectories(directory, filePattern));
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				ConsoleHelper.WriteException(ex);
			}
			
			var filteredDirectories = new List<string>();
			foreach (var directory in directories)
			{
				if (directory.EndsWith(@"\" + filePattern))
				{
					filteredDirectories.Add(directory);
				}
			}
			
			return filteredDirectories;
		}
	}
}