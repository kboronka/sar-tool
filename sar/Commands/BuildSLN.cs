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
	public class BuildSLN : Command
	{
		public BuildSLN(Base.CommandHub parent) : base(parent, "Build - .NET soultion",
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

			// -------------------------------------------------------------------------
			// find solution file
			// -------------------------------------------------------------------------
			Progress.Message = "Searching for soultion file";
			string filePattern = args[2];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			
			// sanity - no solution file found
			if (files.Count == 0) throw new FileNotFoundException(filePattern + " solution file not found");
			
			string soultionPath = files[0];
			string solutionFileName = IO.GetFilename(soultionPath);
			
			// sanity - solution file exists
			if (!File.Exists(soultionPath)) throw new FileNotFoundException(soultionPath + " solution file not found");
			
			// -------------------------------------------------------------------------
			// get list of msbuild versions available
			// -------------------------------------------------------------------------
			string netVersion = args[1];
			Progress.Message = "Locating Installed .NET versions";
			string msbuildPath = IO.FindDotNetFolder(netVersion);
			
			if (netVersion != "1.1" && soultionPath.EndsWith(".sln"))
			{
				msbuildPath += @"\MSBuild.exe";
			}
			else if (netVersion == "1.1" && solutionFileName.EndsWith(".sln"))
			{
				string devenv = Tools.IO.FindApplication("devenv.exe", "Microsoft Visual Studio .NET 2003");
				
				msbuildPath = devenv;
			}
			else if (netVersion == "1.1" && soultionPath.EndsWith(".vbproj"))
			{
				msbuildPath += @"\vbc.exe";
			}
			else if (netVersion == "1.1" && soultionPath.EndsWith(".csproj"))
			{
				msbuildPath += @"\vbc.exe";
			}
			else
			{
				throw new ApplicationException("unsupported project type");
			}
		

		string arguments = "\"" + soultionPath + "\"";
		
		for (int i = 3; i < args.Length; i++)
		{
			arguments += " " + args[i];
		}
		
		Progress.Message = "Building .NET Solution " + solutionFileName;
		
		string output;
		int exitcode = ConsoleHelper.Run(msbuildPath, arguments, out output);
		if (exitcode != 0)
		{
			ConsoleHelper.DebugWriteLine("exit code: " + exitcode.ToString());
			ConsoleHelper.WriteLine(output, ConsoleColor.DarkCyan);
			ConsoleHelper.WriteLine("Build Failed", ConsoleColor.DarkYellow);
			return exitcode;
		}
		else
		{
			ConsoleHelper.WriteLine("Build Successfully Completed", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
}
