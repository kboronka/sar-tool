
using System;

namespace sar.Tools
{
	public class ErrorLogger : FileLogger
	{
		public ErrorLogger(string filename) : base(filename, false)
		{
		}
		
		public void Write(Exception ex)
		{
			base.WriteLine(ConsoleHelper.HR);
			base.WriteLine("Time: " + DateTime.Now.ToString());
			base.WriteLine("Error: " + ex.Message);
			base.WriteLine(ConsoleHelper.HR);
			base.WriteLine(ex.StackTrace);
			base.WriteLine("");
		}
	}
}
