/* Copyright (C) 2021 Kevin Boronka
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

using sar.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace sar.Commands
{
	/// <summary>
	/// Description of CSharpStyleRules.
	/// </summary>
	public static class CSStyleRules
	{
		private static long placholderID;

		public static void Save(SearchResults results, string content)
		{
			if (results.Matches.Count > 0)
			{
				// make file not readonly
				File.SetAttributes(results.FilePath, FileAttributes.Normal);

				// write new content back to file
				using (var writer = new StreamWriter(results.FilePath, false, Encoding.UTF8))
				{
					writer.Write(content);
				}
			}
		}

		private static string GeneratePlaceholder()
		{
			return "PLACEHOLDER" + (placholderID++).ToString("0000000");
		}

		public static Dictionary<string, string> RemoveStrings(ref string content)
		{
			var strings = new Dictionary<string, string>();
			var temp = new List<SearchResultMatch>();
			var placeholder = "";

			// remove literal double quotes
			placeholder = GeneratePlaceholder();
			if (IO.SearchAndReplace(ref content, @"""""", placeholder, "").Count > 0)
			{
				strings.Add(placeholder, @"""""");
			}

			// remove escaped double quotes
			placeholder = GeneratePlaceholder();
			if (IO.SearchAndReplace(ref content, @"\\""", placeholder, "").Count > 0)
			{
				strings.Add(placeholder, @"\""");
			}

			// remove double quote strings
			var matches = Regex.Matches(content, @""".*?""");
			var foundStrings = new List<string>();
			foreach (Match match in matches)
			{
				if (!foundStrings.Contains(match.Value))
				{
					foundStrings.Add(match.Value);
					placeholder = GeneratePlaceholder();
					content = content.Replace(match.Value, placeholder);
					strings.Add(placeholder, match.Value);
				}
			}

			// remove single quote strings
			matches = Regex.Matches(content, @"'.'");
			foundStrings = new List<string>();
			foreach (Match match in matches)
			{
				if (!foundStrings.Contains(match.Value))
				{
					foundStrings.Add(match.Value);
					placeholder = GeneratePlaceholder();
					content = content.Replace(match.Value, placeholder);
					strings.Add(placeholder, match.Value);
				}
			}

			// remove single line comments
			matches = Regex.Matches(content, @"\/\/.*");
			foundStrings = new List<string>();
			foreach (Match match in matches)
			{
				if (!foundStrings.Contains(match.Value))
				{
					foundStrings.Add(match.Value);
					placeholder = @"// " + GeneratePlaceholder();
					content = content.Replace(match.Value, placeholder);
					strings.Add(placeholder, match.Value);
				}
			}

			// remove multi-line comments
			matches = Regex.Matches(content, @"\/\*(?:.|\n)*(?:\s*\*\/)");
			foundStrings = new List<string>();
			foreach (Match match in matches)
			{
				if (!foundStrings.Contains(match.Value))
				{
					foundStrings.Add(match.Value);
					placeholder = @"// " + GeneratePlaceholder();
					content = content.Replace(match.Value, placeholder);
					strings.Add(placeholder, match.Value);
				}
			}

			return strings;
		}

		public static void RevertStrings(ref string content, Dictionary<string, string> placeholders)
		{
			foreach (var key in placeholders.Keys.Reverse())
			{
				content = content.Replace(key, placeholders[key]);
			}
		}

		public static List<SearchResultMatch> FixSemicolon(ref string content)
		{
			var results = new List<SearchResultMatch>();

			// remove empty characters after semicolons
			results.AddRange(IO.SearchAndReplace(ref content,
												 @";[ \t]+\r\n",
												 ";\r\n",
												 "removed empty characters after semicolon"));

			return results;
		}

		public static List<SearchResultMatch> FixSwitchStatements(ref string content)
		{
			var results = new List<SearchResultMatch>();

			// two empty lines between cases statements
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"break;\r\n\s*\r\n([ \t]+)case",
												 "break;\r\n$1case",
												 "removed empty lines before case"));
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"break;\r\n\s*\r\n([ \t]+)default",
												 "break;\r\n$1default",
												 "removed empty lines before default case"));

			return results;
		}

		public static List<SearchResultMatch> FixEmptyLines(ref string content)
		{
			var results = new List<SearchResultMatch>();

			// no empty lines between case statements
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"\r\n\s*\r\n([ \t]+)case",
												 "\r\n$1case",
												 "removed empty lines before case"));
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"\r\n\s*\r\n([ \t]+)default",
												 "\r\n$1default",
												 "removed empty lines before default case"));

			var emptylines = new List<SearchResultMatch>();
			do
			{
				emptylines = IO.SearchAndReplace(ref content,
												 @"\r\n[ \t]*\r\n([ \t]*)\r\n",
												 "\r\n$1\r\n",
												 "removed double empty lines");

				// remove all double empty lines
				results.AddRange(emptylines);
			} while (emptylines.Count > 0);

			// add empty line after #region
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"(#region.*)\r\n([ \t]*)((?![ \t])(?!\r\n))",
												 "$1\r\n\r\n$2$3",
												 "added empty line after #region"));
			// add empty line after #endregion
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"(#endregion)\r\n([ \t]*)((?![ \t])(?!\r\n))",
												 "$1\r\n\r\n$2$3",
												 "added empty line after #endregion"));

			// remove empty line prior to set, catch, finally or else
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"}\r\n[ \t]*\r\n([ \t]*(?:(?:set)|(?:private set)|(?:protected set)|(?:catch)|(?:remove)|(?:finally)|(?:else)|(?:else if \(.*\)))\r\n*[ \t]*{)",
												 "}\r\n$1",
												 "removed empty line"));
			return results;
		}

		public static List<SearchResultMatch> FixBraces(ref string content)
		{
			var results = new List<SearchResultMatch>();

			// remove whitespace after closing brace
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"}[ \t]+\r\n",
												 "}\r\n",
												 "removed empty characters after closing brace"));

			// remove empty lines before a closing brace
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"([^(?:#endregion)|^(?:{)])\r\n[ \t]*\r\n([ \t]*)}",
												 "$1\r\n$2}",
												 "removed empty lines before closing brace"));

			// add empty line after a closing brace
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"}\r\n(\t*)((?![ \t])(?!\r\n)(?!})(?!set)(?!private set)(?!protected set)(?!remove)(?!catch)(?!finally)(?!else)(?!public .* .* {).{2})",
												 "}\r\n\r\n$1$2",
												 "added empty line after closing brace"));

			return results;
		}

		public static List<SearchResultMatch> FixBrackets(ref string content)
		{
			var results = new List<SearchResultMatch>();

			// remove empty lines before a closing backet
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"\r\n[ \t]*\r\n([ \t]*)\)",
												 "\r\n$1)",
												 "removed empty lines before closing bracket"));
			// remove space after '('
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"\([ \t]([^ ^\r^\n]{1})",
												 "($1",
												 "remove space after '('"));
			// remove whitespace before ')'
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"[ |\t|\r|\n]+\)",
												 ")",
												 "remove whitespace before ')'"));
			// remove whitespace before '('
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"(?<!if)(?<!catch)(?<!for)(?<!foreach)(?<!while)(?<!lock)(?<!using)(?<!return)(?<!switch)(?<!case)(?<![\:\?])(?<![\&\|])(?<!\[\=\,\+\-\*\/\>\<])(?<=\w)[ \t]+\(",
												 "(",
												 "remove whitespace before '('"));
			return results;
		}

		public static List<SearchResultMatch> FixSpaces(ref string content)
		{
			var results = new List<SearchResultMatch>();

			// add a space after comma
			results.AddRange(IO.SearchAndReplace(ref content,
												 @",([^ |^\r\n|^\t]{1})",
												 ", $1",
												 "added space after ','"));
			// remove extra spaces after comma (exception: it's a line continuation, and there are comments on the same line
			results.AddRange(IO.SearchAndReplace(ref content,
												 @",[ \t]{2,}(?![ \t])(?!\/\/)",
												 ", ",
												 "remove extra spaces after ','"));
			// remove extra spaces after comma (exception: it's a line continuation, and there are comments on the same line
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"[ \t]+,",
												 ",",
												 "remove extra spaces before ','"));

			// add a space before =
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"([^ ^\+^\!^=^\*^\-^\&^>^<^\|]{1})=",
												 "$1 =",
												 "added space before '='"));
			// add a space after =
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"(?<!operator .)=([^ ^=^>^\r^\n]{1})",
												 "= $1",
												 "added space after '='"));
			// remove extra spaces after =
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"=[ \t]{2,}",
												 "= ",
												 "remove extra spaces after '='"));

			// remove extra spaces after math opeators
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"([\+\-\*\/\>\<])[ \t]{2,}(?![ \t])(?!\/\/)",
												 "$1 ",
												 "remove extra spaces after math opeators"));
			// remove extra spaces before math opeators
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"(?<=\S)[ \t]{2,}([\,\+\-\*\/\>\<])(?!\/)",
												 " $1",
												 "remove extra spaces before math opeators"));

			// add extra space before equality opeator
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"([^ \<\>]{1})((?:==)|(?:\>=)|(?:\<=)|(?:\!=))",
												 "$1 $2",
												 "add extra space before equality opeator"));

			// remove extra space before equality opeator
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"([ \t]{2,})((?:==)|(?:\>=)|(?:\<=)|(?:\!=))",
												 " $2",
												 "remove extra space before equality opeator"));
			// remove extra space after equality opeator
			results.AddRange(IO.SearchAndReplace(ref content,
												 @"((?:==)|(?:\>=)|(?:\<=)|(?:\!=))([ \t]{2,})",
												 "$1 ",
												 "remove extra space after equality opeator"));
			// this doesn't work
			//			// add space after math operator
			//			results.AddRange(IO.SearchAndReplace(ref content,
			//			                                     @"([\+\-\*\/])(?:[ \t]{0})(?![ \t])(?!\/\/)(?![\+\-\*\/\=\)\n\r])",
			//			                                     "$1 ",
			//			                                     "add space after math operator"));

			return results;
		}

		public static List<SearchResultMatch> SortUsingDirectives(ref string content)
		{
			var results = new List<SearchResultMatch>();

			// get all using directives
			var matches = Regex.Matches(content, @"^using (.*);\r\n", RegexOptions.Multiline);

			if (matches.Count > 1)
			{
				var firstLine = IO.GetLineNumber(content, matches[0].Index);
				var lastLine = IO.GetLineNumber(content, matches[matches.Count - 1].Index);

				var beforeLength = matches[matches.Count - 1].Index - matches[0].Index + matches[matches.Count - 1].Value.Length;
				var before = content.Substring(matches[0].Index, beforeLength);

				var namespaces = new Dictionary<string, Match>();
				foreach (Match match in matches)
				{
					var name = match.Groups[1].Value;
					if (!namespaces.ContainsKey(name))
					{
						namespaces.Add(name, match);
					}
				}

				// get all toplevel namespaces
				var topLevelNamespaces = new List<string>();
				foreach (var name in namespaces.Keys)
				{
					var topLevel = name.Split('.')[0];
					if (!topLevelNamespaces.Contains(topLevel))
					{
						topLevelNamespaces.Add(topLevel);
					}
				}

				topLevelNamespaces.Sort();
				if (topLevelNamespaces.Contains("System"))
				{
					topLevelNamespaces.RemoveAll(t => t == "System");
					topLevelNamespaces.Insert(0, "System");
				}

				// re-generate list of using directives
				var after = "";
				for (var i = 0; i < topLevelNamespaces.Count; i++)
				{
					foreach (var name in namespaces.Where(n => n.Key.StartsWith(topLevelNamespaces[i])).OrderBy(n => n.Key))
					{
						after += "using " + name.Key + ";" + Environment.NewLine;
					}

					if (i != topLevelNamespaces.Count - 1)
					{
						after += Environment.NewLine;
					}
				}

				if (after != before)
				{
					var lineNumber = IO.GetLineNumber(content, matches[0].Index);
					results.Add(new SearchResultMatch(matches[0], lineNumber, "order or spacing of using directives"));

					content = content.Replace(before, after);
				}
			}

			return results;
		}
	}
}
