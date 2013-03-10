/*
 * User: Kevin Boronka (kboronka@gmail.com)
 */
 
using System;
using skylib.Tools;
using System.Collections.Generic;

namespace skylib.sar
{
	public class CommandHub
	{
		public static Dictionary<string, BaseCommand> commands = new Dictionary<string, BaseCommand>();
		
		public static void Add(string commandString, BaseCommand commandClass)
		{
			CommandHub.commands.Add(commandString, commandClass);
		}
		
		public static int Execute(string command, string[] args)
		{
			if (String.IsNullOrEmpty(command))
			{
				throw new NullReferenceException("no command provided");
			}
			
			command = command.ToLower();
			
			if (!CommandHub.commands.ContainsKey(command))
			{
				throw new ArgumentException("Unknown command");
			}
			
			return CommandHub.commands[command].Execute(args);
			//return (int)CommandHub.commands[command].function.DynamicInvoke(new object[] { args });
		}
	}
}