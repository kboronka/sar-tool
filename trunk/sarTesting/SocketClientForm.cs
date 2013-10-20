
using System;
using System.Drawing;
using System.Windows.Forms;

using sar.Socket;

namespace sar.Testing
{
	public partial class SocketClientForm : Form
	{
		public SocketClient Client
		{
			set
			{
				this.socketClientControl1.Client = value;
			}
		}
				
		public SocketClientForm()
		{
			InitializeComponent();
		}
		
	}
}
