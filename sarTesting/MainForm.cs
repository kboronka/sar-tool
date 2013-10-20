/* Copyright (C) 2013 Kevin Boronka
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
			this.socketServerControl1.Server = server;
			
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
			
			this.ActiveConnections.Text = "Active Connections: " + this.server.Clients.ToString();
		}
	}
}
