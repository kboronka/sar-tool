using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using sar.Tools;

namespace sar.Commands
{
	/// <summary>
	/// Description of CSharpStyleRules.
	/// </summary>
	public static class CSStyleRules
	{
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
		
		private static long placholderID;
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
			
			// remove comment strings
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
			// remove all double empty lines
			results.AddRange(IO.SearchAndReplace(ref content,
			                                     @"\r\n[ \t]*\r\n([ \t]*)\r\n",
			                                     "\r\n$1\r\n",
			                                     "removed double empty lines"));

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
			
			// TODO: remove empty lines between get {} and set {}
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
			                                     @"}\r\n(\t*)((?![ \t])(?!\r\n)(?!})(?!set)(?!catch)(?!finally)(?!else)(?!public .* .* {).{2})",
			                                     "}\r\n\r\n$1$2",
			                                     "added empty line after closing brace"));
			
			// remove empty line prior to set, catch, finally or else
			results.AddRange(IO.SearchAndReplace(ref content,
			                                     @"}\r\n[ \t]*\r\n([ \t]*(?:(?:set)|(?:catch)|(?:finally)|(?:else)|(?:else if \(.*\)))\r\n*[ \t]*{)",
			                                     "}\r\n$1",
			                                     "removed empty line"));
			
			
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
			
			return results;
		}
	}
}
