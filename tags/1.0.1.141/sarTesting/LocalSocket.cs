/* Copyright (C) 2014 Kevin Boronka
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

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
	public partial class LocalSocket : Form
	{
		private SocketClient client1;
		private SocketClient client2;
		private SocketClient client3;
		
		private SocketClientForm client1Form;
		private SocketClientForm client2Form;
		private SocketClientForm client3Form;
		
		private SocketServer server;
		
		public LocalSocket()
		{
			InitializeComponent();
			server = new SocketServer(8104, Program.ErrorLog, Program.DebugLog);
			this.socketServerControl1.Server = server;

			client1 = new SocketClient("localhost", 8104, Program.ErrorLog, Program.DebugLog);
			client1.RegisterCallback("testmember", new SocketValue.DataChangedHandler(this.Client1Update));
			this.socketMemCacheList1.Client = client1;
			
			client2 = new SocketClient("localhost", 8104, Program.ErrorLog, Program.DebugLog);
			client2.RegisterCallback("testmember", new SocketValue.DataChangedHandler(this.Client2Update));
			this.socketMemCacheList2.Client = client2;
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			if (this.client1Form == null)
			{
				this.client1Form = new SocketClientForm();
				this.client1Form.Client = client1;
				this.client1Form.Show();
			}
			else
			{
				client1.SendData("ping");
				client1.SendData("ping");
				client1.SendData("ping");
			}
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			if (this.client2Form == null)
			{
				this.client2Form = new SocketClientForm();
				this.client2Form.Client = client2;
				this.client2Form.Show();
			}
			else
			{
				client2.SendData("ping");
			}
		}
		
		void Connect3Click(object sender, EventArgs e)
		{
			if (client3 == null)
			{
				client3 = new SocketClient(this.Host.Text, (int)this.Port.Value, Program.ErrorLog, Program.DebugLog);
				client3.SendData("ping");

				this.client3Form = new SocketClientForm();
				this.client3Form.Client = client3;
				this.client3Form.Show();
			}
			else
			{
				client3.SendData("ping");
			}
		}
		
		void SetClick(object sender, EventArgs e)
		{
			this.client1.SetValue("testmember", TestMember.Text, true);
		}
		
		void GetClick(object sender, EventArgs e)
		{
			if (this.client1 != null) this.Client1Member.Text = "Client1: " + this.client1.GetValue("testmember");
			if (this.client2 != null) this.Client2Member.Text = "Client2: " + this.client2.GetValue("testmember");
		}
		
		private void Client1Update(SocketValue data)
		{
			this.Invoke((MethodInvoker) delegate
			            {
			            	this.Client1Member.Text = "Client1: " + data.Data;
			            });
			
		}

		private void Client2Update(SocketValue data)
		{
			this.Invoke((MethodInvoker) delegate
			            {
			            	this.Client2Member.Text = "Client2: " + data.Data;
			            });
			
		}
		
		private void Set_C1Click(object sender, EventArgs e)
		{
			if (this.client1 != null)
			{
				this.client1.SetValue("testmember", DateTime.Now.ToString("ss.fff"));
			}
		}
		
		void Set_C2Click(object sender, EventArgs e)
		{
			if (this.client2 != null)
			{
				this.client2.SetValue("testmember", DateTime.Now.ToString("ss.fff"));
			}
		}
		
		private void Connect3_25Click(object sender, EventArgs e)
		{
			if (client3 == null)
			{

			}
			else
			{
				client3.SendData("ping");
			}
		}
		
		void StartServerClick(object sender, EventArgs e)
		{
			if (this.server == null)
			{
				server = new SocketServer(8104, Program.ErrorLog, Program.DebugLog);
				this.socketServerControl1.Server = server;
			}
		}
		
		void StopServerClick(object sender, EventArgs e)
		{
			if (this.server != null)
			{
				this.server.Stop();
				server = null;
				this.socketServerControl1.Server = null;
			}
		}
	}
}
