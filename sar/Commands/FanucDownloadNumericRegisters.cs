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
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class FanucDownloadNumericRegisters : Command
	{
		private struct NumericRegister
		{
			public string Name;
			public int Number;
			public decimal value;
			
			public NumericRegister(Match match)
			{
				int i = 1;
				this.Number = int.Parse(match.Groups[i++].Value);
				this.value = decimal.Parse(match.Groups[i++].Value);
				this.Name = match.Groups[i++].Value;
			}
			
			public string ToCSVLine()
			{
				var output = "";
				output += Number.ToString() + ", ";
				output += Name + ", ";
				output += value;
								
				return output;
			}
		}
		
		public FanucDownloadNumericRegisters(Base.CommandHub parent) : base(parent, "Fanuc Download Numeric Registers",
		                                                                    new List<string> { "fanuc.DownloadNumericRegisters", "fanuc.downloadR" },
		                                                                    @"-fanuc.downloadPR <ip:port> <new file>",
		                                                                    new List<string> { @"-fanuc.downloadPR 192.168.0.1:21 numeric.csv" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			// TODO: add (date) (time) placeholder handlers
			
			string ip = args[1];
			string csv = args[2];
			string root = Directory.GetCurrentDirectory();

			IO.CheckRootAndPattern(ref root, ref csv);
			string filename = IO.GetFilename(csv);
			string filepath = root + filename;
			var files = FTPHelper.GetFileList(ip);
			
			if (!files.Contains("numreg.va")) throw new FileNotFoundException("numreg.va not found on ftp server");
			
			var input = StringHelper.GetString(FTPHelper.DownloadBytes(ip, "numreg.va"));
			
			if (File.Exists(filepath)) File.Delete(filepath);

			var registers = ExportRegisters(input, filepath);
			
			ConsoleHelper.WriteLine(registers.ToString() + " Registers" + ((registers != 1) ? "s" : "") + " Exported", ConsoleColor.DarkYellow);
			
			return ConsoleHelper.EXIT_OK;
		}
		
		public static int ExportRegisters(string input, string csv)
		{
			const string search = @"\[(\d*)\]\s=\s(-*\d*\.*\d*)\s*'([^']*)'";

			var positions = new List<NumericRegister>();
			var matches = Regex.Matches(input, search);
			var output = "r, description, value" + Environment.NewLine;
			
			foreach (Match match in matches)
			{
				if (match.Groups.Count == 3+1)
				{
					var position = new NumericRegister(match);
					positions.Add(position);
					
					output += position.ToCSVLine() + Environment.NewLine;
				}
			}
			
			IO.WriteFile(csv, output, Encoding.ASCII);
			return positions.Count;
		}
	}
}

