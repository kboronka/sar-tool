
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

using sar.Tools;
using sar.Socket;

namespace sar.Controls
{
	public partial class SocketServerControl : UserControl
	{
		private SocketServer server;
		
		public SocketServer Server
		{
			get { return this.server; }
			set
			{
				if (this.server != null)
				{
					this.server.NewClient -= new EventHandler(this.ClientsChanged);
					this.server.ClientLost -= new EventHandler(this.ClientsChanged);
				}
				
				this.server = value;
				
				if (this.server != null)
				{
					this.server.NewClient += new EventHandler(this.ClientsChanged);
					this.server.ClientLost += new EventHandler(this.ClientsChanged);
					this.UpdateControls();
				}
			}
		}
		
		public SocketServerControl()
		{
			InitializeComponent();
		}
		
		
		private void ClientsChanged(object sender, EventArgs e)
		{
			this.UpdateControls();

		}
		
		private void UpdateControls()
		{
			if (InvokeRequired)
			{
				this.Invoke(new MethodInvoker(UpdateControls));
				return;
			}
			
			this.ActiveConnections.Text = "Active Connections: " + this.server.Clients.ToString();
		}		
	}
}
