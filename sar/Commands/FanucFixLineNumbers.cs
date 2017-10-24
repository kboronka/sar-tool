/* Copyright (C) 2017 Kevin Boronka
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using sar.Base;
using sar.Tools;

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

			var files = IO.GetAllFiles(path, "*.ls").Where(f => !f.ToLower().Contains("logbook.ls")).ToList();
			
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
					
					string text = IO.LinesToString(code);
					
					if (IO.ReadFile(file) != text)
					{
						IO.WriteFile(file, text, Encoding.ASCII);
					}
				}
			}

			ConsoleHelper.WriteLine(files.Count.ToString() + " Files" + ((files.Count != 1) ? "s" : "") + " Checked", ConsoleColor.DarkYellow);
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}

