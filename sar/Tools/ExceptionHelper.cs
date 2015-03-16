/* Copyright (C) 2015 Kevin Boronka
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
using System.Text.RegularExpressions;

namespace sar.Tools
{
	public static class ExceptionHelper
	{
		public static string GetStackTrace(Exception ex)
		{
			try
			{
				if (ex.StackTrace == null) return "[StackTrace not available]";

				string result = "";
				string stackTrace = ex.StackTrace;
				string regex = @"(\s*)at\s((.?)*)\sin\s((.?)*):line\s(\d*)";
				
				
				string[] lines = stackTrace.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
				
				foreach (string line in lines)
				{
					MatchCollection regexResults = Regex.Matches(line, regex);
					if (regexResults.Count == 1 && regexResults[0].Groups.Count == 7)
					{
						if (!String.IsNullOrEmpty(result)) result += Environment.NewLine;
						string method = regexResults[0].Groups[2].Value;
						string filepath = regexResults[0].Groups[4].Value;
						string lineNumber = regexResults[0].Groups[6].Value;

						string filename = IO.GetFilename(filepath);
						
						if (method.Contains("."))
						{
							method = method.Substring(method.LastIndexOf('.') + 1);
						}
						
						if (method.Contains("("))
						{
							method = method.Substring(0, method.LastIndexOf('('));
						}
						
						result += "\t" + method + "() in " + filename + ":line " + lineNumber;
					}
				}
				
				return result;
			}
			catch
			{
				return "[StackTrace not available]";
			}
		}

		public static Exception GetInner(Exception ex)
		{
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			
			return ex;
		}
	}
}
