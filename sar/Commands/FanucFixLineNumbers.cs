using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class FanucFixLineNumbers : Command
	{
		public FanucFixLineNumbers(Base.CommandHub parent) : base(parent, "Fanuc Fix LineNumbers",
		                                                  new List<string> { "fanuc.fixlines" },
		                                                  @"-fanuc.fixlines <path>",
		                                                  new List<string> { @"-fanuc.fixlines C:\Temp" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			var path = args[1];
			
			if (path.EndsWith(@"\")) path = StringHelper.TrimEnd(path, 1);
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
			

			var files = IO.GetAllFiles(path, "_*.ls");
			for (int fileNumber = 0; fileNumber < files.Count; fileNumber++)
			{
				var file = files[fileNumber];
				var fileName = IO.GetFilename(file);
				var progress = (fileNumber / (double)files.Count) * 100;
				
				Progress.Message = "Processing " + progress.ToString("0") + "% [" + fileName + "]";
				
				if (!IO.IsSVN(file))
				{
					string[] code = IO.ReadFileLines(file);
					int lineNumber = 0;
					
					for (var i = 0; i < code.Length; i++)
					{
						string codeLine = code[i];
						if (codeLine.StartsWith(@"   1:", StringComparison.CurrentCulture))
						{
							lineNumber = 1;
						}
						else if (codeLine.StartsWith(@"/POS", StringComparison.CurrentCulture))
						{
							break;
						}
						
						if (lineNumber > 0)
						{
							string replacement = lineNumber.ToString();
							if (replacement.Length > 4) throw new ApplicationException("line number has too many characters");
							
							replacement = new String(' ', 4 - replacement.Length) + replacement + ":";
							
							code[i] = Regex.Replace(codeLine, @"^\s+[\d]*:", replacement);
							
							lineNumber++;
						}
					}
					
					IO.WriteFileLines(file, code);
				}
			}

			ConsoleHelper.WriteLine(files.Count.ToString() + " Files" + ((files.Count != 1) ? "s" : "") + " Checked", ConsoleColor.DarkYellow);
			
			return ConsoleHelper.EXIT_OK;
		}
		
	}
}

