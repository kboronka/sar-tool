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
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class FanucFTP : Command
	{
		public FanucFTP(Base.CommandHub parent) : base(parent, "FanucBackup",
		                                                  new List<string> { "fanuc.bk" },
		                                                  @"-fanuc.bk <ip:port> <path>",
		                                                  new List<string> { @"-fanuc.bk 192.168.0.2:21 C:\Temp" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			var ip = args[1];
			var path = args[2];
			
			if (path.EndsWith(@"\")) path = StringHelper.TrimEnd(path, 1);
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
			
			var files = FTPHelper.GetFileList(ip);
			for (int i = 0; i < files.Count; i++)
			{
				var file = files[i];
				var progress = (i / (double)files.Count) * 100;
				
				Progress.Message = "Downloading " + progress.ToString("0") + "% [" + file + "]";
				
				try
				{
					FTPHelper.DownloadFile(ip, file, path);
				}
				catch (Exception ex)
				{
					ConsoleHelper.WriteLine("Error downloading " + file);
					ConsoleHelper.WriteException(ex);
				}
			}
			
			ConsoleHelper.WriteLine(files.Count.ToString() + " Files" + ((files.Count != 1) ? "s" : "") + " Downloaded", ConsoleColor.DarkYellow);
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}