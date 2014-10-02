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
			
			ConsoleHelper.DebugWriteLine("args[1]: " + args[1]);
			ConsoleHelper.DebugWriteLine("args[2]: " + args[2]);
			
			Progress.Message = "Searching";
			string searchPattern = args[1];
			string searchRoot = Directory.GetCurrentDirectory();
			
			IO.CheckRootAndPattern(ref searchRoot, ref searchPattern);
			List<string> files = IO.GetAllFiles(searchRoot, searchPattern);

			ConsoleHelper.DebugWriteLine("search pattern: " + searchPattern);
			ConsoleHelper.DebugWriteLine("search root: " + searchRoot);
			
			if (files.Count == 0)
			{
				ConsoleHelper.WriteException(new FileNotFoundException("unable to find any files that match pattern: [" + searchPattern + "]  in root: [" + searchRoot + "]"));
				return ConsoleHelper.EXIT_ERROR;
			}
			else
			{
				Progress.Message = "Locating Archive Folder";
				string archiveRoot = args[2];
				string archivePattern = "*.*";
				
				archiveRoot = IO.CheckPath(archiveRoot, "");
				
				ConsoleHelper.DebugWriteLine("archivePattern: " + archivePattern);
				ConsoleHelper.DebugWriteLine("archiveRoot: " + archiveRoot);
				if (!Directory.Exists(archiveRoot))	Directory.CreateDirectory(archiveRoot);

				int counter = 0;
				foreach (string originalFile in files)
				{
					if (!originalFile.Contains(archiveRoot))
					{
						if (this.commandHub.IncludeSVN || !IO.IsSVN(originalFile))
						{
							string fileRelativePath = StringHelper.TrimStart(originalFile, searchRoot.Length);
							string backupFile = archiveRoot + originalFile.Substring(searchRoot.Length);
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
			}
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}
