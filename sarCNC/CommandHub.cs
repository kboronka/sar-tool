using System;

using sar.Base;
using sar.CNC.Commands;

namespace sar.CNC
{
	public class CommandHub : sar.Base.CommandHub
	{
		public CommandHub() : base()
		{
			// load all command modules
			base.commandList.AddRange(new Command[] {
			                          	new Install(this),
			                          	new Uninstall(this),
			                          	new Run(this),
			                          	new Start(this),
			                          	new Stop(this)
			                          });
		}
	}
}