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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace sar.Tools
{
	public class FileLogger
	{
		private const string TIMESTAMP = "yyyy/MM/dd HH:mm:ss.fff";
		private const string FILETIMESTAMP = "yyyy-MM-dd";
		public const string ISO8601_TIMESTAMP = "yyyy-MM-ddTHH:mm:ssZ";
		
		private string path;
		private StreamWriter writer;
		private DateTime today;
		
		#region properties
		
		private StreamWriter LogWriter
		{
			get
			{
				if (this.today == null || (DateTime.Today != this.today) || (this.writer == null))
				{
					this.today = DateTime.Today;
					
					if (this.writer != null)
					{
						this.writer.Close();
					}
					
					string path = this.path.Insert(this.path.LastIndexOf('.'), "." + DateTime.Today.ToString(FILETIMESTAMP));
					string directory = IO.GetFileDirectory(path);
					
					if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
					
					this.writer = new StreamWriter(path, true);
				}
				
				return this.writer;
			}
		}
		
		#endregion
		
		public FileLogger(string filename)
		{
			if (String.IsNullOrEmpty(filename))
			{
				throw new ArgumentNullException("logging filename not specified");
			}
			
			if (filename[filename.Length - 1] == '\\')
			{
				filename = filename.Substring(0, this.path.Length - 1);
			}
			
			if (filename.IndexOf('.') == -1)
			{
				filename += ".log";
			}
			
			this.path = ApplicationInfo.CommonDataDirectory + filename;
		}
		
		public void WriteLine(string text, DateTime timestamp)
		{
			lock (this.LogWriter)
			{
				this.LogWriter.WriteLine(timestamp.ToString(TIMESTAMP) + "\t" + text);
				this.LogWriter.Flush();
			}
		}
		
		public void WriteLine(string text)
		{
			WriteLine(text, DateTime.Now);
		}
	}
}
