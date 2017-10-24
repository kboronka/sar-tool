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

using sar.Base;
using sar.Tools;

namespace sar.Commands
{
	public class CodeClean : Command
	{
		public CodeClean(Base.CommandHub parent) : base(parent, "Code - Clean",
		                                                new List<string> { "code.clean", "c.clean", "c.c" },
		                                                @"-code.code path",
		                                                new List<string> { @"-code.clean *c:\code\" })
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
			string root = args[1];
			if (!Directory.Exists(root))
			{
				throw new DirectoryNotFoundException("directory: " + root.QuoteDouble() +
				                                     " does not exists");
			}
			
			List<string> files = IO.GetAllFiles(root, "*.*");
			if (files.Count == 0)
			{
				throw new FileNotFoundException("unable to find any files in root: " +
				                                root.QuoteDouble());
			}

			var fileChangeResults = new List<SearchResults>();
			foreach (string file in files)
			{
				try
				{
					Progress.Message = "Cleaning " + IO.GetFilename(file);
					var changes = new SearchResults(file);
					
					switch (IO.GetFileExtension(file).ToLower())
					{
						case "vb":
							changes.AddResults(VBStyleRules.FixShortLines(file));
							changes.AddResults(VBStyleRules.FixLineContinuations(file));
							changes.AddResults(VBStyleRules.FixEmptyLines(file));
							fileChangeResults.Add(changes);
							break;
						case "cs":
							// skip automatically generated source
							if (file.Contains(".Designer.cs") || file.Contains("AssemblyInfo.cs"))
							{
								continue;
							}
							
							string content = IO.ReadFileAsUtf8(file);
							var strings = CSStyleRules.RemoveStrings(ref content);
							changes.AddResults(CSStyleRules.SortUsingDirectives(ref content));
							changes.AddResults(CSStyleRules.FixSemicolon(ref content));
							changes.AddResults(CSStyleRules.FixSwitchStatements(ref content));
							changes.AddResults(CSStyleRules.FixBraces(ref content));
							changes.AddResults(CSStyleRules.FixBrackets(ref content));
							changes.AddResults(CSStyleRules.FixEmptyLines(ref content));
							changes.AddResults(CSStyleRules.FixSpaces(ref content));
							fileChangeResults.Add(changes);
							CSStyleRules.RevertStrings(ref content, strings);
							CSStyleRules.Save(changes, content);
							break;
						default:
							break;
					}
					
					if (changes.Matches.Count > 0)
					{
						ConsoleHelper.WriteLine("");
						ConsoleHelper.WriteLine(IO.GetFilename(file), ConsoleColor.Yellow);
						
						foreach (var change in changes.Matches)
						{
							ConsoleHelper.Write("  +line ");
							ConsoleHelper.Write(change.LineNumbrer.ToString(), ConsoleColor.White);
							ConsoleHelper.WriteLine(": " + change.Reason);
						}
					}
				}
				catch (Exception ex)
				{
					ConsoleHelper.WriteException(ex);
				}
			}
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}
