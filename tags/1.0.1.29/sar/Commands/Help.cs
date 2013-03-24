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
		private static bool titleDisplayed = false;
		
		public Help() : base("Help", new List<string> { "help", "?" }, "-help [command]", new List<string>() { @"-help bk" })
		{
			
		}
		
		public static void WriteTitle()
		{
			if (!Help.titleDisplayed)
			{
				ConsoleHelper.Write(AssemblyInfo.Product + " v" + AssemblyInfo.Version, ConsoleColor.Yellow);
				ConsoleHelper.WriteLine("  " + AssemblyInfo.Copyright);
				titleDisplayed = true;
			}
		}
		
		public override int Execute(string[] args)
		{
			Help.WriteTitle();

			if (args.Length == 2)
			{
				string commandString = args[1];
				if (CommandHub.commands.ContainsKey(commandString))
				{
					BaseCommand command = CommandHub.commands[commandString];
					
					ConsoleHelper.WriteLine("\nUsage:", ConsoleColor.White);
					ConsoleHelper.WriteLine("\t" + command.Usage);
					
					if (command.Examples.Count > 0)
					{
						if (command.Examples.Count > 1) ConsoleHelper.WriteLine("\nExamples:", ConsoleColor.White);
						if (command.Examples.Count == 1) ConsoleHelper.WriteLine("\nExample:", ConsoleColor.White);
						
						foreach (string example in command.Examples)
						{
							ConsoleHelper.WriteLine("\t" + example);
						}
					}
				}
			}
			else
			{
				ConsoleHelper.WriteLine("\nUsage:", ConsoleColor.White);
				ConsoleHelper.WriteLine("\t -help [command]", ConsoleColor.White);
				ConsoleHelper.WriteLine("\nCommands:");
				
				BaseCommand lastCommand = null;
				foreach (BaseCommand command in CommandHub.commands.Values)
				{
					if (command != lastCommand)
					{
						lastCommand = command;
						ConsoleHelper.Write("\t" + command.Name + ": ", ConsoleColor.Red);
						
						string seperator = "";
						foreach (string commandString in command.Commands)
						{
							ConsoleHelper.Write(seperator);
							ConsoleHelper.Write(commandString);
							seperator = ", ";
						}
						
						ConsoleHelper.WriteLine("");
					}
				}
			}
			
			return Program.EXIT_OK;
		}
	}
}