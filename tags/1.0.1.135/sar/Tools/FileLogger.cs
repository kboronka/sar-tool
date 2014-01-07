/* Copyright (C) 2014 Kevin Boronka
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
		
		private string root;
		private string filename;
		
		private StreamWriter writer;
		private DateTime today;
		private bool logTime = true;
		
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
						this.writer.Flush();
						this.writer.Close();
					}
					
					string path = this.root + DateTime.Today.ToString(FILETIMESTAMP) + "." + this.filename;
					string directory = IO.GetFileDirectory(path);
					
					if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
					
					this.writer = new StreamWriter(path, true);
				}
				
				return this.writer;
			}
		}
		
		public bool LogTime
		{
			set { this.logTime = value; }
		}
		
		#endregion
		
		public FileLogger(string filename)
		{
			this.filename = filename;
			
			// no filename
			if (String.IsNullOrEmpty(filename)) this.filename = AssemblyInfo.Name + ".log";
			
			// no file extension
			if (this.filename.IndexOf('.') == -1) this.filename += ".log";

			// root = C:\ProgramData\Company\Product
			this.root = ApplicationInfo.CommonDataDirectory;
		}
		
		public void WriteLine(Exception ex)
		{
			bool logTimeSetting = this.logTime;
			
			this.logTime = false;
			
			this.WriteLine(ConsoleHelper.HR);
			this.WriteLine("Time: " + DateTime.Now.ToString());
			this.WriteLine("Error: " + ex.Message);
			this.WriteLine(ConsoleHelper.HR);
			this.WriteLine(ex.StackTrace);
			this.WriteLine("");
			this.logTime = logTimeSetting;
		}
		
		public void WriteLine(string text, DateTime timestamp)
		{
			lock (this.LogWriter)
			{
				if (this.logTime) text = timestamp.ToString(TIMESTAMP) + "\t" + text;
				this.LogWriter.WriteLine(text);
				this.LogWriter.Flush();
			}
		}
		
		public void WriteLine(string text)
		{
			WriteLine(text, DateTime.Now);
		}
	}
}
