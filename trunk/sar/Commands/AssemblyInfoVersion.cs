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
using sar.Tools;
using System.Collections.Generic;
using System.IO;

namespace sar.Tools
{
	public class AssemblyInfoVersion : BaseCommand
	{
		public AssemblyInfoVersion() : base("Set AssemblyInfo version number",
		                                    new List<string> { "assembly.version", "assy.ver" },
		                                    "-assembly.version [AssemblyInfo file] [version]",
		                                    new List<string> { "-assembly.version \"AssemblyInfo.cs\" \"1.0.2.1\"" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2 && args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string version = args[1];
			string filePattern = "AssemblyInfo.*";
			
			if (args.Length == 3)
			{
				version = args[2];
				filePattern = args[1];
			}

			string[] versionNumbers = version.Split('.');
			
			if (versionNumbers.Length != 4)
			{
				throw new ArgumentException("incorrect version format");
			}
			
			foreach (string number in versionNumbers)
			{
				int val;
				if (!int.TryParse(number, out val))
				{
					throw new ArgumentException("incorrect version format");
				}
			}

			Progress.Message = "Searching";
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			List<string> changedFiles = new List<string>();
			
			foreach (string file in files)
			{
				// [assembly: AssemblyVersion("1.0.1.85")]
				// <Assembly: AssemblyFileVersion("1.0")>
				if (IO.SearchAndReplaceInFile(file, @"(AssemblyFileVersion|AssemblyVersion)[\(][\""][^\""]*[\""][)]", @"$1(""" + version + @""")") > 0)
				{
					changedFiles.Add(file);
				}
			}
			
			ConsoleHelper.WriteLine("Version number updated in " + changedFiles.Count.ToString() + " location" + ((changedFiles.Count != 1) ? "s" : ""), ConsoleColor.DarkYellow);
			
			return Program.EXIT_OK;
		}
	}
}
