using System;

namespace sar
{
	// event delegate
	public delegate void LoggerEventHandler(LoggerEventArgs e);

	public class LoggerEventArgs : EventArgs
	{
		public bool Exception { get; private set; }
		public string message { get; private set; }
		
		public LoggerEventArgs(Exception ex)
		{
			this.Exception = true;
			this.message = ex.Message;
		}
		
		public LoggerEventArgs(string message)
		{
			this.Exception = false;
			this.message = message;
		}
	}
}
