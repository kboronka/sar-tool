/*
 * Created by SharpDevelop.
 * User: kboronka
 * Date: 11/7/2012
 * Time: 12:06 PM
 */
using System;
using System.Timers;
using skylib.Tools;

namespace skylib.sar
{
	public class Progress
	{
		public static Timer timer;
		private static int i = 0;
		
		public Progress()
		{
			timer = new Timer();
			timer.Elapsed += new ElapsedEventHandler(Update);
			Progress.i = 0;
		}
		
		static void Update(object sender, ElapsedEventArgs e)
		{
			
			if (++Progress.i >= 6) Progress.i = 0;
			
			ConsoleHelper.Write("running" + new String('.', i) + new String(' ', 6 - i) + "\r", ConsoleColor.Cyan);
		}
		
		public void DoWork()
		{
			timer.Enabled = true;
			
			/*
			while(true)
			{
				for (int i = 0; i < 6; i++)
				{
					ConsoleHelper.Write("running" + new String('.', i) + new String(' ', 6 - i) + "\r", ConsoleColor.Cyan);
					Thread.Sleep(100);
				}
			}
			*/
		}
	}
}
