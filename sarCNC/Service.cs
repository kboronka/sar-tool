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
			Engine.Start();
		}
		
		protected override void OnStop()
		{
			Engine.Stop();
		}
	}
}
