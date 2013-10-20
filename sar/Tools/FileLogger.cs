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
			this.LogWriter.WriteLine(timestamp.ToString(TIMESTAMP) + "\t" + text);
		}
		
		public void WriteLine(string text)
		{
			WriteLine(text, DateTime.Now);
		}
	}
}
