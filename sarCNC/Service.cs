using System;
using System.ServiceProcess;
using System.Threading;

using sar.Tools;
using Base = sar.Base;

namespace sar.CNC
{
	public class Service : ServiceBase
	{
		public const string NAME = "sarCNC";
		
		public Service()
		{
			InitializeComponent();
		}
		
		private void InitializeComponent()
		{
			this.ServiceName = NAME;
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		
		protected override void OnStart(string[] args)
		{
			var thread = new Thread(StartServices);
			thread.Start();
		}
		
		protected override void OnStop()
		{
			
		}
		
		public static void StartServices()
		{
			try
			{
				Program.Log("Staring Service");
				App.Load();
				Program.Log("Service Startup Complete");
			}
			catch (Exception ex)
			{
				Program.Log(ex);
				Base.Program.FlushLogs();
				
				if (Environment.UserInteractive)
				{
					ConsoleHelper.WriteException(ex);
				}
			}
		}
	}
}
