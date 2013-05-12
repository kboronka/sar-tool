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
							// fix the "_ Then" or _ Handles lines
							changes += IO.SearchAndReplaceInFile(file, @"[\s]*[_]{1}[\s]*[\n\r][\s]*(Then|Handles)", @" $1");
							
							// remove empty lines after "Then"
							changes += IO.SearchAndReplaceInFile(file, @"Then[\s]*[\n\r]{1,2}[\s]*[\n\r]+(\s*)(\S)", "Then\r\n$1$2");
							
							// remove the xml documentation
							changes += IO.SearchAndReplaceInFile(file, @"[\n\r][\s]*[\']{3}[^\n\r]*", @"");

							// remove extra white space
							changes += IO.SearchAndReplaceInFile(file, @"\r*\n\r*\n(\s*)(End|Else|Next|Catch|Finally)", "\r\n$1$2");	
							changes += IO.SearchAndReplaceInFile(file, @"(\r*\n\s*)(Do|Case|If|Else|For|Select|Private Sub|Public Sub|Public Class|Try|Catch)([^\r\n]*)\r*\n\r*\n", "$1$2$3\r\n");
													
							
							// add once space between methods
							changes += IO.SearchAndReplaceInFile(file, @"(End Sub|End Function|End Region)[\n\r]{1,2}[\n\r]*(\s*)(\S)", "$1\r\n\r\n$2$3");
								
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
