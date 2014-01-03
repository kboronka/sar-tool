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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

using sar.Commands;
using sar.Base;
using sar.Tools;


// binary download: https://sar-tool.googlecode.com/svn/trunk/release/sar.exe
// release download: https://sar-tool.googlecode.com/svn/tags/

namespace sar
{
	public class Program
	{
		public static int Main(string[] args)
		{
			//FIXME: no error handling here

			CommandHub hub = new CommandHub();
			ConsoleHelper.Start();
			if (args.Length == 0) Help.WriteTitle();
			
			try
			{
				// process command line arguments
				bool commandlineActive = false;
				int exitCode = ConsoleHelper.EXIT_OK;
				
				while (!commandlineActive)
				{
					try
					{
						if (args.Length == 0)
						{
							commandlineActive = false;
							args = new string[1];
							ConsoleHelper.Write("> ", ConsoleColor.White);
							
							args = StringHelper.ParseString(ConsoleHelper.ReadLine(), " ");
							args = hub.RemoveGlobalArgs(args);
							
							if (args.Length == 0)
							{
								throw new ArgumentException("too few arguments");
							}
						}
						else
						{
							args = hub.RemoveGlobalArgs(args);
							commandlineActive = true;
						}
						
						string command = args[0].ToLower();
						if (command[0] == '-' || command[0] == '/')
						{
							command = command.Substring(1);
						}
						
						// Execute Command
						if (command != "exit")
						{
							Progress.UpdateTimer.Enabled = true;
							exitCode = hub.Execute(command, args);
							Progress.UpdateTimer.Enabled = false;
							args = new string[0];
						}
					}
					catch (Exception ex)
					{
						try
						{
							Progress.UpdateTimer.Enabled = false;
							//backgroundThread.Abort();
							//Progress.UpdateTimer.Enabled = false;

							ConsoleHelper.WriteException(ex);

							if (hub.Debug && commandlineActive)
							{
								Thread.Sleep(2000);
							}
							
							args = new string[0];
						}
						catch
						{
							
						}
						
						exitCode = ConsoleHelper.EXIT_ERROR;
					}
				}
				
				Progress.UpdateTimer.Enabled = false;
				//backgroundThread.Abort();

				ConsoleHelper.Shutdown();
				return exitCode;
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteException(ex);
				Thread.Sleep(2000);

				Progress.UpdateTimer.Enabled = false;
				//backgroundThread.Abort();
				
				if (hub.Debug)
				{
					ConsoleHelper.WriteLine("Press anykey to continue", ConsoleColor.Yellow);
					ConsoleHelper.ReadKey();
				}
				
				ConsoleHelper.Shutdown();
				return ConsoleHelper.EXIT_ERROR;
			}
		}
	}
}