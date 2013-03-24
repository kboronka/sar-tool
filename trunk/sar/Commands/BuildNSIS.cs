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
using System.IO;

namespace skylib.sar
{
	public class BuildNSIS : BaseCommand
	{
		public BuildNSIS() : base("Build NSIS installer",
		                          new List<string> { "build.nsis", "b.nsis" },
		                          "-b.nsis <nsis_filepath>",
		                          new List<string> { @"-b.nsis src\Installer\chesscup.nsi" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 2)
			{
				throw new ArgumentException("too few arguments");
			}
			
			string scriptFile =  "\"" + IO.FindFile(args[1]) + "\"";
			string exePath = IO.FindApplication("makensis.exe");
					
			string arguments = "";
			for (int i = 2; i < args.Length; i++)
			{
				arguments += " " + args[i];
			}
			
			arguments += " " + scriptFile;
			
			#if DEBUG
			ConsoleHelper.WriteLine(exePath + " " + arguments);
			#endif

			string output;
			int exitcode = ConsoleHelper.Shell(exePath, arguments, out output);
			
			if (exitcode != 0)
			{
				ConsoleHelper.WriteLine("Build Failed", ConsoleColor.DarkYellow);
				ConsoleHelper.WriteLine(output, ConsoleColor.DarkCyan);
				return exitcode;
			}
			else
			{
				ConsoleHelper.WriteLine("Build Successfully Completed", ConsoleColor.DarkYellow);
				return Program.EXIT_OK;
			}
		}
	}
}
