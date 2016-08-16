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
using System.Windows.Forms;
using System.IO;

using sar.Tools;

namespace sar.Tools
{
	public partial class LogForm : Form
	{
		private const int MAXLINES = 100;
		
		private bool updateFlag;
		private bool saveToFile;
		private string path;
		private StreamWriter writer;
		private DateTime today;
		private bool logTimestamp = true;
		
		private List<string> lines = new List<string>();
		
		#region properties
		
		private StreamWriter LogWriter
		{
			get
			{
				if ((DateTime.Today != this.today) || (this.writer == null))
				{
					if (this.writer != null)
					{
						this.writer.Close();
					}
					
					this.today = DateTime.Today;
					
					string path = this.path.Insert(this.path.LastIndexOf('.'), "." + this.today.ToString(FileLogger.FILETIMESTAMP));
					string directory = this.path.Substring(0, this.path.LastIndexOf('\\'));
					
					if (!Directory.Exists(directory))
						Directory.CreateDirectory(directory);
					
					this.writer = new StreamWriter(path, true);
				}
				
				return this.writer;
			}
		}
		
		public bool LogTimestamp
		{
			get { return logTimestamp; }
		}
		
		#endregion
		
		public LogForm()
		{
			InitializeComponent();
			this.updateDisplayTimer.Enabled = true;
			this.saveToFile = false;
		}
		
		public LogForm(string filename)
		{
			InitializeComponent();
			this.updateDisplayTimer.Enabled = true;
			
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
			this.saveToFile = true;
		}
		
		public void WriteLine(string text, DateTime timestamp)
		{
			if (this.logTimestamp) text = timestamp.ToString(FileLogger.TIMESTAMP) + "\t" + text;
			this.WriteLine(text);
		}
		
		public void WriteLine(string text)
		{
			updateFlag = true;
			lines.Add(text);
			
			if (this.saveToFile)
			{
				this.LogWriter.WriteLine(text);
			}
			
			if (lines.Count > MAXLINES)
			{
				lines.RemoveAt(0);
			}
		}
		
		public void UpdateDisplay()
		{
			string display = "";
			
			foreach (string line in lines)
			{
				display = line + Environment.NewLine + display;
			}
			
			this.textBox1.Text = display;
		}
		
		#region Events
		
		private void UpdateDisplayTimerTick(object sender, EventArgs e)
		{
			if (updateFlag)
			{
				//this.LogWriter.Flush();
				this.updateDisplayTimer.Enabled = false;
				this.UpdateDisplay();
				this.updateDisplayTimer.Enabled = true;
				updateFlag = false;
			}
			else
			{
				//this.LogWriter.Close();
				//this.writer = null;
			}
		}
		
		#endregion
		
	}
}