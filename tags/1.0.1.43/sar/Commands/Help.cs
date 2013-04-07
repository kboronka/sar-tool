/* Copyright (C) 2013 Kevin Boronka
 * 
 * software is distributed under the BSD license
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
				
				string copyright = " \n";
				copyright += "this software is distributed under the BSD license\n";
				copyright += "\n";
				copyright += "THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\"\n";
				copyright += "AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE\n";
				copyright += "IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE\n";
				copyright += "ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE\n";
				copyright += "LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR\n";
				copyright += "CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF\n";
				copyright += "SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS\n";
				copyright += "INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN\n";
				copyright += "CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)\n";
				copyright += "ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE\n";
				copyright += "POSSIBILITY OF SUCH DAMAGE.\n";
				ConsoleHelper.WriteLine(copyright, ConsoleColor.DarkGreen);
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
				ConsoleHelper.WriteLine("\t -help [command]");
				ConsoleHelper.WriteLine("\nCommands:", ConsoleColor.White);
				
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