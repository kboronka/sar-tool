/* Copyright (C) 2016 Kevin Boronka
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
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class WindowsLogin : Command
	{
		public WindowsLogin(Base.CommandHub parent) : base(parent, "Windows - Login", new List<string> { "windows.login", "win.login", "net.login", "n.login" },
		                                                   "-windows.login [ip] [domain/username] [password] [p|persistent] [ping]",
		                                                   new List<string>() { @"-windows.login \\192.168.0.244\temp test testpw p",
		                                                   	@"-n.login 192.168.0.244 test testpw ping" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 4)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string path = args[1];
			Progress.Message = "Logging into " + path;
			
			string username = args[2];
			string password = args[3];
			bool persistent = false;
			bool ping = false;
			

			if (args.Length >= 5)
			{
				for (int argIndex = 4; argIndex < args.Length; argIndex++)
				{
					string arg = args[argIndex].ToLower();
					if (arg  == "p" || arg == "persistent")
					{
						persistent = true;
					}
					else if (arg == "-ping")
					{
						ping = true;
					}
				}
			}
			
			if (Login(path, username, password, persistent, ping) != ConsoleHelper.EXIT_OK)
			{
				ConsoleHelper.WriteLine("Login to " + path + " has failed", ConsoleColor.DarkYellow);
				return ConsoleHelper.EXIT_ERROR;
			}

			ConsoleHelper.WriteLine("Login to " + path + " was successful", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
		
		public static int Login(string path, string username, string password, bool persistent, bool ping)
		{
			string uncPath = path;
			if (uncPath.Substring(0,2) != @"\\") uncPath = @"\\" + uncPath;
			string hostName = NetHelper.GetHostName(uncPath);
			
			string persistentCommand = "/persistent:no";

			if (persistent) persistentCommand = "/persistent:yes";
			if (ping && !NetHelper.Ping(hostName, 200)) throw new ApplicationException("Unable to ping " + hostName);
			int exitcode;
			
			exitcode = ConsoleHelper.Run("net", @"use " + uncPath + @" /DELETE /y");
			exitcode = ConsoleHelper.Run("net", @"use " + uncPath + @" /USER:" + username + " " + password + " " + persistentCommand);
			
			if (exitcode != 0)
			{
				return ConsoleHelper.EXIT_ERROR;
			}

			return ConsoleHelper.EXIT_OK;
		}
	}
}
