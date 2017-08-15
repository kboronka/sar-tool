using System;
using System.Collections.Generic;

namespace sar.FSM
{
	/// <summary>
	/// Description of CommandQueue.
	/// </summary>
	public class CommandQueue
	{
		private readonly List<Command> commands;
		
		public bool Available { get { return commands.Count > 0; } }
		
		public CommandQueue()
		{
			commands = new List<Command>();
		}
		
		public void QueueCommand(Enum command)
		{
			commands.Add(new Command(command));
		}
		
		public void QueueCommand(Enum command, object paramerter)
		{
			commands.Add(new Command(command, paramerter));
		}

		private Command DequeueCommand()
		{
			if (commands.Count == 0)
			{
				return null;
			}
			
			var currentCommand = commands[commands.Count - 1];
			commands.RemoveAt(commands.Count - 1);
			
			return currentCommand;
		}
	}
}
