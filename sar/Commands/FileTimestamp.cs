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
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class FileTimestamp : Command
	{
		public FileTimestamp(Base.CommandHub parent) : base(parent, "File - Timestamp Name", 
		                                new List<string> { "file.timestamp", "f.t", "timestamp", "t" },
		                                @"-timestamp <FilePath> [date/time format]",
		                               new List<string> { "-timestamp backup.zip \"yyyy.MM.dd-HH.mm\"" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 2 || args.Length > 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string timestampFormat = "yyyy.MM.dd-HH.mm.ss";
			if (args.Length == 3)
			{
				timestampFormat = args[2];
			}
			
			string filepath = args[1];
			
			// original file must exits
			if (!File.Exists(filepath))
			{
				throw new FileNotFoundException("file not found. \"" + filepath + "\"");
			}
			
			string datetimestamp = DateTime.Now.ToString(timestampFormat);
			string fileExtension = filepath.Substring(filepath.LastIndexOf('.') + 1);
			string filepathNew = filepath.Substring(0, filepath.Length - fileExtension.Length - 1) + "." + datetimestamp + "." + fileExtension;
			
			if (File.Exists(filepathNew))
			{
				throw new FileLoadException("file already exists. \"" + filepathNew + "\"");
			}
			
			File.Move(filepath, filepathNew);
			
			ConsoleHelper.WriteLine("file renamed to " + filepathNew, ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}		
	}
}
