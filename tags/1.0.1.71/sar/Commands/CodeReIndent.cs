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
	public class CodeReIndent : BaseCommand
	{
		public CodeReIndent() : base("Code - ReIndent",
		                             new List<string> { "code.reindent", "c.reindent", "c.r" },
		                             @"-code.reindent [filepath/pattern]",
		                             new List<string> { "-code.reindent *.vb" })
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
			foreach (string file in files)
			{
				List<string> indent = new List<string>();
				
				switch (IO.GetFileExtension(file).ToLower())
				{
					case "vb":
						counter++;
						string[] lines = IO.ReadFileLines(file);
						List<string> newlines = new List<string>();
						int level = 0;
						bool linecontinue = false;
						
						foreach (string line in lines)
						{
							string newline = StringHelper.TrimWhiteSpace(line);
							
							string temp = StringHelper.Remove(line, new List<string> { "Private", "Public", "Shared" });
							if (temp.Contains("'")) temp = temp.Substring(0, temp.IndexOf('\''));
							temp = StringHelper.TrimWhiteSpace(temp);
							
							string firstword = StringHelper.FirstWord(temp);
							
							if (!string.IsNullOrEmpty(firstword) && firstword[0] != '\'')
							{
								// single level
								if (StringHelper.StartsWith(temp, new List<string>() { "Loop", "End", "End If", "Catch", "End Try", "End Select", "End Sub", "End Function", "End Enum", "Case" }) ||
								    (temp.StartsWith("ElseIf") && temp.EndsWith("Then")) ||
								    (temp.StartsWith("Else") && !temp.StartsWith("ElseIf"))
								   )
								{
									level--;
								}
								
								// double level
								if (StringHelper.StartsWith(temp, new List<string>() { "End Select" }))
								{
									level--;
								}
								
								if (level < 0) level = 0;
								newlines.Add( new String('\t', level + (linecontinue ? 1 : 0)) + newline);
								
								string lastword = StringHelper.LastWord(temp);
								if (StringHelper.EndsWith(temp, new List<string>() { "Then", "Else" }) ||
								    StringHelper.StartsWith(temp, new List<string>() { "Class", "Function", "Enum", "Sub", "Select Case", "Case", "Do While", "While", "Try", "Catch" }))
								{
									level++;
								}
								
								// double level
								if ( StringHelper.StartsWith(temp, new List<string>() { "Select Case" }))
								{
									level++;
								}
								
								linecontinue = StringHelper.EndsWith(temp, new List<string>() { "_" } );
							}
							else
							{
								if (newline.StartsWith("'"))
								{
									if (level < 0) level = 0;
									newlines.Add(new String('\t', level) + newline);
								}
								else
								{
									newlines.Add(newline);
								}
							}
						}
						
						IO.WriteFileLines(file, newlines);

						break;
					default:
						break;
				}
			}
			
			ConsoleHelper.WriteLine(counter.ToString() + " File" + ((counter != 1) ? "s" : "") + " Checked", ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}
