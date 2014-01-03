/* Copyright (C) 2014 Kevin Boronka
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Threading;

using sar.Tools;

namespace sar.Base
{
	public abstract class CommandHub
	{
		public bool NoWarning = false;
		public bool Debug = false;
		public bool IncludeSVN = false;
		public bool IncludeSubFolders = true;
		public bool commandlineActive = false;
		internal Dictionary<string, Command> commands = new Dictionary<string, Command>();
		
		public CommandHub()
		{
			
		}
		
		internal int ProcessCommands(string[] args)
		{
			try
			{
				int exitCode = ConsoleHelper.EXIT_OK;
				
				while (!commandlineActive)
				{
					try
					{
						if (args.Length == 0)
						{
							commandlineActive = false;
							args = new string[1];
							ConsoleHelper.Write("> ", ConsoleColor.White);
							
							args = StringHelper.ParseString(ConsoleHelper.ReadLine(), " ");
							args = this.RemoveGlobalArgs(args);
							
							if (args.Length == 0)
							{
								throw new ArgumentException("too few arguments");
							}
						}
						else
						{
							args = this.RemoveGlobalArgs(args);
							commandlineActive = true;
						}
						
						string command = args[0].ToLower();
						if (command[0] == '-' || command[0] == '/')
						{
							command = command.Substring(1);
						}
						
						// Execute Command
						if (command != "exit")
						{
							Progress.UpdateTimer.Enabled = true;
							exitCode = this.Execute(command, args);
							Progress.UpdateTimer.Enabled = false;
							args = new string[0];
						}
					}
					catch (Exception ex)
					{
						try
						{
							Progress.UpdateTimer.Enabled = false;
							ConsoleHelper.WriteException(ex);

							if (this.Debug && commandlineActive)
							{
								Thread.Sleep(2000);
							}
							
							args = new string[0];
						}
						catch
						{
							
						}
						
						exitCode = ConsoleHelper.EXIT_ERROR;
					}
				}
				
				ConsoleHelper.Shutdown();
				return exitCode;
			}
			catch (Exception ex)
			{
				Progress.UpdateTimer.Enabled = false;
				ConsoleHelper.WriteException(ex);
				Thread.Sleep(2000);
				
				if (this.Debug)
				{
					ConsoleHelper.WriteLine("Press anykey to continue", ConsoleColor.Yellow);
					ConsoleHelper.ReadKey();
				}

				return ConsoleHelper.EXIT_ERROR;
			}
		}
		
		internal void Add(string commandString, Command commandClass)
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
		
		private int Execute(string command, string[] args)
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
		
		private string[] RemoveGlobalArgs(string[] args)
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
