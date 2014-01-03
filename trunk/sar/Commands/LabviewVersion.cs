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
	public class LabviewVersion : BaseCommand
	{
		public LabviewVersion(CommandHubBase commandHub) : base(commandHub, "Set LabVIEW project version number",
		                               new List<string> { "lv_ver" },
		                               "-lv_ver [lvproj_file] [version]",
		                               new List<string> { "-lv_ver \"*.lvproj_file\" \"1.0.2.1\"" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string[] version = args[2].Split('.');
			
			if (version.Length != 4)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Searching";
			string filePattern = args[1];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			ConsoleHelper.DebugWriteLine("pattern: " + filePattern);
			ConsoleHelper.DebugWriteLine("root: " + root);
			if (files.Count == 0) throw new FileNotFoundException("unable to find any files that match pattern: \"" + filePattern + "\" in root: \"" + root + "\"");

			
			int changes = 0;
			int counter = 0;
			foreach (string file in files)
			{
				if (this.commandHub.IncludeSVN || !IO.IsSVN(file))
				{
					counter++;
					changes += IO.SearchAndReplaceInFile(file, "<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">" + version[0] + "</Property>");
					changes += IO.SearchAndReplaceInFile(file, "<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">" + version[0] + "</Property>");
					changes += IO.SearchAndReplaceInFile(file, "<Property Name=\"TgtF_fileVersion.minor\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.minor\" Type=\"Int\">" + version[1] + "</Property>");
					changes += IO.SearchAndReplaceInFile(file, "<Property Name=\"TgtF_fileVersion.patch\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.patch\" Type=\"Int\">" + version[2] + "</Property>");
					changes += IO.SearchAndReplaceInFile(file, "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">" + version[3] + "</Property>");
				}
			}
			
			// remove duplicates


			if (counter > 0)
			{
				ConsoleHelper.WriteLine("LabVIEW project version number updated", ConsoleColor.DarkYellow);
			}
			else
			{
				ConsoleHelper.WriteLine("LabVIEW project version number not updated", ConsoleColor.DarkYellow);
			}
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}
