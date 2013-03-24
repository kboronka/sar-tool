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
	public class LabviewVersion : BaseCommand
	{
		public LabviewVersion() : base("Set LabVIEW project version number",
		                               new List<string> { "lv_ver" },
		                               "-lv_ver <lvproj_file> <version>",
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
			
			string filePattern = args[1];
			string[] version = args[2].Split('.');
			
			if (version.Length != 4)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string root = Directory.GetCurrentDirectory();
			
			/*
			<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">9</Property>
			<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">9</Property>
			<Property Name=\"TgtF_fileVersion.minor\" Type=\"Int\">9</Property>
			<Property Name=\"TgtF_fileVersion.patch\" Type=\"Int\">9</Property>
			 */
			
			List<string> changedFiles = new List<string>();
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.major\" Type=\"Int\">" + version[0] + "</Property>"));
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.minor\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.minor\" Type=\"Int\">" + version[1] + "</Property>"));
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.patch\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.patch\" Type=\"Int\">" + version[2] + "</Property>"));
			changedFiles.AddRange(IO.SearchAndReplaceInFiles(root, filePattern, "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">\\d{1,}</Property>", "<Property Name=\"TgtF_fileVersion.build\" Type=\"Int\">" + version[3] + "</Property>"));
			
			// remove duplicates
			changedFiles.Sort();
			Int32 index = 0;
			while (index < changedFiles.Count - 1)
			{
				if (changedFiles[index] == changedFiles[index + 1])
					changedFiles.RemoveAt(index);
				else
					index++;
			}


			if (changedFiles.Count > 0)
			{
				ConsoleHelper.WriteLine("Replacments made in the following file" + ((changedFiles.Count > 1) ? "s" : ""));
				foreach (string file in changedFiles)
				{
					ConsoleHelper.WriteLine(file.Replace(root, ""));
				}
			}
			else
			{
				ConsoleHelper.WriteLine("search string was not found");
			}
			
			return Program.EXIT_OK;
		}
	}
}
