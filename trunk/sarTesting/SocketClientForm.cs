
using System;
using System.Drawing;
using System.Windows.Forms;

using sar.Socket;
using sar.Controls;

namespace sar.Testing
{
	public partial class SocketClientForm : Form
	{
		private SocketClient client;
		public SocketClient Client
		{
			set
			{
				this.socketClientControl1.Client = value;
				this.client = value;
			}
		}
		
		public SocketClientForm()
		{
			InitializeComponent();
		}
		
		void SocketClientFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.client != null && this.client.Connected)
			{
				if(MessageBox.Show("Do you want to close the client?", "sarTesting", MessageBoxButtons.YesNo) ==  DialogResult.No)
				{
					e.Cancel = true;
				}
				
				this.client.Disconnect();
			}
		}
	}
}
