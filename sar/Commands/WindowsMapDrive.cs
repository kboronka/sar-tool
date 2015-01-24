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
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class WindowsMapDrive : Command
	{
		public WindowsMapDrive(Base.CommandHub parent) : base(parent, "Windows - Map Drive", new List<string> { "windows.map", "win.map" },
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
			
			bool persistent = false;
			if (args.Length >= 4 && (args[3].ToLower() == "p" || args[3].ToLower() == "persistent"))
			{
				persistent = true;
			}
			
			return MapDrive(drive, uncPath, persistent);
		}
		
		public static bool MappingExists(string drive, string uncPath)
		{
			int exitcode;
			string result = "";
			
			exitcode = ConsoleHelper.Run("net", @"use", out result);
			
			return result.ToLower().Contains("ok           " + drive.ToLower() + ":        " + uncPath.ToLower());
		}
		
		public static int MapDrive(string drive, string uncPath, bool persistent)
		{
			if (uncPath.Substring(0,2) != @"\\") uncPath = @"\\" + uncPath;
			
			if (!MappingExists(drive, uncPath))
			{
				int exitcode;
				exitcode = ConsoleHelper.Run("net", @"use " + drive + @": /DELETE /y");
				exitcode = ConsoleHelper.Run("net", @"use " + drive + @": " + uncPath + (persistent ? " /savecred /persistent:yes" : ""));
				
				if (exitcode != 0)
				{
					ConsoleHelper.WriteLine("Mapping of " + drive + ": drive has failed", ConsoleColor.DarkYellow);
					return ConsoleHelper.EXIT_ERROR;
				}
				
				ConsoleHelper.WriteLine("Mapping of " + drive + ": drive was successful", ConsoleColor.DarkYellow);
			}
			else
			{
				ConsoleHelper.WriteLine(drive.ToUpper() + ": drive already mapped to " + uncPath, ConsoleColor.DarkYellow);
			}
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}