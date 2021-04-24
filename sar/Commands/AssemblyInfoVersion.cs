﻿/* Copyright (C) 2017 Kevin Boronka
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
	public class AssemblyInfoVersion : Command
	{
		public AssemblyInfoVersion(Base.CommandHub parent) : base(parent, "Set AssemblyInfo version number",
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

			// TODO: handle no version changes
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
			ConsoleHelper.DebugWriteLine("currentDir = " + root);

			IO.CheckRootAndPattern(ref root, ref filePattern);
			ConsoleHelper.DebugWriteLine("root = " + root);
			ConsoleHelper.DebugWriteLine("filePattern = " + filePattern);

			var files = IO.GetAllFiles(root, filePattern);
			var changedFiles = new List<string>();

			foreach (string file in files)
			{
				// [assembly: AssemblyVersion("1.0.1.85")]
				// <Assembly: AssemblyFileVersion("1.0")>
				// [assembly: AssemblyVersion ("1.1.0.5")]
				if (IO.SearchAndReplaceInFile(file, @"(AssemblyFileVersion|AssemblyVersion)\s*[\(][\""][^\""]*[\""][)]", @"$1(""" + version + @""")").Count > 0)
				{
					changedFiles.Add(file);
				}
			}

			ConsoleHelper.WriteLine("Version number updated in " + changedFiles.Count.ToString() + " location" + ((changedFiles.Count != 1) ? "s" : ""), ConsoleColor.DarkYellow);

			return ConsoleHelper.EXIT_OK;
		}
	}
}
