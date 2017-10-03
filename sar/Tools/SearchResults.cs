using System;
using System.Collections.Generic;

namespace sar.Tools
{
	/// <summary>
	/// Description of SearchResults.
	/// </summary>	
	public class SearchResults
	{
		public string FilePath { get; private set; }
		public List<SearchResultMatch> Matches { get; private set; }
		
		public SearchResults(string filepath)
		{
			this.FilePath = filepath;
			this.Matches = new List<SearchResultMatch>();
		}
	}
}
