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

namespace skylib.sar
{
	public class TimestampFilename : BaseCommand
	{
		public TimestampFilename() : base("Timestamp Filename", 
		                                new List<string> { "file.timestamp", "f.t", "timestamp", "t" },
		                                @"-timestamp <FilePath> [date/time format]",
		                               new List<string> { "-timestamp backup.zip \"yyyy.MM.dd-HH.mm\"" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 2 ||args.Length > 3)
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
			
			return Program.EXIT_OK;
		}		
	}
}
