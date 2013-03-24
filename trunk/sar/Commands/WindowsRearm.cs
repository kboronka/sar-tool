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
using System.Diagnostics;

namespace skylib.sar
{
	public class WindowsRearm : BaseCommand
	{
		public WindowsRearm() : base("Windows Activation Trial Rearm",
		                             new List<string> { "windows.rearm", "win.rearm", "w.r" },
		                             @"-windows.rearm",
		                             new List<string> { "-windows.rearm" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			if (Environment.OSVersion.Version.Major != 6)
			{
				throw new Exception("Windows 7 not detected");
			}
			
			if (Program.NoWarning || ConsoleHelper.Confirm("Rearm Activation? (y/n) "))
			{
				string token = IO.Windows + @"\ServiceProfiles\NetworkService\AppData\Roaming\Microsoft\SoftwareProtectionPlatform\tokens.dat";
				List<string> files = new List<string>();
				List<string> tempfiles = new List<string>();

				files.AddRange(Directory.GetFiles(IO.System32, "*.C7483456-A289-439d-8115-601632D005A0"));
				if (File.Exists(token)) files.Add(token);
				
				foreach (string file in files)
				{
					tempfiles.Add(IO.Temp + Guid.NewGuid().ToString() + ".temp");
				}
				
				if (files.Count != 3)
				{
					throw new FileNotFoundException("Files not found");
				}
				
				foreach (string file in files)
				{
					ConsoleHelper.WriteLine(file.Substring(file.LastIndexOf('\\')));
				}
				
				if (ConsoleHelper.Shell("net stop sppsvc") != Program.EXIT_OK) throw new Exception("failed to stop sppsvc");
				for (int i = 0; i < files.Count; i++) { File.Move(files[i], tempfiles[i]); }				
				if (ConsoleHelper.Shell("net start sppsvc") != Program.EXIT_OK) throw new Exception("failed to stop sppsvc");
				if (ConsoleHelper.Shell("slmgr /dlv") != Program.EXIT_OK) throw new Exception("failed to slmgr /dlv");
				if (ConsoleHelper.Shell("net stop sppsvc") != Program.EXIT_OK) throw new Exception("failed to stop sppsvc");
				for (int i = 0; i < files.Count; i++) { File.Move(tempfiles[i], files[i]); }
				ConsoleHelper.Shell("slmgr /ipk D4F6K-QK3RD-TMVMJ-BBMRX-3MBMV");
				
				ConsoleHelper.WriteLine("Rearmed - Reboot Required", ConsoleColor.DarkYellow);
				return Program.EXIT_OK;
			}
			
			return Program.EXIT_ERROR;
		}
	}
}
