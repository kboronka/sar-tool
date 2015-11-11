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
using System.IO;
using System.Threading;

namespace sar.Tools
{
	public class FileLogger
	{
		public const string DATETIMESTAMP = "yyyy/MM/dd HH:mm:ss.fff";
		public const string TIMESTAMP = "HH:mm:ss.fff";
		public const string FILETIMESTAMP = "yyyy-MM-dd";
		public const string ISO8601_TIMESTAMP = "yyyy-MM-ddTHH:mm:ss.fff";
		
		private string root;
		private string filename;
		
		private StreamWriter writer;
		private DateTime today;
		private bool logTime;
		private bool printSeperator = true;
		
		#region properties
		
		public bool LogTime
		{
			get { return this.logTime; }
			set { this.logTime = value; }
		}
		
		#endregion
		
		public FileLogger(string filename, bool logTimstamp)
		{
			try
			{
				this.filename = filename;
				this.logTime = logTimstamp;
				
				// no filename
				if (String.IsNullOrEmpty(filename)) this.filename = AssemblyInfo.Name + ".log";
				
				// no file extension
				if (!this.filename.EndsWith(".log")) this.filename += ".log";

				// root = C:\ProgramData\Company\Product
				this.root = ApplicationInfo.CommonDataDirectory;
				if (!Directory.Exists(this.root)) Directory.CreateDirectory(this.root);
				
				string path = this.root + DateTime.Today.ToString(FILETIMESTAMP) + "." + this.filename;
				if (!File.Exists(path)) this.printSeperator = false;
				
				this.writer = new StreamWriter(path, true);
				
				this.fileFlush = new Thread(this.FlushLoop);
				this.fileFlush.IsBackground = true;
				this.fileFlush.Priority = ThreadPriority.Lowest;
				this.fileFlush.Start();
				
				this.deleteOld = new Thread(this.DeleteLoop);
				this.deleteOld.IsBackground = true;
				this.deleteOld.Priority = ThreadPriority.Lowest;
				this.deleteOld.Start();					
			}
			catch
			{
				
			}
		}
		
		~FileLogger()
		{
			try
			{
				this.writer.WriteLine("disposing file logger");
				this.writer.Flush();
				this.writer.Close();
				flushLoopShutdown = true;
				deleteLoopShutdown = true;
			}
			catch
			{
				
			}
		}
		
		public void WriteLine(string text, DateTime timestamp)
		{
			lock (this.root)
			{
				if (this.writer == null) return;
				
				lock (this.writer)
				{
					if (this.today == null || (DateTime.Today != this.today))
					{
						this.writer.Flush();
						this.writer.Close();
						
						string path = this.root + DateTime.Today.ToString(FILETIMESTAMP) + "." + this.filename;
						if (!File.Exists(path)) this.printSeperator = false;
						
						this.writer = new StreamWriter(path, true);
						this.today = DateTime.Today;
					}
					
					if (this.printSeperator) this.writer.WriteLine(ConsoleHelper.HR);
					if (this.logTime) text = timestamp.ToString(TIMESTAMP) + "\t" + text;
					this.writer.WriteLine(text);
					this.printSeperator = false;
				}
			}
		}
		
		public void WriteLine(string text)
		{
			WriteLine(text, DateTime.Now);
		}
		
		#region service
		
		#region flush
		
		private Thread fileFlush;
		private bool flushLoopShutdown;
		
		private void FlushLoop()
		{
			Thread.Sleep(1000);
			
			while (!flushLoopShutdown)
			{
				try
				{
					this.FlushFile();
					Thread.Sleep(500);
				}
				catch
				{
					Thread.Sleep(5000);
				}
			}
		}

		internal void FlushFile()
		{
			lock (this.writer)
			{
				this.writer.Flush();
			}
		}
		
		#endregion
		
		#region delete old
		
		private Thread deleteOld;
		private bool deleteLoopShutdown;
		
		private void DeleteLoop()
		{
			Thread.Sleep(10000);
			
			while (!deleteLoopShutdown)
			{
				try
				{
					this.DeleteOldFiles();
					Thread.Sleep(50000);
				}
				catch
				{
					Thread.Sleep(5000);
				}
			}
		}

		internal void DeleteOldFiles()
		{
			string filePattern = "*." + this.filename;
			string root = this.root;
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);

			foreach (string file in files)
			{
				try
				{
					if (File.GetLastWriteTime(file) < DateTime.Now.AddDays(-3))
					{
						File.Delete(file);
					}
				}
				catch
				{
					
				}
			}
		}
		
		#endregion
		
		#endregion
	}
}
