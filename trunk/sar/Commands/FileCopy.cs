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
	public class FileCopy : BaseCommand
	{
		public FileCopy(CommandHubBase commandHub) : base(commandHub, "File - Copy",
		                           new List<string> { "file.copy", "f.c" },
		                           @"-file.copy [root\filepattern] [destination] <speedlimt>",
		                           new List<string> { "-file.copy \"*.*\" \"\\\\10.242.211.57\transfer\\x\\\"" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 3 || args.Length > 4 )
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "searching";
			string destinationRoot = args[2];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref destinationRoot);

			string filePattern = args[1];
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			int counter = 0;
			for (int i = 0; i < files.Count; i++)
			{
				counter++;
				// FIXME: the percentage complete string is not working
				Progress.Message = i.ToString() + " of " + files.Count.ToString() + " " + ((i / files.Count) * 100).ToString() + "% ";
				string filename = IO.GetFilename(files[i]);
				string destinationPath = IO.GetRoot(destinationRoot + files[i].Substring(root.Length));

				string sourceFile = files[i];
				string destinationFile = destinationPath + filename;
				
				long len = IO.FileSize(files[i]);
				ConsoleHelper.WriteLine(filename + ": " + len.ToString());
				//if (!Directory.Exists(StringHelper.TrimEnd(destinationPath))) Directory.CreateDirectory(StringHelper.TrimEnd(destinationPath));
				//if (!File.Exists(destinationFile)) File.Copy(sourceFile, destinationFile);
			}

			ConsoleHelper.WriteLine(counter.ToString() + " File" + ((counter != 1) ? "s" : "") + " Copied", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
