/* Copyright (C) 2013 Kevin Boronka
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
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class FileBackup : BaseCommand
	{
		public FileBackup() : base("File - Backup",
		                       new List<string> { "file.backup", "f.bk" },
		                       @"-file.backup [FilePath] [backup_location]",
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
			
			
			string filepattern = args[1];
			string archiveroot = args[2];
			if (archiveroot.Substring(archiveroot.Length - 1) != "\\") archiveroot = archiveroot + '\\';
			
			ConsoleHelper.DebugWriteLine("filepattern: " + filepattern);
			ConsoleHelper.DebugWriteLine("archiveroot: " + archiveroot);
			
			
			List<string> filelist = IO.GetAllFiles(filepattern);
			
			if (!Directory.Exists(archiveroot))
			{
				throw new DirectoryNotFoundException("unable to find storage folder " + archiveroot);
			}
			
			if (filelist.Count == 0)
			{
				throw new FileNotFoundException("unable to find a file that matches pattern " + filepattern);
			}
			
			string filepath = filelist[0];
			
			string filepathNew = archiveroot + filepath.Substring(filepath.LastIndexOf('\\') + 1);
			
			// original file must exits
			if (!File.Exists(filepath))
			{
				throw new FileNotFoundException("file not found. \"" + filepath + "\"");
			}
			
			
			ConsoleHelper.DebugWriteLine("filepath: " + filepath);
			ConsoleHelper.DebugWriteLine("filepathNew: " + filepathNew);
			
			FileInfo originalFile = new FileInfo(filepath);
			FileInfo oldestFile = IO.GetNewestFile(archiveroot);
			
			
			ConsoleHelper.DebugWriteLine("originalFile.Length: " + originalFile.Length.ToString());
			if (oldestFile != null)
			{
				ConsoleHelper.DebugWriteLine("oldestFile.Length: " + oldestFile.Length.ToString());
			}
			
			if (oldestFile != null)
			{
				if (oldestFile.Length == originalFile.Length)
				{
					ConsoleHelper.WriteLine("Backup Not Required", ConsoleColor.DarkYellow);
					
					return Program.EXIT_OK;
				}
			}
			
			#if !DEBUG
			File.Move(filepath, filepathNew);
			#endif
			
			#if DEBUG
			File.Copy(filepath, filepathNew);
			#endif
			
			ConsoleHelper.WriteLine("Backup Successfully Completed", ConsoleColor.DarkYellow);
			
			return Program.EXIT_OK;
		}
	}
}
