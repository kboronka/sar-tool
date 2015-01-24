/* Copyright (C) 2015 Kevin Boronka
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
		
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SocketClient Client
		{
			get { return this.client; }
			set
			{
				try
				{
					if (this.client != null)
					{
						this.client.ConnectionChange -= new EventHandler(this.Connected);
						this.connected.Status = false;
					}
					
					this.client = value;
					
					if (this.client != null)
					{
						this.client.ConnectionChange += new EventHandler(this.Connected);
						this.connected.Status = this.client.Connected;
					}
				}
				catch
				{
					
				}
			}
		}
		
		public SocketClientControl()
		{
			InitializeComponent();
			
			this.MessageList.Columns.Add("ID", -2, HorizontalAlignment.Left);
			this.MessageList.Columns.Add("", -2, HorizontalAlignment.Left);
			this.MessageList.Columns.Add("From", -2, HorizontalAlignment.Left);
			this.MessageList.Columns.Add("To", -2, HorizontalAlignment.Left);
			this.MessageList.Columns.Add("Message", -2, HorizontalAlignment.Left);
			this.MessageList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.MessageList.FullRowSelect = true;
		}
		
		private void SendClick(object sender, EventArgs e)
		{
			if (this.client == null)
			{
				return;
			}
			
			if (!string.IsNullOrEmpty(CustomMessage.Text))
			{
				this.client.SendData(CustomMessage.Text);
				CustomMessage.Text = "";
			}
		}
		
		private void Connected(object sender, EventArgs e)
		{
			bool connected = (bool)sender;
			
			this.UpdateControls();
		}
		
		private void MessageSent(object sender, EventArgs e)
		{
			try
			{
				if (sender is SocketMessage)
				{
					SocketMessage message = (SocketMessage)sender;
					this.UpdateHistory("<", message);
				}
			}
			catch (Exception)
			{
				
			}
		}
		
		private void MessageRecived(object sender, EventArgs e)
		{
			try
			{
				if (sender is SocketMessage)
				{
					SocketMessage message = (SocketMessage)sender;
					string log = "IN: (" + message.Id.ToString() + ")";
					
					if (!String.IsNullOrEmpty(message.Command)) log += " " + message.Command;
					if (!String.IsNullOrEmpty(message.Member)) log += " " + message.Member;
					if (!String.IsNullOrEmpty(message.Data)) log += " " + message.Data;
					
					this.UpdateHistory(">", message);
				}
			}
			catch (Exception)
			{
				
			}
		}
		
		private void UpdateControls()
		{
			if (InvokeRequired)
			{
				this.Invoke(new MethodInvoker(UpdateControls));
				return;
			}
			
			this.connected.Status = this.client.Connected;
			this.PacketsIn.Text = "In: " + this.client.PacketsIn.ToString();
			this.PacketsOut.Text = "Out: " + this.client.PacketsOut.ToString();
			this.ClientID.Text = "ClientID: " + this.client.ID.ToString();
		}
		
		private void UpdateHistory(string direction, SocketMessage message)
		{
			this.Invoke((MethodInvoker) delegate
			            {
			            	this.UpdateControls();

			            	this.MessageList.BeginUpdate();
			            	ListViewItem newItem = new ListViewItem(message.Id.ToString());
			            	newItem.SubItems.Add(direction);
			            	newItem.SubItems.Add(message.FromID.ToString());
			            	newItem.SubItems.Add(message.ToID.ToString());
			            	newItem.SubItems.Add(message.Command);

			            	this.MessageList.Items.Add(newItem);
			            	
			            	int max = (this.MessageList.ClientSize.Height / this.MessageList.GetItemRect(0).Height) - 1;
			            	
			            	while (this.MessageList.Items.Count > max)
			            	{
			            		this.MessageList.Items.RemoveAt(0);
			            	}
			            	
			            	this.MessageList.Columns[0].Width = -2;
			            	this.MessageList.Columns[1].Width = -2;
			            	this.MessageList.Columns[2].Width = -2;
			            	this.MessageList.Columns[3].Width = -2;
			            	this.MessageList.Columns[4].Width = -2;
			            	
			            	this.MessageList.EndUpdate();
			            });
		}
	}
}
