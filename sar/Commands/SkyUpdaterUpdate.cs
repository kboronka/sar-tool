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

namespace sar.Tools
{
	public class SkyUpdaterUpdate : BaseCommand
	{
		public SkyUpdaterUpdate(): base("SkyUpdater - Check for new version",
		                            new List<string> { "sky.update" },
		                            "-sky.update",
		                            new List<string> { "-sky.update" })
		{

		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Checking for updates";
			SkyUpdater updater = SkyUpdater.OpenURL(@"https://sar-tool.googlecode.com/svn/trunk/SkyUpdate.info");

			if (AssemblyInfo.Version != updater.Version)
			{
				ConsoleHelper.WriteLine("Updates Available", ConsoleColor.DarkYellow);
			}
			else
			{
				ConsoleHelper.WriteLine("No Updates Available", ConsoleColor.DarkYellow);
			}
			
			
			return Program.EXIT_OK;
		}
	}
}
