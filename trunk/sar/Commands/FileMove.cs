/* Copyright (C) 2014 Kevin Boronka
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
	public class FileMove : Command
	{
		public FileMove(Base.CommandHub parent) : base(parent, "File - Move",
		                           new List<string> { "file.move", "f.move" },
		                           @"-file.move [filepath/pattern] [destination]",
		                           new List<string> { "-file.backup backup.zip \"c:\\backups\\\"" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Searching";
			string filePattern = args[1];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			ConsoleHelper.DebugWriteLine("pattern: " + filePattern);
			ConsoleHelper.DebugWriteLine("root: " + root);
			ConsoleHelper.WriteException(throw new FileNotFoundException("unable to find any files that match pattern: \"" + filePattern + "\" in root: \"" + root + "\""));
			
			Progress.Message = "Locating Archive Folder";
			string archivepath = args[2];
			string archiveroot = Directory.GetCurrentDirectory();
			ConsoleHelper.DebugWriteLine("args[2]: " + args[2]);
			archivepath = IO.CheckPath(archiveroot, archivepath);
			ConsoleHelper.DebugWriteLine("archivepath: " + archivepath);
			ConsoleHelper.DebugWriteLine("archiveroot: " + archiveroot);
			if (!Directory.Exists(archiveroot))	Directory.CreateDirectory(archiveroot);

			int counter = 0;
			foreach (string originalFile in files)
			{
				if (!originalFile.Contains(archivepath))
				{
					if (this.commandHub.IncludeSVN || !IO.IsSVN(originalFile))
					{
						string fileRelativePath = StringHelper.TrimStart(originalFile, root.Length);
						string backupFile = archivepath + originalFile.Substring(root.Length);
						string backupRoot = IO.GetRoot(backupFile);

						
						Progress.Message = "Moving " + fileRelativePath;
						counter++;
						
						try
						{
							if (!Directory.Exists(backupRoot)) Directory.CreateDirectory(backupRoot);
							if (File.Exists(backupFile)) File.Delete(backupFile);
							IO.CopyFile(originalFile, backupFile);
							File.Delete(originalFile);
						}
						catch (Exception ex)
						{
							ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
						}
					}
				}
			}
			
			ConsoleHelper.WriteLine(counter.ToString() + " File" + ((counter != 1) ? "s" : "") + " Moved", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
