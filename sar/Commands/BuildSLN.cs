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
	public class BuildSLN : BaseCommand
	{
		public BuildSLN() : base("Build - .NET soultion",
		                         new List<string> { "build.net", "b.net" },
		                         "-b.net [.net version] [solution_path] [msbuild arguments]",
		                         new List<string> { "-b.net 3.5 sar.sln /p:Configuration=Release /p:Platform=\"x86\"" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 3)
			{
				throw new ArgumentException("too few arguments");
			}
			
			Progress.Message = "Searching";
			string netVersion = args[1];
			string filepath = IO.FindFile(args[2]);
			string filename = IO.GetFilename(filepath);
			
			// get list of msbuild versions availble
			Progress.Message = "Locating Installed .NET versions";
			string msbuildFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + @"\..\Microsoft.NET\Framework";
			Dictionary<string, string> msBuildFolders = new Dictionary<string, string>();
			
			foreach (string path in Directory.GetDirectories(msbuildFolder))
			{
				string version = path.Remove(0,path.LastIndexOf('\\')+1).Substring(1,3);
				string msBuildPath = path + "\\MSBuild.exe";
				if (File.Exists(msBuildPath))
				{
					msBuildFolders.Add(version, msBuildPath);
					ConsoleHelper.DebugWriteLine(version + " = " + msBuildPath);
				}
			}
			
			// sanity - .net version installed
			if (!msBuildFolders.ContainsKey(netVersion)) throw new ArgumentOutOfRangeException(".net version");
			
			// sanity - solution file exists
			if (!File.Exists(filepath)) throw new FileNotFoundException(filepath + " solution file not found");
			
			string msbuildPath = msBuildFolders[netVersion];
			string arguments = "\"" + filepath + "\"";
			
			for (int i = 3; i < args.Length; i++)
			{
				arguments += " " + args[i];
			}
			
			Progress.Message = "Building .NET Solution " + filename;
			
			string output;
			int exitcode = ConsoleHelper.Shell(msbuildPath, arguments, out output);
			if (exitcode != 0)
			{
				ConsoleHelper.WriteLine("Build Failed", ConsoleColor.DarkYellow);
				ConsoleHelper.WriteLine(output, ConsoleColor.DarkCyan);
				ConsoleHelper.WriteLine("exit code: " + exitcode.ToString());
				return exitcode;
			}
			else
			{
				ConsoleHelper.WriteLine("Build Successfully Completed", ConsoleColor.DarkYellow);
				return Program.EXIT_OK;
			}
		}
	}
}
