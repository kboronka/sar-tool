/* Copyright (C) 2015 Kevin Boronka
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
using System.Diagnostics;
using System.Threading;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class WindowsRearm : Command
	{
		public WindowsRearm(Base.CommandHub parent) : base(parent, "Windows - Activation Trial Rearm",
		                             new List<string> { "windows.rearm", "win.rearm", "w.rarm" },
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
			
			if (this.commandHub.NoWarning || ConsoleHelper.Confirm("Caution: Rearm Activation? (y/n) "))
			{
				/*
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
				
				#if DEBUG
				foreach (string file in files)
				{
					ConsoleHelper.DebugWriteLine(file.Substring(file.LastIndexOf('\\') + 1));
				}
				#endif
				
				try
				{
					if (ConsoleHelper.Shell("net stop sppsvc") != ConsoleHelper.EXIT_OK) throw new Exception("failed to stop sppsvc");
				}
				catch (Exception ex)
				{
					ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
				}
				
				Thread.Sleep(1000);
				
				for (int i = 0; i < files.Count; i++)
				{
					try
					{
						ConsoleHelper.WriteLine(files[i], ConsoleColor.Yellow);
						File.Copy(files[i], tempfiles[i]);
						File.Delete(files[i]);
					}
					catch (Exception ex)
					{
						ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
					}
				}
				
				try
				{
					if (ConsoleHelper.Shell("net start sppsvc") != ConsoleHelper.EXIT_OK) throw new Exception("failed to stop sppsvc");
				}
				catch (Exception ex)
				{
					ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
				}
				
				
				if (ConsoleHelper.Shell("slmgr /dlv") != ConsoleHelper.EXIT_OK) throw new Exception("failed to slmgr /dlv");
				
				try
				{
					if (ConsoleHelper.Shell("net stop sppsvc") != ConsoleHelper.EXIT_OK) throw new Exception("failed to stop sppsvc");
				}
				catch (Exception ex)
				{
					ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
				}
				
				for (int i = 0; i < files.Count; i++)
				{
					try
					{
						File.Move(tempfiles[i], files[i]);
					}
					catch (Exception ex)
					{
						ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
					}
				}

				
				ConsoleHelper.Shell("slmgr /ipk D4F6K-QK3RD-TMVMJ-BBMRX-3MBMV");
				*/
				
				Progress.Message = "Rearming Windows Activation";
				ConsoleHelper.Run("slmgr /rearm");
				ConsoleHelper.WriteLine("Rearmed - Reboot Required", ConsoleColor.DarkYellow);
				return ConsoleHelper.EXIT_OK;
			}
			
			return ConsoleHelper.EXIT_ERROR;
		}
	}
}
