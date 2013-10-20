
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

using sar.Tools;
using sar.Socket;

namespace sar.Testing.Controls
{
	public partial class SocketClientControl : UserControl
	{
		private SocketClient client;
		private List<string> messageHistory;
		private int maxHistory = 10;
		
		public SocketClient Client
		{
			get { return this.client; }
			set
			{
				if (this.client != null)
				{
					this.client.MessageSent -= new EventHandler(this.MessageSent);
					this.client.MessageRecived -= new EventHandler(this.MessageRecived);
					this.client.ConnectionChange -= new EventHandler(this.Connected);
					this.connected.Status = false;
				}
				
				this.client = value;
				
				if (this.client != null)
				{
					this.client.MessageSent += new EventHandler(this.MessageSent);
					this.client.MessageRecived += new EventHandler(this.MessageRecived);
					this.client.ConnectionChange += new EventHandler(this.Connected);
					this.connected.Status = this.client.Connected;
				}
			}
		}
		
		public SocketClientControl()
		{
			InitializeComponent();
			this.messageHistory = new List<string>();
			this.maxHistory = this.History.ClientSize.Height / this.History.ItemHeight;
		}
		
		private void SendClick(object sender, EventArgs e)
		{
			if (this.client == null)
			{
				return;
			}
			
			if (!string.IsNullOrEmpty(Message.Text))
			{
				this.client.SendData(Message.Text);
				Message.Text = "";
			}
		}
		
		private void Connected(object sender, EventArgs e)
		{
			bool connected = (bool)sender;
			if (connected)
			{
				this.messageHistory.Add("Connected");
			}
			else
			{
				this.messageHistory.Add("Disconnected");
			}
			
			this.connected.Status = connected;
			if (this.messageHistory.Count > maxHistory) this.messageHistory.RemoveAt(0);
			this.UpdateHistory();
		}
		
		private void MessageSent(object sender, EventArgs e)
		{
			string message = (string)sender;
			if (!string.IsNullOrEmpty(message))
			{
				this.messageHistory.Add("SEND: " + message);
				if (this.messageHistory.Count > maxHistory) this.messageHistory.RemoveAt(0);
				this.UpdateHistory();
			}
		}
		
		private void MessageRecived(object sender, EventArgs e)
		{
			string message = (string)sender;
			if (!string.IsNullOrEmpty(message))
			{
				this.messageHistory.Add("RECIVED: " + message);
				if (this.messageHistory.Count > maxHistory) this.messageHistory.RemoveAt(0);
				this.UpdateHistory();
			}
		}
		
		private void UpdateHistory()
		{
			if (InvokeRequired)
			{
				this.Invoke(new MethodInvoker(UpdateHistory));
				return;
			}
			
			this.History.BeginUpdate();
			this.History.Items.Clear();
			
			foreach (string log in this.messageHistory)
			{
				this.History.Items.Add(log);
			}
			
			this.History.EndUpdate();
		}
	}
}
