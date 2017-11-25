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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using sar.Tools;

namespace sar.Commands
{
	/// <summary>
	/// Description of CSharpStyleRules.
	/// </summary>
	public static class HashCacheRules
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
		
		public static List<SearchResultMatch> AddHashs(string contentRoot, ref string content)
		{
			var results = new List<SearchResultMatch>();

			// style sheets - href
			// example: <link rel="stylesheet" href="/Content/Junk/app/junk.css" />
			var search = @" href\s*=\s*\""(.*?\.css)(?:\?[vh]=.*?)*\""";
			var replace = " href=" + "$1?h=**HASH**".QuoteDouble();
			results.AddRange(ProcessMatches(ref content, search, replace, contentRoot, "href"));

			// javascript - src
			// example: <script src="/example/example.js" type="text/javascript"></script>
			search = @" src\s*=\s*\""(.*?\.js)(?:\?[vh]=.*?)*\""";
			replace = " src=" + "$1?h=**HASH**".QuoteDouble();
			results.AddRange(ProcessMatches(ref content, search, replace, contentRoot, "src"));

			/*
			// ng-include
			//<div ng-include="'/example/example-table.tpl.html'"></div>
			search = @"ng-include=\""\'(.*?)(?:\?[vh]=.*)*\'\""";
			replace = "ng-include=" + "$1?h=**HASH**".QuoteSingle().QuoteDouble();
			results.AddRange(ProcessMatches(ref content, search, replace, contentRoot, "ng-include"));
			*/
			
			// template
			//templateUrl: '/example/example.tpl.html',
			search = @"\'(.*?\.tpl\.html)(?:\?[vh]=.*)*\'";
			replace = "$1?h=**HASH**".QuoteSingle();
			results.AddRange(ProcessMatches(ref content, search, replace, contentRoot, "template ulr"));
			return results;
		}
		
		private static List<SearchResultMatch> ProcessMatches(ref string content, string search, string replace, string contentRoot, string reason)
		{
			var results = new List<SearchResultMatch>();
			var matches = Regex.Matches(content, search);
			foreach (Match match in matches)
			{
				// log the replacment
				var lineNumber = IO.GetLineNumber(content, match.Index);
				results.Add(new SearchResultMatch(match, lineNumber, reason));
				
				var file = match.Groups[1].Value;
				
				if (!String.IsNullOrEmpty(file) && !file.Contains("#") && !file.Contains("{"))
				{
					// make the replacment
					var hash = GenerateHash(contentRoot, file);

					var newValue = replace.Replace("**HASH**", hash);
					newValue = Regex.Replace(match.Value, search, newValue);
					
					content = content.Replace(match.Value, newValue);
				}
			}
			
			return results;
		}
		
		private static string GenerateHash(string contentRoot, string file)
		{
			file = file.Replace(@"/", @"\");
			var filepath = contentRoot + file;
			
			if (!File.Exists(filepath))
			{
				throw new FileNotFoundException("File not found: " + filepath.QuoteDouble());
			}
			
			using (var md5 = MD5.Create())
			{
				using (var stream = File.OpenRead(filepath))
				{
					var hashBytes = md5.ComputeHash(stream);
					var hashString = "";
					
					foreach (byte bt in hashBytes)
					{
						hashString += bt.ToString("x2");
					}
					
					return hashString;
				}
			}
		}
	}
}
