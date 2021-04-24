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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace sar.Commands
{
	public class FanucPositionsToCSV : Command
	{
		public FanucPositionsToCSV(Base.CommandHub parent) : base(parent, "Fanuc Export Positions",
																  new List<string> { "fanuc.ExportPositions", "fanuc.ExportPR" },
																  @"-fanuc.ExportPR <path to posreg.va>",
																  new List<string> { @"-fanuc.ExportPositions C:\Temp" })
		{
		}

		private struct PositionRegister
		{
			public string Name;
			public int Number;
			public string Configuration;
			public double x;
			public double y;
			public double z;
			public double w;
			public double p;
			public double r;

			public PositionRegister(Match match)
			{
				int i = 1;
				this.Number = int.Parse(match.Groups[i++].Value);
				this.Name = match.Groups[i++].Value;
				this.Configuration = match.Groups[i++].Value;
				this.x = double.Parse(match.Groups[i++].Value);
				this.y = double.Parse(match.Groups[i++].Value);
				this.z = double.Parse(match.Groups[i++].Value);
				this.w = double.Parse(match.Groups[i++].Value);
				this.p = double.Parse(match.Groups[i++].Value);
				this.r = double.Parse(match.Groups[i++].Value);
			}

			public string ToCSVLine()
			{
				var output = "";
				output += Number.ToString() + ", ";
				output += Name + ", ";
				output += Configuration + ", ";
				output += x.ToString() + ", ";
				output += y.ToString() + ", ";
				output += z.ToString() + ", ";
				output += w.ToString() + ", ";
				output += p.ToString() + ", ";
				output += r.ToString();

				return output;
			}
		}

		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("incorrect number of arguments");
			}

			string filePattern = args[1];
			string root = Directory.GetCurrentDirectory();

			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);

			if (files.Count == 0)
				throw new FileNotFoundException("file not found");

			var input = IO.ReadFile(files[0]);
			var positions = ExportPositions(input, IO.GetFileDirectory(files[0]) + @"\" + "posreg.csv");

			ConsoleHelper.WriteLine(positions.ToString() + " Position" + ((positions != 1) ? "s" : "") + " Exported", ConsoleColor.DarkYellow);

			return ConsoleHelper.EXIT_OK;
		}

		public static int ExportPositions(string input, string csv)
		{
			const string search = @"\s*\[1,(\d*)] = \s*'([^\']*)'\s*Group:\s\d*\s*Config:\s\d*([^,]*)[^X]*X:\s*([^\s]*)\s*Y:\s*([^\s]*)\s*Z:\s*([^\s]*)\s*W:\s*([^\s]*)\s*P:\s*([^\s]*)\s*R:\s*([^\s]*)";

			var positions = new List<PositionRegister>();
			var matches = Regex.Matches(input, search);
			var output = "pr, description, config, x, y, z, w, p, r" + Environment.NewLine;

			foreach (Match match in matches)
			{
				if (match.Groups.Count == 10)
				{
					var position = new PositionRegister(match);
					positions.Add(position);

					output += position.ToCSVLine() + Environment.NewLine;
				}
			}

			// create the folder if it's missing
			var directory = IO.GetFileDirectory(csv);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			IO.WriteFile(csv, output, Encoding.ASCII);
			return positions.Count;
		}
	}
}
