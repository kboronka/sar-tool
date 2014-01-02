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
using sar.Tools;
using System.Collections.Generic;
using System.IO;

namespace sar.Tools
{
	public class FileMirror : BaseCommand
	{
		public FileMirror() : base("File - Mirror",
		                           new List<string> { "file.mirror", "f.m" },
		                           @"-f.m [root\filepattern] [destination]",
		                           new List<string> { "-file.mirror \"*.*\" \"\\\\10.242.211.57\transfer\\x\\\"" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
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
			
			for (int i = 0; i < files.Count; i++)
			{
				// FIXME: the percentage complete string is not working
				Progress.Message = i.ToString() + " of " + files.Count.ToString() + " " + ((i / files.Count) * 100).ToString() + "% ";
				string filename = IO.GetFilename(files[i]);
				string destinationPath = IO.GetRoot(destinationRoot + files[i].Substring(root.Length));

				string sourceFile = files[i];
				string destinationFile = destinationPath + filename;

				if (!Directory.Exists(StringHelper.TrimEnd(destinationPath))) Directory.CreateDirectory(StringHelper.TrimEnd(destinationPath));
				if (!File.Exists(destinationFile)) File.Copy(sourceFile, destinationFile);
			}

			ConsoleHelper.WriteLine(files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : "") + " mirrored", ConsoleColor.DarkYellow);
			
			return Program.EXIT_OK;
		}
	}
}
