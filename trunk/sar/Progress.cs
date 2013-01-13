/*
 * Created by SharpDevelop.
 * User: kboronka
 * Date: 11/7/2012
 * Time: 12:06 PM
 */
using System;
using System.Threading;

namespace skylib.sar
{
	public class Progress
	{
		public Progress()
		{

		}
		
		public void DoWork()
		{
			char[] sequence = {'-', '\\', '|', '/'};
			while(true)
			{
				for (int i = 0; i < 6; i++)
				{
					Console.ForegroundColor = ConsoleColor.Cyan;
					//Console.Write(sequence[i] + "\r");
					Console.Write("running" + new String('.', i) + new String(' ', 6 - i) + "\r");
					Console.ResetColor();
					Thread.Sleep(100);
				}
			}
		}
	}
}
