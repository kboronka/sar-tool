using System;
using System.Collections.Generic;
using System.Threading;

using sar.Tools;
using Base = sar.Base;

namespace sar.CNC.Commands
{
	public class Run : sar.Base.Command
	{
		public Run(sar.Base.CommandHub parent) : base(parent, "Run",
		                                              new List<string> { "run", "r" },
		                                              @"-r",
		                                              new List<string> { "-r" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Program.Log("Engine Running in Console Mode");
			Progress.Message = "Engine Running in Console Mode";
			var thread = new Thread(Service.StartServices);
			thread.Start();
			
			while (true)
			{
				Thread.Sleep(10000);
			}
		}
	}
}
