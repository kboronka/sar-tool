/* Copyright (C) 2017 Kevin Boronka
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
		                                                            new List<string> { @"-svn.GetAssemblyVersion http://svnserver/trunk/Properties/AssemblyInfo.cs" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			string repo = args[1];
			
			Progress.Message = "Reading Assembly Version Number";
			var version = GetVersion(repo);
			
			if (!String.IsNullOrEmpty(version))
			{								
				ConsoleHelper.WriteLine(version, ConsoleColor.White);
				return ConsoleHelper.EXIT_OK;
			}
			else
			{
				return ConsoleHelper.EXIT_ERROR;
			}
		}

		public static string GetVersion(string repo)
		{
			// find svn executiable
			var svn = IO.FindApplication("svn.exe", @"TortoiseSVN\bin");
			if (!File.Exists(svn)) throw new ApplicationException("svn.exe not found");

			// create temp folder used to checkout all files from svn repo
			string tempFolder = Path.GetTempPath();
			Directory.CreateDirectory(tempFolder);
			var extension = IO.GetFileExtension(repo);
			
			var tempPath = Path.Combine(tempFolder, Guid.NewGuid().ToString() + "." + extension);


			ConsoleHelper.Run(svn, " export " + repo + @" """ + tempPath + @"""");
			
			var content = IO.ReadFile(tempPath);
			File.Delete(tempPath);

			const string pattern = @"AssemblyVersion\s*\(\""(.*)\""\)";
			
			var regex = new Regex(pattern);
			var match = regex.Match(content);
			
			if (match.Success)
			{
				var version = match.Groups[1].Value;				
				return version;
			}
			
			return null;
		}
	}
}
