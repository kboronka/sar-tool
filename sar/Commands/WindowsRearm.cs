/* Copyright (C) 2017 Kevin Boronka
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

using sar.Base;
using sar.Tools;

namespace sar.Commands
{
	public class WindowsRearm : Command
	{
		public WindowsRearm(Base.CommandHub parent) : base(parent, "Windows - Activation Trial Rearm",
		                                                   new List<string> { "windows.rearm", "win.rearm", "w.rarm" },
		                                                   @"-windows.rearm",
		                                                   new List<string> { "-windows.rearm" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			if (Environment.OSVersion.Version.Major != 6)
			{
				throw new Exception("Windows 7 not detected");
			}
			
			if (this.commandHub.NoWarning || ConsoleHelper.Confirm("Caution: Rearm Activation? (y/n) "))
			{
				Progress.Message = "finding cscript.exe";
				var cscript = IO.FindSystemApplication("cscript.exe");
				ConsoleHelper.DebugWriteLine("cscript = " + cscript);
				
				Progress.Message = "Rearming Windows Activation";
				
				var output = "";
				var error = "";
				
				ConsoleHelper.Run(cscript, Environment.SystemDirectory + @"\slmgr.vbs /rearm", Environment.SystemDirectory, out output, out error);
				ConsoleHelper.DebugWriteLine("output = " + output);
				ConsoleHelper.DebugWriteLine("error = " + error);
				
				//System.Diagnostics.Process.Start(cscript + @" //B //Nologo " + Environment.SystemDirectory + @"\slmgr.vbs /rearm");

				ConsoleHelper.WriteLine("Rearmed - Reboot Required", ConsoleColor.DarkYellow);
				return ConsoleHelper.EXIT_OK;
			}
			
			return ConsoleHelper.EXIT_ERROR;
		}
	}
}
