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
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

using sar.Tools;
using sar.Socket;

namespace sar.Controls
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
		
		void ConnectPBClick(object sender, EventArgs e)
		{
			this.Client.Connect();
		}
		
		void DissconnectPBClick(object sender, EventArgs e)
		{
			this.Client.Disconnect();
		}
	}
}
