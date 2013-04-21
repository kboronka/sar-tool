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

namespace skylib.sar
{
	public class FileDestory : BaseCommand
	{
		public FileDestory() : base("File - Destroy",
		                            new List<string> { "file.destroy", "f.d" },
		                            "-f.d [filepattern]",
		                            new List<string> { "-f.d \"*.vmdk\"" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length > 2 && args.Length > 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			// TODO: remove this when /q condition is working
			if (args.Length == 3) Program.NoWarning = Program.NoWarning || (args[2] == "quite");
			
			Progress.Message = "Searching";
			string filePattern = args[1];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			if (!Program.NoWarning)
			{
				foreach (string file in files)
				{
					ConsoleHelper.Write("found: ", ConsoleColor.Cyan);
					ConsoleHelper.WriteLine(StringHelper.TrimStart(file, root.Length));
				}

				ConsoleHelper.WriteLine(files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : "") + " found");
			}
			
			int counter = 0;
			if (files.Count > 0)
			{
				if (Program.NoWarning || ConsoleHelper.Confirm("Destroy " + files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : "") + "? (y/n)"))
				{
					foreach (string file in files)
					{
						Progress.Message = "Destroying " + StringHelper.TrimStart(file, root.Length);

						try
						{
							IO.DestroyFile(file);
							counter++;
						}
						catch (Exception ex)
						{
							ConsoleHelper.Write("failed: ", ConsoleColor.Red);
							ConsoleHelper.WriteLine(StringHelper.TrimStart(file, root.Length));
							
							if (Program.Debug)
							{
								ConsoleHelper.WriteException(ex);
							}
						}
					}
				}
			}
			
			ConsoleHelper.WriteLine(counter.ToString() + " File" + ((counter != 1) ? "s" : "") + " Destroyed", ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}
