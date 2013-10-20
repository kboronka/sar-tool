using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Threading;
using System.Windows.Forms;

using skylib;
using skylib.Tools;
using skylib.Socket;

namespace sar_testing
{
	public partial class MainForm : Form
	{
		private System.Timers.Timer updateTimer;
		private SocketClient client1;
		private SocketClient client2;
		
		private SocketClientForm client1Form;
		private SocketClientForm client2Form;
		
		private SocketServer server;
		
		public MainForm()
		{
			InitializeComponent();
			server = new SocketServer(8100, Encoding.ASCII);
			updateTimer = new System.Timers.Timer(1000);
			updateTimer.Enabled = true;
			updateTimer.Elapsed += new ElapsedEventHandler(UpdateTimerDone);
			
			this.client1Form = new SocketClientForm();
			this.client2Form = new SocketClientForm();
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			if (client1 == null)
			{
				client1 = new SocketClient("localhost", 8100, Encoding.ASCII);
				this.client1Form.Client = client1;
				this.client1Form.Show();
				client1.SendData("ping");
			}
			else
			{
				client1.SendData("ping");
			}
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			if (client2 == null)
			{
				client2 = new SocketClient("localhost", 8100, Encoding.ASCII);
				this.client2Form.Client = client2;
				this.client2Form.Show();
				client2.SendData("ping");
			}
			else
			{
				client2.SendData("ping");
			}
		}
		
		private void UpdateTimerDone(object sender, ElapsedEventArgs e)
		{
			try
			{
				this.UpdateControls();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}
		
		private void UpdateControls()
		{
			if (InvokeRequired)
			{
				this.Invoke(new MethodInvoker(UpdateControls));
				return;
			}
			
			this.ActiveConnections.Text = "Active Connections: " + this.server.Connections.Count.ToString();
		}
	}
}
