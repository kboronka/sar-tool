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

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class VboxManage : Command
	{
		public VboxManage(Base.CommandHub parent) : base(parent, "Vbox Manage Tool",
		                           new List<string> { "vbox.manage", "vb.manage" },
		                           @"-vbox.manage [arg1] [arg2] [arg3]",
		                           new List<string> { "-vbox.manage modifyhd \"WinXP-disk1.vdi\" --resize 3000" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 2)
			{
				throw new ArgumentException("too few arguments");
			}
					
			string exePath = IO.FindApplication("VBoxManage.exe");
			
			string arguments = "";
			for (int i = 1; i < args.Length; i++)
			{
				arguments += " " + args[i];
			}

			string output;
			int exitcode = ConsoleHelper.Run(exePath, arguments, out output);
				
			if (exitcode != 0)
			{
				ConsoleHelper.WriteLine("Command Failed", ConsoleColor.DarkYellow);
				ConsoleHelper.WriteLine(output, ConsoleColor.DarkCyan);
				ConsoleHelper.WriteLine("exit code: " + exitcode.ToString());
				return exitcode;
			}
			else
			{
				ConsoleHelper.WriteLine("Command Successfully Completed", ConsoleColor.DarkYellow);
				return ConsoleHelper.EXIT_OK;
			}
		}
	}
}
