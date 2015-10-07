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
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class FanucPositionsToCSV : Command
	{
		public FanucPositionsToCSV(Base.CommandHub parent) : base(parent, "Fanuc Export Positions",
		                                                          new List<string> { "fanuc.ExportPositions" },
		                                                          @"-fanuc.ExportPositions <path to posreg.va>",
		                                                          new List<string> { @"-fanuc.ExportPositions C:\Temp" })
		{
		}
		
		private struct PositionRegister
		{
			public string Name;
			public int Number;
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
			
			var file = args[1];
			if (!File.Exists(file)) throw new FileNotFoundException("file not found");

			var positions = new List<PositionRegister>();
			var input = IO.ReadFile(file);
			var output = "pr, description, x, y, z, w, p, r" + Environment.NewLine;
			var search = @"\s*\[1,(\d*)] = \s*'([^\']*)'\s*Group:[^X]*X:\s*([^\s]*)\s*Y:\s*([^\s]*)\s*Z:\s*([^\s]*)\s*W:\s*([^\s]*)\s*P:\s*([^\s]*)\s*R:\s*([^\s]*)";
			
			var matches = Regex.Matches(input, search);
			
			foreach (Match match in matches)
			{
				if (match.Groups.Count == 9)
				{
					var position = new PositionRegister(match);
					positions.Add(position);
					
					output += position.ToCSVLine() + Environment.NewLine;
				}
			}
			

			IO.WriteFile(IO.GetFileDirectory(file) + @"\" +"posreg.csv", output, Encoding.ASCII);
			
			ConsoleHelper.WriteLine(positions.Count.ToString() + " Position" + ((positions.Count != 1) ? "s" : "") + " Exported", ConsoleColor.DarkYellow);
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}
