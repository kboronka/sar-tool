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
using sar.Tools;
using System.Collections.Generic;
using System.IO;

namespace sar.Tools
{
	public class WindowsMapDrive : BaseCommand
	{
		public WindowsMapDrive() : base("Windows - Map Drive", new List<string> { "windows.map", "win.map" },
		                                "-windows.map [drive letter] [UNC path] [persistent]",
		                                new List<string>() { @"-windows.map S \\192.168.0.244\temp p" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 3 || args.Length > 4)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string drive = args[1];
			
			if (drive.Length != 1)
			{
				throw new ArgumentException("invalid drive letter");
			}
			
			// TODO: check drive letter
			
			string serverAddres = args[2];
			Progress.Message = "Logging into " + serverAddres;
			
			string uncPath = serverAddres;
			if (uncPath.Substring(0,2) != @"\\") uncPath = @"\\" + uncPath;
			
			string persistent = "";
			if (args.Length == 4 && args[3] == "p")
			{
				persistent = " /savecred /p:yes";
			}
			
			int exitcode;
			
			exitcode = ConsoleHelper.Run("net", @"use " + drive + @": /DELETE");
			exitcode = ConsoleHelper.Run("net", @"use " + drive + @": " + uncPath + persistent);
			
			if (exitcode != 0)
			{
				ConsoleHelper.WriteLine("Login to " + serverAddres + " has failed", ConsoleColor.DarkYellow);
				return Program.EXIT_ERROR;
			}

			ConsoleHelper.WriteLine("Login to " + serverAddres + " was successful", ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}
