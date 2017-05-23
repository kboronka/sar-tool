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
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class FileLock : Command
	{
		public FileLock(Base.CommandHub parent) : base(parent, "File - Lock",
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
			
			int timeout = 5 * 60 * 1000;
			Int32.TryParse(args[2], out timeout);
			
			Progress.Message = "Searching";
			string filePattern = args[1];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			Progress.Message = "Waiting for lock";			
			if (!IO.WaitForFileSystem(root, timeout, true))
			{
				ConsoleHelper.WriteLine("File System Not Found", ConsoleColor.DarkYellow);
				return ConsoleHelper.EXIT_ERROR;
			}
			else
			{
				ConsoleHelper.WriteLine("File System Found", ConsoleColor.DarkYellow);
			}
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}
