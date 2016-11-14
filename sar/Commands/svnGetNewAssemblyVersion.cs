/* Copyright (C) 2016 Kevin Boronka
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
	public class svnGetNewAssemblyVersion : Command
	{
		public svnGetNewAssemblyVersion(Base.CommandHub parent) : base(parent, "svn Get New Assembly Version",
		                                                            new List<string> { "svn.GetNewAssemblyVersion" },
		                                                            @"svn.GetNewAssemblyVersion <svn/path>",
		                                                            new List<string> { @"-svn.GetNewAssemblyVersion http://svnserver/trunk/Properties/AssemblyInfo.cs" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			Progress.Enabled = false;
			
			string repo = args[1];
			var version = svnGetAssemblyVersion.GetVersion(repo);

			if (!String.IsNullOrEmpty(version))
			{
				var build = IO.GetFileExtension(version);
				var newVersion = StringHelper.TrimEnd(version, build.Length);
				
				var newBuild = (int.Parse(build) + 1).ToString();
				newVersion += newBuild.ToString();
				
				ConsoleHelper.WriteLine(newVersion, ConsoleColor.White);
				return ConsoleHelper.EXIT_OK;
			}
			else
			{
				ConsoleHelper.WriteLine("0.0.0.0", ConsoleColor.Red);
				return ConsoleHelper.EXIT_ERROR;
			}
		}
	}
}
