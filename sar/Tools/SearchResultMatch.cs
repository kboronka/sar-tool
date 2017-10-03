using System;
using System.Text.RegularExpressions;

namespace sar.Tools
{
	/// <summary>
	/// Description of SearchResultMatch.
	/// </summary>
	public class SearchResultMatch
	{
		public int LineNumbrer { get; private set; }
		public Match Match { get; private set; }
		
		public SearchResultMatch(Match match, int lineNumber)
		{
			this.Match = match;
			this.LineNumbrer = lineNumber;
		}
	}
}
