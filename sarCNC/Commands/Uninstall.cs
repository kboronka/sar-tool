using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Reflection;

using sar.Tools;

namespace sar.CNC.Commands
{
	public class Uninstall : sar.Base.Command
	{
		public Uninstall(sar.Base.CommandHub parent) : base(parent, "Uninstall",
		                                                    new List<string> { "uninstall", "u" },
		                                                    @"-u",
		                                                    new List<string> { "-u" })
		{
		}

		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Uninstalling Service";
			
			var installArgs = new List<string>();
			installArgs.Add(@"/u");
			installArgs.Add(@"/LogToConsole = false");
			installArgs.Add(Assembly.GetExecutingAssembly().Location);
			
			ManagedInstallerClass.InstallHelper(installArgs.ToArray());
			
			ConsoleHelper.WriteLine("");
			ConsoleHelper.WriteLine("Uninstall Complete", ConsoleColor.Yellow);

			return ConsoleHelper.EXIT_OK;
		}
	}
}
