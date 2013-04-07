
using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class NetListAddaptors : BaseCommand
	{
		public NetListAddaptors() : base("Network - List Addaptors",
		                                 new List<string> { "net.list", "n.l" },
		                                 @"-net.list",
		                                 new List<string> { "-net.list" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			NetHelper.Addaptors();
			
			return Program.EXIT_OK;
		}
	}
}
