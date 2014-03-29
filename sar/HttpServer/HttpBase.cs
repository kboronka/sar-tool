using System;

using sar.Tools;

namespace sar.HttpServer
{
	public class HttpBase
	{
		public HttpBase()
		{
		}
		
		#region logger
		
		FileLogger debugLog;
		ErrorLogger errorLogger;
		
		public FileLogger DebugLog
		{
			get { return this.debugLog; }
			private set { this.debugLog = value; }
		}
		
		public ErrorLogger ErrorLog
		{
			get { return this.errorLogger; }
			private set { this.errorLogger = value; }
		}
		
		protected void Log(string line)
		{
			if (this.debugLog == null) return;
			this.debugLog.WriteLine(this.ToString() + ": " + line);
		}
		
		protected void Log(Exception ex)
		{
			if (this.errorLogger == null) return;
			this.errorLogger.Write(ex);
		}
		
		#endregion		
	}
}
