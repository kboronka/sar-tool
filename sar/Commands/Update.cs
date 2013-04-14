
using System;
using System.Collections.Generic;
using System.IO;

using skylib.Tools;

namespace skylib.sar
{
	public class Update : BaseCommand
	{
		public Update(): base("Check for sar updates",
		                            new List<string> { "update" },
		                            "-update",
		                            new List<string> { "-update" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			WebHelper.Download(@"http://sar-tool.googlecode.com/svn/trunk/release/sar.exe", IO.Temp + "sar.exe");
			WebHelper.ReadURL(@"http://sar-tool.googlecode.com/svn/trunk/release/license.txt");
			
			return Program.EXIT_OK;
		}
	}
}
