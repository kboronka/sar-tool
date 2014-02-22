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
	public class FileRemoveDirectory : Command
	{
		public FileRemoveDirectory(Base.CommandHub parent) : base(parent, "File - Remove Directory",
		                                    new List<string> { "file.removedirectory", "f.rd", "d.d" },
		                                    "-f.d [filepattern]",
		                                    new List<string> { "-f.rd \"C:\\Temp\"" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Searching";
			string root = args[1];
			List<string> directories = IO.GetAllDirectories(root);
			
			if (!Directory.Exists(root)) throw new ApplicationException("Directory does not exist");
			
			
			if (!this.commandHub.NoWarning)
			{
				foreach (string directory in directories)
				{
					ConsoleHelper.Write("found: ", ConsoleColor.Cyan);
					ConsoleHelper.WriteLine(StringHelper.TrimStart(directory, root.Length));
				}

				ConsoleHelper.WriteLine(directories.Count.ToString() + " file" + ((directories.Count != 1) ? "s" : "") + " found");
			}
			
			if (directories.Count > 0)
			{
				if (this.commandHub.NoWarning || ConsoleHelper.Confirm("Destroy " + directories.Count.ToString() + " director" + ((directories.Count != 1) ? "ies" : "y") + "? (y/n)"))
				{
					Progress.Message = "Destroying " + root;
					
					try
					{
						Directory.Delete(root, true);
					}
					catch (Exception ex)
					{
						ConsoleHelper.Write("failed: ", ConsoleColor.Red);
						ConsoleHelper.WriteLine(root);
						
						if (this.commandHub.Debug)
						{
							ConsoleHelper.WriteException(ex);
						}
					}
				}
			}
			
			ConsoleHelper.WriteLine(directories.Count.ToString() + " Director" + ((directories.Count != 1) ? "ies" : "y") + " Destroyed", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}