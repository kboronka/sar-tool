/* Copyright (C) 2013 Kevin Boronka
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
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace sar.Tools
{
	public struct ShellResults
	{
		public int ExitCode;
		public string Output;
		public string Error;
		public long ElapsedMilliseconds;
		public string Filename;
		public string Arguments;
		public string WorkingPath;
	}
	
	public class Shell
	{
		public const int EXIT_OK = 0;
		public const int EXIT_ERROR = 1;
		
		public Shell()
		{
		}
		
		public static ShellResults Run(string applicationFilePath)
		{
			return 	Run(applicationFilePath, "");
		}
		
		public static ShellResults Run(string applicationFilePath, string arguments)
		{
			return 	Run(applicationFilePath, arguments, Directory.GetCurrentDirectory());
		}		
		
		public static ShellResults Run(string applicationFilePath, string arguments, string workingDirectory)
		{
			if (String.IsNullOrEmpty(applicationFilePath))
			{
				throw new NullReferenceException("application filename was not specified");
			}
			
			if (String.IsNullOrEmpty(arguments))
			{
				arguments = "";
			}
			
			if (string.IsNullOrEmpty(workingDirectory))
			{
				throw new NullReferenceException("working directory was not specified");
			}
			
			if (!File.Exists(applicationFilePath))
			{
				throw new FileNotFoundException("application filename not found");
			}
			
			if (!Directory.Exists(workingDirectory))
			{
				throw new DirectoryNotFoundException("working directory not found");
			}
			
			
			Stopwatch applicationExecutionTime = new Stopwatch();
			applicationExecutionTime.Start();
			
			ShellResults results = new ShellResults();
			results.Filename = applicationFilePath;
			results.Arguments = arguments;
			results.WorkingPath = workingDirectory;
			
			Process shell = new Process();
			shell.StartInfo.FileName = applicationFilePath;
			shell.StartInfo.Arguments = arguments;
			shell.StartInfo.UseShellExecute = false;
			shell.StartInfo.RedirectStandardOutput = true;
			shell.StartInfo.RedirectStandardError = true;
			shell.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			shell.StartInfo.CreateNoWindow = true;
			shell.StartInfo.WorkingDirectory = workingDirectory;
			
			shell.Start();
			results.Output = shell.StandardOutput.ReadToEnd();
			results.Error = shell.StandardError.ReadToEnd();
			shell.WaitForExit();
			
			results.ExitCode = shell.ExitCode;
			applicationExecutionTime.Stop();
			results.ElapsedMilliseconds = applicationExecutionTime.ElapsedMilliseconds;
			return results;
		}
	}
}
