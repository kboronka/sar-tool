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

using sar.Base;
using sar.Tools;
using System;
using System.Collections.Generic;
using System.IO;

namespace sar.Commands
{
	public class HashCache : Command
	{
		public HashCache(Base.CommandHub parent)
			: base(parent, "Hash Cache",
				   new List<string> {
					   "hash.cache",
					   "hc"
				   },
				   "hc <root_path> <file_search_pattern>",
				   new List<string> {
					   @"ch .\Content .\Views"
				   })
		{

		}

		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}

			Progress.Message = "Searching";
			string contentRoot = args[1];
			string searchRoot = args[2];

			if (!Directory.Exists(searchRoot))
			{
				throw new DirectoryNotFoundException("search directory: " + searchRoot.QuoteDouble() +
													 " does not exists");
			}

			if (!Directory.Exists(contentRoot))
			{
				throw new DirectoryNotFoundException("content directory: " + searchRoot.QuoteDouble() +
													 " does not exists");
			}

			List<string> files = IO.GetAllFiles(searchRoot, "*.*");
			if (files.Count == 0)
			{
				throw new FileNotFoundException("unable to find any files in search directory: " +
												searchRoot.QuoteDouble());
			}

			var fileChangeResults = new List<SearchResults>();
			foreach (string file in files)
			{
				try
				{
					Progress.Message = "Checking " + IO.GetFilename(file);
					var changes = new SearchResults(file);
					var type = IO.GetFileExtension(file).ToLower();
					string content = IO.ReadFileAsUtf8(file);
					var original = content;

					if (type == "aspx" || type == "ascx" || type == "html" || type == "js" || type == "master")
					{
						changes.AddResults(HashCacheRules.AddHashs(contentRoot, ref content));
					}

					if (content != original)
					{
						fileChangeResults.Add(changes);
						HashCacheRules.Save(changes, content);
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
