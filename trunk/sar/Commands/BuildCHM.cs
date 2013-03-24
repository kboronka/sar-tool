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
	public class BuildCHM : BaseCommand
	{
		public BuildCHM() : base("Build CHM help file",
		                         new List<string> { "build.chm", "b.chm" },
		                         "-b.chm <hhp_filepath>",
		                         new List<string> { @"-b.chm help\help.hhp" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 2)
			{
				throw new ArgumentException("too few arguments");
			}
			
			string hhpFile = args[1];
			
			// get list of hhc.exe file locations availble
			List<String> files = IO.GetAllFiles(IO.ProgramFilesx86, "hhc.exe");
			if (files.Count == 0)
			{
				files = IO.GetAllFiles(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles), "hhc.exe");
			}
			
			// sanity - hhc.exe not installed
			if (files.Count == 0)
			{
				throw new FileNotFoundException("sar unable to locate hhc.exe");
			}
			
			string hhcPath = files[0];
			
			// sanity - solution file exists
			if (!File.Exists(hhpFile))
			{
				throw new FileNotFoundException(hhpFile + " hhp file not found");
			}
			

			string arguments = hhpFile;
			
			for (int i = 2; i < args.Length; i++)
			{
				arguments += " " + args[i];
			}
			
			#if DEBUG
			ConsoleHelper.WriteLine(hhpFile + " " + arguments);
			#endif

			string output;
			int exitcode = ConsoleHelper.Shell(hhcPath + " " + arguments, out output);
			
			if (exitcode != 1)
			{
				ConsoleHelper.WriteLine("Build Failed", ConsoleColor.DarkYellow);
				ConsoleHelper.WriteLine(output, ConsoleColor.DarkCyan);
				return Program.EXIT_ERROR;
			}
			else
			{
				ConsoleHelper.WriteLine("Build Successfully Completed", ConsoleColor.DarkYellow);
				return Program.EXIT_OK;
			}
		}
	}
}
