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
using System.Collections.Generic;

namespace sar.Commands
{
	/// <summary>
	/// Description of VBStyleRules.
	/// </summary>
	public static class VBStyleRules
	{
		public static List<SearchResultMatch> FixShortLines(string filepath)
		{
			// fix short line continuations (less than 40 characters)
			return IO.SearchAndReplaceInFile(filepath,
											 @"[\s]*[_][\s]*[\n\r][\s]*(.{1,45}[\n\r])",
											 @" $1",
											 "short lines");
		}

		public static List<SearchResultMatch> FixLineContinuations(string filepath)
		{
			var results = new List<SearchResultMatch>();

			// fix the "_ Then" or _ Handles lines
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"[\s]*[_]{1}[\s]*[\n\r][\s]*(Then|Handles)",
													   @" $1",
													   "line continuation"));

			// fix the "_ Then" or _ Handles lines
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"[\s]*[_]{1}[\s]*[\n\r][\s]*(Then|Handles)",
													   @" $1",
													   "line continuation"));

			// fix the "_ '" lines
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"[\s]*[_]{1}[\s]*[\n\r][\s]*(')",
													   @" $1",
													   "line continuation"));

			// fix the "_ )" lines
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"[\s]*[_]{1}[\s]*[\n\r][\s]*(\))",
													   @" $1",
													   "line continuation"));

			// fix the "_ (" lines
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"[\s]*[_]{1}[\s]*[\n\r][\s]*(\()",
													   @" $1",
													   "line continuation"));

			// fix the "_ =" lines
			results.AddRange(IO.SearchAndReplaceInFile(filepath, @"[\s]*[_]{1}[\s]*[\n\r][\s]*=[\s]*",
													   @" = ",
													   "line continuation"));

			// fix the "= _ " lines
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"=[\s]*[_]{1}[\s]*[\n\r][\s]*",
													   @"= ",
													   "line continuation"));

			return results;
		}

		public static List<SearchResultMatch> FixEmptyLines(string filepath)
		{
			var results = new List<SearchResultMatch>();

			// fix the "_ Then" or _ Handles lines
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"[\s]*[_]{1}[\s]*[\n\r][\s]*(Then|Handles)",
													   @" $1",
													   "empty lines"));

			// remove empty lines after "Then"
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"Then\r*\n\s*\r*\n(\s*)(\S)",
													   "Then\r\n$1$2",
													   "line continuation"));

			// remove extra white space
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"\r*\n\s*\n(\s*)(End|Else|Next|Catch|Finally)",
													   "\r\n$1$2",
													   "line continuation"));
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"(\r*\n\s*)(Do|Case|If|Else|For|Select|Private Sub|Public Sub|Private Function|Public Function|Public Class|Try|Catch)([^\r\n]*)\r*\n\r*\n",
													   "$1$2$3\r\n",
													   "line continuation"));
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"\r*\n(\r*\n\s*)(Loop|End)",
													   "$1$2",
													   "line continuation"));

			// one empty line between methods
			//changes += IO.SearchAndReplaceInFile(file, @"(End Sub|End Function)\r*\n([^\n\r])(\S*)\s(?:(?!Class)\w)", "$1\r\n\r\n$2$3").Matches);
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"(End Sub|End Function)\r*\n(\t*)(\S\w*\s)((?!Class))",
													   "$1\r\n\r\n$2$3",
													   "line continuation"));
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"(End Sub|End Function)\r*\n\r*\n[\r\n]+(\s*)(\S)",
													   "$1\r\n\r\n$2$3",
													   "line continuation"));

			// one empty line between #Region start and first line
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"(#Region[^(\r|\n)]*)\r*\n([\t]*[\S]{1,})",
													   "$1\r\n\r\n$2",
													   "line continuation"));
			results.AddRange(IO.SearchAndReplaceInFile(filepath,
													   @"(#Region[^(\r|\n)]*)\r*\n(\t*)\r*\n\t*\r*\n",
													   "$1\r\n$2\r\n",
													   "line continuation"));

			return results;
		}
	}
}
