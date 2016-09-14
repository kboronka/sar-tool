/* Copyright (C) 2016 Kevin Boronka
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
	public class FanucDownloadPositions : Command
	{
		public FanucDownloadPositions(Base.CommandHub parent) : base(parent, "Fanuc Download Positions",
		                                                          new List<string> { "fanuc.DownloadPositions", "fanuc.downloadPR" },
		                                                          @"-fanuc.downloadPR <ip:port> <new file>",
		                                                          new List<string> { @"-fanuc.downloadPR 192.168.0.1:21 posreg.csv" })
		{
		}

		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string ip = args[1];
			string csv = args[2];
			string root = Directory.GetCurrentDirectory();

			IO.CheckRootAndPattern(ref root, ref csv);			
			string filename = IO.GetFilename(csv);
			string filepath = root + filename;
			var files = FTPHelper.GetFileList(ip);
			
			if (!files.Contains("posreg.va")) throw new FileNotFoundException("posreg.va not found on ftp server");
			
			var input = StringHelper.GetString(FTPHelper.DownloadBytes(ip, "posreg.va"));
			
				
			if (File.Exists(filepath)) File.Delete(filepath);

			var positions = FanucPositionsToCSV.ExportPositions(input, filepath);
			
			ConsoleHelper.WriteLine(positions.ToString() + " Position" + ((positions != 1) ? "s" : "") + " Exported", ConsoleColor.DarkYellow);
			
			return ConsoleHelper.EXIT_OK;
		}
		
		
	}
}

