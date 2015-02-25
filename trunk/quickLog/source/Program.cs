/*
 * Created by SharpDevelop.
 * User: kboronka
 * Date: 2015-02-23
 * Time: 6:32 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;

using sar.Tools;
using Base=sar.Base;

namespace quickLog
{
	class Program : Base.Program
	{
		public static int Main(string[] args)
		{
			try
			{
				Base.Program.LogInfo();
				ConsoleHelper.Start();
				
				ConsoleHelper.WriteLine("Hello", ConsoleColor.Magenta);
				
				// TODO: Implement Functionality Here
				Thread.Sleep(1500);
				
				Base.Program.ErrorLog.FlushFile();
				Base.Program.DebugLog.FlushFile();
				
				ConsoleHelper.Shutdown();
				return ConsoleHelper.EXIT_OK;
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteException(ex);

				Base.Program.ErrorLog.FlushFile();
				Base.Program.DebugLog.FlushFile();
				
				ConsoleHelper.Shutdown();
				return ConsoleHelper.EXIT_ERROR;
			}
		}
	}
}