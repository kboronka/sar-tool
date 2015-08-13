using System;
using System.Collections.Generic;
using System.IO;

using sar.Tools;
using sar.Base;


namespace sar.Commands
{
	public class Bower : Command
	{
		public Bower(Base.CommandHub parent) : base(parent, "Bower Update",
		                                            new List<string> { "bower" },
		                                            "-bower",
		                                            new List<string> { @"-bower" })
		{
			
		}
		
		public override int Execute(string[] args)
		{			
			var nodejs = IO.FindApplication("node.exe", "nodejs");
			var bower = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\npm\node_modules\bower\bin\bower";
		
			
			if (!File.Exists(bower)) throw new ApplicationException("Bower not found");
			
			//ConsoleHelper.Run(nodejs, bower + " install");
			ConsoleHelper.Run(nodejs, bower + " update");
			
			ConsoleHelper.WriteLine("Bower update was successfully completed", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
