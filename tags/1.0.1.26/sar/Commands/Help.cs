/*
 * User: Kevin Boronka (kboronka@gmail.com)
 */

using System;
using skylib.Tools;
using System.Collections.Generic;

namespace skylib.sar
{
	public class Help : BaseCommand
	{
		public Help() : base("Help", new List<string> { "help", "?" }, "-help [command]", new List<string>() { @"-help bk" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(AssemblyInfo.Name + "  v" + AssemblyInfo.Version + "  " + AssemblyInfo.Copyright);
			Console.ResetColor();

			if (args.Length == 2)
			{
				string commandString = args[1];
				if (CommandHub.commands.ContainsKey(commandString))
				{
					BaseCommand command = CommandHub.commands[commandString];
					
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine("\nUsage:");
					Console.ResetColor();
					Console.WriteLine("\t" + command.Usage);
					
					if (command.Examples.Count > 0)
					{
						Console.ForegroundColor = ConsoleColor.White;
						if (command.Examples.Count > 1) Console.WriteLine("\nExamples:");
						if (command.Examples.Count == 1) Console.WriteLine("\nExample:");
						Console.ResetColor();
						
						foreach (string example in command.Examples)
						{
							Console.WriteLine("\t" + example);
						}

						
					}
				}
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("\nUsage:");
				Console.ResetColor();
				
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("\t -help [command]");
				Console.WriteLine("\nCommands:");
				Console.ResetColor();
				
				BaseCommand lastCommand = null;
				foreach (BaseCommand command in CommandHub.commands.Values)
				{
					if (command != lastCommand)
					{
						lastCommand = command;
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Write("\t" + command.Name + ": ");
						Console.ResetColor();
						
						string seperator = "";
						foreach (string commandString in command.Commands)
						{
							Console.Write(seperator);
							Console.Write(commandString);
							seperator = ", ";
						}
						
						Console.WriteLine();
					}
				}
			}
			
			return Program.EXIT_OK;
		}
	}
}