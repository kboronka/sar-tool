
using System;

using sar.Tools;

namespace sar.Base
{
	public abstract class Program
	{
		#region logger
		
		private static FileLogger errorLog;
		public static FileLogger ErrorLog
		{
			get
			{
				if (Program.errorLog == null)
				{
					Program.errorLog = new FileLogger("error.log");
					Program.errorLog.LogTime = false;
				}
				
				return Program.errorLog;
			}
		}
		
		private static FileLogger debugLog;
		public static FileLogger DebugLog
		{
			get
			{
				if (Program.debugLog == null)
				{
					Program.debugLog = new FileLogger("debug.log");
					Program.errorLog.LogTime = true;
				}
				
				return Program.debugLog;
			}
		}
		
		public static void Log(Exception ex)
		{
			try
			{
				Program.ErrorLog.WriteLine(ex);
			}
			catch
			{
				
			}
		}
		
		public static void Log(string message)
		{
			try
			{
				Program.DebugLog.WriteLine(message);
			}
			catch
			{

			}
		}
		
		#endregion
	}
}
