/* Copyright (C) 2015 Kevin Boronka
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
using sar.Commands;

namespace sar.Base
{
	public abstract class CommandHub
	{
		protected List<Command> commandList;
		
		public bool NoWarning = false;
		public bool Debug = false;
		public bool IncludeSVN = false;
		public bool IncludeSubFolders = true;
		public bool commandlineActive;
		public bool Loop = false;
		public bool RunAsAdministator = false;
		public bool PauseAfter = false;
		
		internal Dictionary<string, Command> commands = new Dictionary<string, Command>();
		
		public CommandHub()
		{
			this.commandList = new List<Command>();
			this.commandList.AddRange(new Command[] {
			                          	new Help(this),
			                          	new Delay(this)
			                          });
		}
		
		public int ProcessCommands(string[] args)
		{
			try
			{
				int exitCode = ConsoleHelper.EXIT_OK;
				
				this.commandlineActive = (args.Length == 0);
				
				do
				{
					try
					{
						while (args.Length == 0 && commandlineActive)
						{
							ConsoleHelper.Write("> ", ConsoleColor.White);
							
							args = StringHelper.ParseString(ConsoleHelper.ReadLine(), " ");
						}
						

						args = this.RemoveGlobalArgs(args);

						if (args.Length != 0)
						{
							string command = args[0].ToLower();
							if (command[0] == '-' || command[0] == '/')
							{
								command = command.Substring(1);
							}
							
							// Execute Command
							if (command != "exit")
							{
								Progress.Enabled = true;

								do
								{
									try
									{
										exitCode = this.Execute(command, args);
									}
									catch (Exception ex)
									{
										if (!this.Loop) throw;
										ConsoleHelper.WriteException(ex);
									}
									
									if (this.Loop)
									{
										Progress.Message ="Looping";
										Thread.Sleep(2000);
									}
									
								} while (this.Loop);
								
								Progress.Enabled = false;
								args = new string[0];
							}
							else
							{
								this.commandlineActive = false;
							}
						}
						
						if (this.PauseAfter)
						{
							ConsoleHelper.WriteLine("Press anykey to continue", ConsoleColor.Yellow);
							ConsoleHelper.ReadKey();
						}
					}
					catch (Exception ex)
					{
						args = new string[0];
						Progress.Enabled = false;
						ConsoleHelper.WriteException(ex);
						if (this.PauseAfter)
						{
							ConsoleHelper.WriteLine("Press anykey to continue", ConsoleColor.Yellow);
							ConsoleHelper.ReadKey();
						}
						
						exitCode = ConsoleHelper.EXIT_ERROR;
					}
				} while (this.commandlineActive);
				
				ConsoleHelper.Shutdown();
				return exitCode;
			}
			catch (Exception ex)
			{
				Progress.Enabled = false;
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
			
			int exitCode = ConsoleHelper.EXIT_ERROR;
			Progress.Enabled = true;
			
			do
			{
				exitCode = this.commands[command].Execute(args);
				
				if (this.Loop)
				{
					Progress.Message ="Loop Delay";
					Thread.Sleep(2000);
				}
				
			} while (this.Loop);
			
			Progress.Enabled = false;
			
			return exitCode;
		}
		
		private string[] RemoveGlobalArgs(string[] args)
		{
			this.NoWarning = false;
			this.Debug = false;
			this.IncludeSVN = false;
			this.IncludeSubFolders = true;
			this.Loop = false;
			this.PauseAfter = false;
			
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
						case "/quite":
							this.NoWarning = true;
							break;
						case "/d":
						case "/debug":
							this.Debug = true;
							break;
						case "/svn":
							this.IncludeSVN = true;
							break;
						case "/nosubfolders":
						case "/nosubs":
							this.IncludeSubFolders = false;
							break;
						case "/l":
						case "/loop":
							this.Loop = true;
							break;
						case "/pause":
							this.PauseAfter = true;
							break;
						case "/admin":
							this.RunAsAdministator = true;
							
							if (!ApplicationInfo.HasAdministrativeRight)
							{
								List<string> newArgs = new List<string>();
								
								foreach (string oldArg in args)
								{
									if (oldArg.Contains(" "))
									{
										newArgs.Add('"' + oldArg + '"');
									}
									else
									{
										newArgs.Add(oldArg);
									}
								}
								
								Shell.RunElevated(ApplicationInfo.ApplicationPath, string.Join(" ", newArgs.ToArray()), ApplicationInfo.CurrentDirectory);
								Environment.Exit(0);
								return new string[] {};
							}
							
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
