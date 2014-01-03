
using System;
using System.Collections.Generic;

using sar.Tools;

namespace sar.Base
{
	public class CommandHubBase
	{
		public CommandHubBase()
		{
		
		}
		
		public bool NoWarning = false;
		public bool Debug = false;
		public bool IncludeSVN = false;
		public bool IncludeSubFolders = true;
		public Dictionary<string, BaseCommand> commands = new Dictionary<string, BaseCommand>();
		
		public void Add(string commandString, BaseCommand commandClass)
		{
			try
			{
				this.commands.Add(commandString, commandClass);
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteLine("Command: " + commandString);
				ConsoleHelper.WriteException(ex);
			}
		}
		
		public int Execute(string command, string[] args)
		{
			if (String.IsNullOrEmpty(command))
			{
				throw new NullReferenceException("no command provided");
			}
			
			command = command.ToLower();
			
			if (!this.commands.ContainsKey(command))
			{
				throw new ArgumentException("Unknown command");
			}
			
			
			Progress.UpdateTimer.Enabled = true;
			int exitCode = this.commands[command].Execute(args);
			//int exitCode =  (int)this.commands[command].function.DynamicInvoke(new object[] { args });
			Progress.UpdateTimer.Enabled = false;
			return exitCode;
		}
		
		public string[] RemoveGlobalArgs(string[] args)
		{
			this.NoWarning = false;
			this.Debug = false;
			this.IncludeSVN = false;
			this.IncludeSubFolders = true;
			
			#if DEBUG
			this.Debug = true;
			#endif
			
			List<string> result = new List<string>();
			
			foreach (string arg in args)
			{
				if (arg.Length > 1 && arg.Substring(0, 1) == "/")
				{
					switch (arg.ToLower())
					{
						case "/q":
							this.NoWarning = true;
							break;
						case "/d":
							this.Debug = true;
							break;
						case "/svn":
							this.IncludeSVN = true;
							break;
						case "/nosubfolders":
						case "/nosubs":
							this.IncludeSubFolders = false;
							break;
						default:
							result.Add(arg);
							break;
					}
				}
				else
				{
					result.Add(arg);
				}
			}
			
			ConsoleHelper.ShowDebug = this.Debug;
			IO.IncludeSubFolders = this.IncludeSubFolders;
			
			return result.ToArray();
		}
	}
}
