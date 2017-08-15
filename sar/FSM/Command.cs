using System;

namespace sar.FSM
{
	/// <summary>
	/// Description of Command.
	/// </summary>
	public class Command
	{
		public Enum CommandSignal { get; private set; }
		public Object Parameters { get; private set; }
		
		public Command(Enum command, Object parameters)
		{
			this.CommandSignal = command;
			this.Parameters = parameters;
		}
		
		public Command(Enum command)
			: this(command, null)
		{
			
		}
	}
}
