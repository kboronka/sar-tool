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
	public class FileDestory : BaseCommand
	{
		public FileDestory() : base("Destory Files",
		                            new List<string> { "file.destroy", "f.d" },
		                            "-f.d <filepattern>",
		                            new List<string> { "-f.d \"*.vmdk\"" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
						
			string filePattern = args[1];						
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
						
			ConsoleHelper.WriteLine(files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : "") + " found");
			
			int count = 0;
			
			if (files.Count > 0)
			{
				if (Program.NoWarning || ConsoleHelper.Confirm("Destroy files? (y/n)"))
				{
					foreach (string file in files)
					{
						try
						{
							File.SetAttributes(file, FileAttributes.Normal);
							StreamWriter sw = new StreamWriter(file);
							sw.WriteLine("file corrupt");
							sw.Close();
							sw.Dispose();
							File.Delete(file);
							ConsoleHelper.Write("destroyed: ", ConsoleColor.Cyan);
							ConsoleHelper.WriteLine(file.Substring(root.Length + 1));
							count++;
						}
						catch (Exception ex)
						{
							ConsoleHelper.Write("failed: ", ConsoleColor.Red);
							ConsoleHelper.WriteLine(file.Substring(root.Length + 1));
							
							if (Program.Debug)
							{
								ConsoleHelper.WriteException(ex);
							}
						}
					}
				}
			}
			
			
			ConsoleHelper.WriteLine("Files destroyed: " + count.ToString() + " of " + files.Count.ToString());
			return Program.EXIT_OK;
		}
	}
}
