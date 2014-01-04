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

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class Help : Command
	{
		public Help(Base.CommandHub parent) : base(parent, "Help", new List<string> { "help", "?" }, "-help [command]", new List<string>() { @"-help bk" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			if (args.Length == 2)
			{
				string commandString = args[1];
				if (this.commandHub.commands.ContainsKey(commandString))
				{
					Command command = this.commandHub.commands[commandString];
					
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
				ConsoleHelper.WriteLine("\t -help [command]");
				ConsoleHelper.WriteLine("\nCommands:", ConsoleColor.White);
				
				Command lastCommand = null;
				foreach (Command command in this.commandHub.commands.Values)
				{
					if (command != lastCommand)
					{
						lastCommand = command;
						ConsoleHelper.Write("\t" + command.Name, ConsoleColor.Yellow);
						ConsoleHelper.Write(": ");
						
						string seperator = "";
						foreach (string commandString in command.Commands)
						{
							ConsoleHelper.Write(seperator);
							ConsoleHelper.Write(commandString, ConsoleColor.Gray);
							seperator = " | ";
						}
						
						ConsoleHelper.WriteLine("");
					}
				}
			}
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}