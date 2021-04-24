/* Copyright (C) 2021 Kevin Boronka
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

using sar.Base;
using sar.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;


namespace sar.Commands
{
	public class Bower : Command
	{
		public Bower(Base.CommandHub parent) : base(parent, "Bower Update",
													new List<string> { "bower" },
													"-bower",
													new List<string> { @"-bower" })
		{

		}

		public override int Execute(string[] args)
		{
			Progress.Message = "bower updating";

			var nodejs = IO.FindApplication("node.exe", "nodejs");
			var bower = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\npm\node_modules\bower\bin\bower";

			if (!File.Exists(bower))
				throw new ApplicationException("Bower not found");

			var p = new Process();
			p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
			p.StartInfo.FileName = nodejs;
			p.StartInfo.Arguments = bower + " update";
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.CreateNoWindow = true;
			p.OutputDataReceived += new DataReceivedEventHandler((s, e) => { ParseBowerOutput(e.Data); });
			p.Start();
			p.BeginOutputReadLine();
			p.WaitForExit();
			//ConsoleHelper.WriteLine(p.StandardOutput.ReadToEnd());
			//ConsoleHelper.WriteLine("error: " + p.StandardError.ReadToEnd());


			//ConsoleHelper.Run(nodejs, bower + " cache clean");
			if (p.ExitCode == ConsoleHelper.EXIT_OK)
			{
				ConsoleHelper.WriteLine("Bower update was successfully completed", ConsoleColor.DarkYellow);
				return ConsoleHelper.EXIT_OK;
			}

			ConsoleHelper.WriteLine("Bower failed to update", ConsoleColor.Red);
			return ConsoleHelper.EXIT_ERROR;
		}

		public static void ParseBowerOutput(string data)
		{
			if (String.IsNullOrEmpty(data))
				return;

			var pattern = @"bower\s([^\s]*)\s+([^\s]*)\s";
			var match = Regex.Match(data, pattern);

			if (match.Success)
			{
				var groups = match.Groups;

				if (groups.Count == 3)
				{
					Progress.Message = "bower updating [" + groups[1].Value + "]";

#if DEBUG
					/*
					ConsoleHelper.Write("bower ", ConsoleColor.Gray);
					ConsoleHelper.Write(groups[1].Value + " ", ConsoleColor.Green);
					ConsoleHelper.Write(groups[2].Value, ConsoleColor.DarkGray);
					ConsoleHelper.WriteLine();
					*/
#endif
				}
			}
		}
	}
}
