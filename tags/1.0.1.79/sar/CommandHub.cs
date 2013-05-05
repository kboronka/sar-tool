/* Copyright (C) 2013 Kevin Boronka
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

using skylib.Tools;

namespace skylib.sar
{
	public class CommandHub
	{
		public static Dictionary<string, BaseCommand> commands = new Dictionary<string, BaseCommand>();
		
		public static void Add(string commandString, BaseCommand commandClass)
		{
			try
			{
				CommandHub.commands.Add(commandString, commandClass);
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteLine("Command: " + commandString);
				ConsoleHelper.WriteException(ex);
			}
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