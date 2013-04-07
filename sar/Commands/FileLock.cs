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
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace skylib.sar
{
	public class FileLock : BaseCommand
	{
		public FileLock() : base("File - Lock",
		                         new List<string> { "file.lock", "f.lock" },
		                         "-file.find [filepattern] <timeout>",
		                         new List<string> { "-file.find \"*.exe\" 10000" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Waiting for lock";
			string filePattern = args[1];
			
			int timeout = 5 * 60 * 1000;
			Int32.TryParse(args[2], out timeout);
			
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = new List<string>();

			
			if (!IO.WaitForFileSystem(root, timeout, true))
			{
				ConsoleHelper.WriteLine("File System Not Found", ConsoleColor.DarkYellow);
				return Program.EXIT_ERROR;
			}
			else
			{
				ConsoleHelper.WriteLine("File System Found", ConsoleColor.DarkYellow);
			}
			
			return Program.EXIT_OK;
		}
	}
}
