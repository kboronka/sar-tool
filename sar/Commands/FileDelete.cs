/* Copyright (C) 2021 Kevin Boronka
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

using sar.Base;
using sar.Tools;
using System;
using System.Collections.Generic;
using System.IO;

namespace sar.Commands
{
	public class FileDelete : Command
	{
		public FileDelete(Base.CommandHub parent) : base(parent, "File - Delete",
														 new List<string> { "file.delete", "f.del" },
														 "-f.del [filepattern]",
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

			Progress.Message = "Searching";
			string filePattern = args[1];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);

			if (!this.commandHub.NoWarning)
			{
				if (this.commandHub.Debug)
				{
					foreach (string file in files)
					{
						ConsoleHelper.Write("found: ", ConsoleColor.Cyan);
						ConsoleHelper.WriteLine(StringHelper.TrimStart(file, root.Length));
					}
				}

				ConsoleHelper.WriteLine(files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : "") + " found");
			}

			int counter = 0;
			if (files.Count > 0)
			{
				if (this.commandHub.NoWarning || ConsoleHelper.Confirm("Delete " + files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : "") + "? (y/n)"))
				{
					foreach (string file in files)
					{
						Progress.Message = "Deleting " + StringHelper.TrimStart(file, root.Length);

						try
						{
							if (this.commandHub.IncludeSVN || !IO.IsSVN(file))
							{
								File.Delete(file);
								counter++;
							}
						}
						catch (Exception ex)
						{
							ConsoleHelper.Write("failed: ", ConsoleColor.Red);
							ConsoleHelper.WriteLine(StringHelper.TrimStart(file, root.Length));

							if (this.commandHub.Debug)
							{
								ConsoleHelper.WriteException(ex);
							}
						}
					}
				}
			}

			ConsoleHelper.WriteLine(counter.ToString() + " File" + ((counter != 1) ? "s" : "") + " Deleted", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
