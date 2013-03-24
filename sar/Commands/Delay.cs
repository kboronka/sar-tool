using System;
using skylib.Tools;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace skylib.sar
{
	public class Delay : BaseCommand
	{
		public Delay() : base("Delay",
		                       new List<string> { "delay" },
		                       @"-delay <milliseconds>",
		                       new List<string> { "-delay 5000" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			int delay;
			if (!Int32.TryParse(args[1], out delay) || delay < 0 || delay > Int32.MaxValue)
			{
				throw new ArgumentException("invalid delay value");
			}
			
			Thread.Sleep(delay);
			
			return Program.EXIT_OK;
		}
	}
}
