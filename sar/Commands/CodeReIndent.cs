
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
						
						/*
						ignore: "Private", "Public", "Shared", "Declare"
						
						indent:
							startwith: "Class", "Function", "Enum", "Sub", "Select Case", "Case", "Do While", "Try", "Catch"
							endswith: "_", "Then", "Else"

						outdent:
							start with "Loop", "End If", "End Try", "End Select", "End Sub", "End Function", "End Enum"
						 */
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
