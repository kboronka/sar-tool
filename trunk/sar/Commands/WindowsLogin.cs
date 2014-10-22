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
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class WindowsLogin : Command
	{
		public WindowsLogin(Base.CommandHub parent) : base(parent, "Windows - Login", new List<string> { "windows.login", "win.login", "net.login", "n.login" },
		                         "-windows.login [ip] [domain/username] [password] [persistent]",
		                         new List<string>() { @"-windows.login \\192.168.0.244\temp test testpw p" })
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
			Progress.Message = "Logging into " + serverAddres;
			
			string uncPath = serverAddres;
			if (uncPath.Substring(0,2) != @"\\") uncPath = @"\\" + uncPath;
			
			string userName = args[2];
			string password = args[3];
			string persistent = "/PERSISTENT:NO";

			if (args.Length >= 4 && (args[3].ToLower() == "p" || args[3].ToLower() == "persistent"))
			{
				persistent = " /persistent:yes";
			}
			
			int exitcode;
			
			exitcode = ConsoleHelper.Run("net", @"use " + uncPath + @" /DELETE /y");
			exitcode = ConsoleHelper.Run("net", @"use " + uncPath + @" /USER:" + userName + " " + password + " " + persistent);
			
			if (exitcode != 0)
			{
				ConsoleHelper.WriteLine("Login to " + serverAddres + " has failed", ConsoleColor.DarkYellow);
				return ConsoleHelper.EXIT_ERROR;
			}

			ConsoleHelper.WriteLine("Login to " + serverAddres + " was successful", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
