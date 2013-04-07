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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

using skylib.Tools;

namespace skylib.sar
{
	public class WindowsRestart : BaseCommand
	{
		public WindowsRestart() : base("Windows - Restart",
		                               new List<string> { "windows.restart", "win.restart" },
		                               @"-windows.restart [ip | computername] [domain/username] [password] <timeout (ms)>",
		                               new List<string> { "-windows.restart 192.168.0.244 admin-username mypassword 35000" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 4 || args.Length > 5)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string serverAddres = args[1];
			Progress.Message = "Rebooting " + serverAddres;
			
			string uncPath = serverAddres;
			if (uncPath.Substring(0,2) != @"\\") uncPath = @"\\" + uncPath;
			
			string userName = args[2];
			string password = args[3];
			
			int timeout = 5 * 60 * 1000;
			Int32.TryParse(args[4], out timeout);
			
			int exitcode;
			
			exitcode = ConsoleHelper.Shell("net", @"use " + uncPath + @" /DELETE");
			exitcode = ConsoleHelper.Shell("net", @"use " + uncPath + @" /USER:" + userName + " " + password);
			
			if (exitcode != 0)
			{
				ConsoleHelper.WriteLine("Login Failed", ConsoleColor.DarkYellow);
				return Program.EXIT_ERROR;
			}
			
			exitcode = ConsoleHelper.Shell("shutdown", @"/r /m " + uncPath + @" /t 0 /f");

			if (exitcode != 0)
			{
				ConsoleHelper.WriteLine(serverAddres + " failed to restart", ConsoleColor.DarkYellow);
				return Program.EXIT_ERROR;
			}
			
			if (timeout != 0)
			{
				Progress.Message = "Shutting Down " + serverAddres;
				if (!NetHelper.WaitForPing(serverAddres, timeout, false))
				{
					ConsoleHelper.WriteLine(serverAddres + " did not shutdown", ConsoleColor.DarkYellow);
					return Program.EXIT_ERROR;
				}
				else
				{
					ConsoleHelper.WriteLine(serverAddres + " has shutdown", ConsoleColor.DarkYellow);
				}
				
				Progress.Message = "Restarting " + serverAddres;
				
				if (!NetHelper.WaitForPing(serverAddres, timeout, true))
				{
					ConsoleHelper.WriteLine(serverAddres + " did not restart", ConsoleColor.DarkYellow);
					return Program.EXIT_ERROR;
				}
				else
				{
					ConsoleHelper.WriteLine(serverAddres + " has restarted", ConsoleColor.DarkYellow);
				}
			}
			
			ConsoleHelper.WriteLine(serverAddres + " reboot complete", ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}
