using System;
using System.Collections.Generic;

using sar.Tools;

namespace sar.CNC.Commands
{
	public class Start : sar.Base.Command
	{
		public Start(sar.Base.CommandHub parent) : base(parent, "Start",
		                                                new List<string> { "start" },
		                                                @"-start",
		                                                new List<string> { "-start" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Starting Service";
			bool success = ServiceHelper.TryStart("sar.CNC.exe");
			
			if (success)
			{
				ConsoleHelper.WriteLine("Service Started", ConsoleColor.Yellow);
				return ConsoleHelper.EXIT_OK;
			}
			else
			{
				ConsoleHelper.WriteLine("Service Failed to Start", ConsoleColor.Red);
				return ConsoleHelper.EXIT_ERROR;
			}
		}
	}
}
