
using System;
using System.Text.RegularExpressions;

namespace sar.Tools
{
	public static class ExceptionHelper
	{
		public static string GetStackTrace(this Exception ex)
		{
			try
			{
				if (ex.StackTrace == null) return "[StackTrace not available]";

				string result = "";
				string stackTrace = ex.StackTrace;
				string regex = @"(\s*)at\s((.?)*)\sin\s((.?)*):line\s([1-9]*)";
				
				
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
	}
}
