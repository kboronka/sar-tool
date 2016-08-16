using System;

namespace sar
{
	public static class Logger
	{
		public static void Log(Exception ex)
		{
			Logger.Log(ex);
		}
		
		public static void Log(string message)
		{
			Logger.Log(message);
		}
		
		public static void FlushLogs()
		{
			Base.Program.FlushLogs();
		}
	}
}
