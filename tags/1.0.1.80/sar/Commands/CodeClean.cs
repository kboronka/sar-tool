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
	public class CodeClean : BaseCommand
	{
		public CodeClean() : base("Code - Clean",
		                             new List<string> { "code.clean", "c.clean", "c.c" },
		                             @"-code.reindent [filepath/pattern]",
		                             new List<string> { "-code.clean *.vb" })
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
			string filePattern = args[1];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			ConsoleHelper.DebugWriteLine("pattern: " + filePattern);
			ConsoleHelper.DebugWriteLine("root: " + root);
			if (files.Count == 0) throw new FileNotFoundException("unable to find any files that match pattern: \"" + filePattern + "\" in root: \"" + root + "\"");

			int counter = 0;
			int changes = 0;
			foreach (string file in files)
			{
				try
				{
					Progress.Message = "Cleaning " + IO.GetFilename(file);
					
					switch (IO.GetFileExtension(file).ToLower())
					{
						case "vb":
							counter++;
							// fix the "_ Then" lines
							changes += IO.SearchAndReplaceInFile(file, @"(\s*([_*]\s*\r{1}\n{1}\s+)Then)", @" Then");
							
							// remove empty lines after "Then"
							changes += IO.SearchAndReplaceInFile(file, @"(Then\s*\r{0,1}\n{1}\s*\r{0,1}\n{1})", "Then\r\n");
							
							// remove the xml documentation
							changes += IO.SearchAndReplaceInFile(file, @"(\r{0,1}\n{1}\s*\'{3}\s*[^\r]+)", @"");
							
							break;
						default:
							break;
					}
				}
				catch (Exception ex)
				{
					ConsoleHelper.WriteException(ex);
				}
			}
			
			
			ConsoleHelper.WriteLine(changes.ToString() + " line" + ((changes != 1) ? "s" : "") + " cleaned", ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}
