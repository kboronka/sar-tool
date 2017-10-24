using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Reflection;

using sar.Tools;

namespace sar.CNC.Commands
{
	public class Install : sar.Base.Command
	{
		public Install(sar.Base.CommandHub parent) : base(parent, "Install",
		                                                  new List<string> { "install", "i" },
		                                                  @"-i",
		                                                  new List<string> { "-i" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Installing Service";

			var installArgs = new List<string>();
			installArgs.Add(@"/LogToConsole = false");
			installArgs.Add(Assembly.GetExecutingAssembly().Location);
			
			ManagedInstallerClass.InstallHelper(installArgs.ToArray());
			
			ConsoleHelper.WriteLine("");
			ConsoleHelper.WriteLine("Install Complete", ConsoleColor.Yellow);

			return ConsoleHelper.EXIT_OK;
		}
	}
}
