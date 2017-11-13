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
using System.Text.RegularExpressions;


using sar.Base;
using sar.Tools;

namespace sar.Commands
{
	public class SvnUpdate : Command
	{
		public SvnUpdate(Base.CommandHub parent)
			: base(parent, "SVN check for uncommited files",
			       new List<string> { "svn.uc" },
			       @"svn.un",
			       new List<string> { @"-svn.uc" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			Progress.Enabled = false;
			
			// find svn executiable
			var svn = IO.FindApplication("svn.exe", @"TortoiseSVN\bin");
			if (!File.Exists(svn)) throw new ApplicationException("svn.exe not found");
			
			string result = "";
			ConsoleHelper.Run(svn, " status update", out result);
			
			// check for conflicts
			var match = Regex.Match(result, @"^C\s{4}.*$");
			
			if (match.Success)
			{
				ConsoleHelper.WriteLine("********************************************************", ConsoleColor.White);
				ConsoleHelper.Write("* ", ConsoleColor.White);
				ConsoleHelper.WriteLine("SVN merge conflicts detected.  Manual merge required", ConsoleColor.Red);
				ConsoleHelper.WriteLine("********************************************************", ConsoleColor.White);
				return ConsoleHelper.EXIT_ERROR;
			}
			else
			{
				ConsoleHelper.WriteLine("svn update completed", ConsoleColor.Green);
				return ConsoleHelper.EXIT_OK;
			}
		}
	}
}
