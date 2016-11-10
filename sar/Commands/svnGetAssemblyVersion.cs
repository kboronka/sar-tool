﻿/* Copyright (C) 2016 Kevin Boronka
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
using System.Text;
using System.Text.RegularExpressions;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class svnGetAssemblyVersion : Command
	{
		public svnGetAssemblyVersion(Base.CommandHub parent) : base(parent, "svn Get Assembly Version",
		                                                            new List<string> { "svn.GetAssemblyVersion" },
		                                                            @"svn.GetAssemblyVersion <svn/path>",
		                                                            new List<string> { @"-svn.AddExternals http://svnserver/trunk/Properties/AssemblyInfo.cs" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			string repo = args[1];
			
			// find svn executiable
			Progress.Message = "finding svn.exe";
			var svn = IO.FindApplication("svn.exe", @"TortoiseSVN\bin");
			if (!File.Exists(svn)) throw new ApplicationException("svn.exe not found");

			// create temp folder used to checkout all files from svn repo
			string tempFolder = Path.GetTempPath();
			Directory.CreateDirectory(tempFolder);
			var extension = IO.GetFileExtension(repo);
			
			var tempPath = Path.Combine(tempFolder, Guid.NewGuid().ToString() + "." + extension);


			// svn checkout --depth empty http://svnserver/trunk/proj
			Progress.Message = "exporting assembly file";
			ConsoleHelper.Run(svn, " export " + repo + @" """ + tempPath + @"""");
			
			Progress.Message = "getting version number";
			var content = IO.ReadFile(tempPath);
			File.Delete(tempPath);

			var pattern = @"AssemblyVersion\s*\(\""(.*)\""\)";
			
			var regex = new Regex(pattern);
			var match = regex.Match(content);
			
			if (match.Success)
			{
				var version = match.Groups[1].Value;
				
				ConsoleHelper.WriteLine(version, ConsoleColor.White);
				return ConsoleHelper.EXIT_OK;
			}
			else
			{
				return ConsoleHelper.EXIT_ERROR;
			}
		}
	}
}