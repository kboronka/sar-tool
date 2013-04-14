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
using skylib.Tools;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace skylib.sar
{
	public class Kill : BaseCommand
	{
		public Kill() : base("Kill Process",
		                     new List<string> { "kill", "k" },
		                     @"-kill [FilePath] [backup_location]",
		                     new List<string> { "-backup backup.zip \"c:\\backups\\\"" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("wrong number of arguments");
			}
			
			string processName = args[1];
			
			Progress.Message = "Locating Process " + processName;
			Process[] foundProcess = Process.GetProcessesByName(processName);
			if (foundProcess.Length != 0)
			{

				foreach (Process process in foundProcess)
				{
					process.Kill();
					ConsoleHelper.WriteLine(process.ProcessName + " has been shutdown", ConsoleColor.DarkYellow);
				}
			}
			else
			{
				ConsoleHelper.WriteLine(processName + " not running", ConsoleColor.DarkYellow);
			}
			
			return Program.EXIT_OK;
		}
	}
}
