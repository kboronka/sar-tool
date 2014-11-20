using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

using sar.Tools;

namespace sarValidate
{
	class Program : sar.Base.Program
	{
		public static int Main(string[] args)
		{
			try
			{
				Program.LogInfo();
				
				try
				{
					CommandHub hub = new CommandHub();
					ConsoleHelper.Start();
					ConsoleHelper.ApplicationShortTitle();
					hub.ProcessCommands(args);
				}
				catch (Exception ex)
				{
					ConsoleHelper.WriteException(ex);

				}
				
				ConsoleHelper.Shutdown();
				return ConsoleHelper.EXIT_OK;
			}
			catch
			{
				return ConsoleHelper.EXIT_ERROR;
			}
		}
	}
}