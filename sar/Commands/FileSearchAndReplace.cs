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
using sar.Tools;
using System.Collections.Generic;
using System.IO;

namespace sar.Tools
{
	public class FileSearchAndReplace : BaseCommand
	{
		public FileSearchAndReplace() : base("File - Search And Replace",
		                                 new List<string> { "replace", "r" },
		                                 "-replace <file_search_pattern> <search_text> <replace_text>",
		                                 new List<string> {
		                                 	"-r \"AssemblyInfo.cs\" \"0.0.0.0\" \"1.0.0.0\"",
		                                 	"-r AssemblyInfo.* ((Version)\\(\\\"\\d+\\.\\d+\\.\\d+\\.\\d+\\\"\\)) \"Version(\\\"%VERSION%\\\")\"",
		                                 	"-r \\sar\\\"AssemblyInfo.cs\" \"0.0.0.0\" \"1.0.0.0\"",
		                                 })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 4)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string search = args[2];
			string replace = args[3];

			string filePattern = args[1];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);

			int counter = 0;
			int changes = 0;
			foreach (string file in files)
			{
				if (Program.IncludeSVN || !IO.IsSVN(file))
				{
					int found = IO.SearchAndReplaceInFile(file, search, replace);
					if (found > 0)
					{
						changes += found;
						counter++;
					}
				}
			}			

			ConsoleHelper.WriteLine(changes.ToString() + " replacment" + ((changes != 1) ? "s" : "") + " made in " + counter.ToString() + " file" + ((counter != 1) ? "s" : ""), ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}
