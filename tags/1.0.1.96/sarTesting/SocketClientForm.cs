
using System;
using System.Drawing;
using System.Windows.Forms;

using skylib.Socket;

namespace sar_testing
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
