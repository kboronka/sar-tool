
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Threading;
using System.Windows.Forms;

using sar.Tools;
using sar.Socket;

namespace sar.Testing
{
	public partial class RemoteSocket : Form
	{
		private SocketClient client1;

		public RemoteSocket()
		{
			InitializeComponent();

			client1 = new SocketClient("10.240.14.8", 8111, Encoding.ASCII);
			this.socketMemCacheList1.Client = client1;
			this.socketClientControl1.Client = client1;
			client1.SendData("ping");
		}
		
		void Connect3Click(object sender, EventArgs e)
		{
			
		}
	}
}
