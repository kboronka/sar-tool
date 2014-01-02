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
				try
				{
					Progress.Message = "ReIndenting " + IO.GetFilename(file);
					
					switch (IO.GetFileExtension(file).ToLower())
					{
						case "vb":
							counter++;
							string[] lines = IO.ReadFileLines(file);
							List<string> newlines = new List<string>();
							int level = 0;
							bool linecontinue = false;
							//bool levelup = false;
							bool meta = false;
							bool metaContinue = false;
							int linenumber = 0;
							int lastIndentLevel = 0;

							foreach (string line in lines)
							{
								linenumber++;
								string newline = StringHelper.TrimWhiteSpace(line);
								
								// clean string
								string temp = StringHelper.Remove(line, new List<string> { "Private", "Protected", "Public", "Shared", "Overridable", "Overrides", "Overloads", "Friend", "ReadOnly", "WriteOnly", "Partial", "Shadows", "Default", "NotInheritable" });
								temp = StringHelper.TrimWhiteSpace(temp);
								
								// trim meta tags
								meta = false;
								if (temp.StartsWith("<"))
								{
									if (temp.Contains(">"))
									{
										meta = true;
										temp = temp.Substring(temp.IndexOf('>') + 1);
									}
									else if (temp.EndsWith("_"))
									{
										temp = temp.Substring(temp.IndexOf('_'));
										metaContinue = false;
									}
									else
									{
										ConsoleHelper.DebugWriteLine("invalid meta tag in code : " + file);
									}
								}
								else if (metaContinue)
								{
									if (temp.Contains(">"))
									{
										meta = true;
										metaContinue = false;
										temp = temp.Substring(temp.IndexOf('>') + 1);
									}
									else
									{
										temp = "";
									}
								}
								
								// trim comments
								if (temp.Contains("'"))
								{
									temp = temp.Substring(0, temp.IndexOf('\''));
								}
								
								temp = StringHelper.TrimWhiteSpace(temp);
								
								string firstword = StringHelper.FirstWord(temp);
								string lastword = StringHelper.LastWord(temp);
								
								if (!string.IsNullOrEmpty(firstword) && firstword[0] != '\'')
								{
									// ******************** Level Down Before Print *************************** //
									// single level
									if (StringHelper.StartsWith(temp, new List<string>() { "Loop", "Next", "End", "End If", "ElseIf", "#End If", "#ElseIf", "#Else", "Catch", "Finally", "End Try", "End Select", "End Sub", "End Function", "End Enum", "Case" }) ||
									    (firstword == "Else" && !temp.StartsWith("Else :"))
									   )
									{
										level--;
									}
									
									// double level
									if (StringHelper.StartsWith(temp, new List<string>() { "End Select" }))
									{
										level--;
									}
									
									// ******************** correction for line continuation *************************** //
									int correction = level;
									//correction -= ((linecontinue) ? 1 : 0);
									//correction -= ((linecontinue & !levelup) ? 1 : 0);
									if (level + correction < 0)
									{
										if (temp != "#End Region")
										{
											ConsoleHelper.DebugWriteLine("invalid vb code : " + IO.GetFilename(file));
											//ConsoleHelper.DebugWriteLine("file: " + file);
											ConsoleHelper.DebugWriteLine("temp: " + temp);
											ConsoleHelper.DebugWriteLine("line: " + linenumber.ToString());
										}
									}
									
									// ******************** Print Line *************************** //
									lastIndentLevel = correction;
									if (correction < 0) correction = 0;
									newlines.Add(new String('\t', correction) + (linecontinue ? new String(' ', 2) : "") + newline);
									
									linecontinue = StringHelper.EndsWith(temp, new List<string>() { "_" } ) & !meta;
									
									// ******************** Level Up after line *************************** //
									if (StringHelper.EndsWith(temp, new List<string>() { "Then", "Else", "#ElseIf", "#Else" }) ||
									    StringHelper.StartsWith(temp, new List<string>() { "Namespace", "Class", "Structure", "Function", "Property", "Operator", "Enum", "Sub", "Module", "SyncLock", "Select Case", "Case", "For", "For Each", "Do", "Do While", "While", "Try", "Catch", "Finally", "With", "Custom Event" }) ||
									    temp.StartsWith("AddHandler(") ||  temp.StartsWith("RemoveHandler(") ||  temp.StartsWith("RaiseEvent(") ||
									    (firstword == "Get") || (firstword == "Set"))
									{
										level++;
										//levelup = true;
									}
									else
									{
										//if (!linecontinue) levelup = false;
									}
									
									// double level
									if (StringHelper.StartsWith(temp, new List<string>() { "Select Case" }))
									{
										level++;
									}
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
							
							if (lastIndentLevel != 0)
							{
								IO.WriteFileLines(file + ".error", newlines);
								throw new Exception("failed to indent " + IO.GetFilename(file));
							}
							
							IO.WriteFileLines(file, newlines);

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
			
			
			ConsoleHelper.WriteLine(counter.ToString() + " File" + ((counter != 1) ? "s" : "") + " Checked", ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}
