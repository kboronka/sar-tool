/*
 * User: Kevin Boronka (kboronka@gmail.com)
 */

using System;
using skylib.Tools;
using System.Collections.Generic;

namespace skylib.sar
{
	public abstract class BaseCommand
	{
		private string usage;
		private List<string> examples;
		private string name;
		
		private List<string> commands;
		private delegate int FunctionPointer (string[] args);
		public Delegate function;
		
		public List<string> Commands
		{
			get
			{
				return this.commands;
			}
		}
		
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Usage
		{
			get
			{
				return this.usage;
			}
		}
		
		public List<string> Examples
		{
			get
			{
				return this.examples;
			}
		}

		protected BaseCommand(string name, List<string> commands, string help, List<string> examples)
		{
			this.function = new FunctionPointer(this.Execute);
			this.usage = help;
			this.name = name;
			this.commands = commands;
			this.examples = examples;
			
			foreach (string command in commands)
			{
				CommandHub.Add(command.ToLower(), this);
			}
		}

		public abstract int Execute(string[] args);
	}

}
