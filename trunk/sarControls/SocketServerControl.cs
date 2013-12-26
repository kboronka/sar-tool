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
	public partial class SocketServerControl : UserControl
	{
		private SocketServer server;
		
		public SocketServer Server
		{
			get { return this.server; }
			set
			{
				if (this.server != null)
				{
					this.server.NewClient -= new EventHandler(this.ClientsChanged);
					this.server.ClientLost -= new EventHandler(this.ClientsChanged);
					this.server.DataChanged -= new SocketValue.DataChangedHandler(this.DataChanged);
				}
				
				this.server = value;
				
				if (this.server != null)
				{
					this.server.NewClient += new EventHandler(this.ClientsChanged);
					this.server.ClientLost += new EventHandler(this.ClientsChanged);
					this.server.DataChanged += new SocketValue.DataChangedHandler(this.DataChanged);
				}
			}
		}
		
		public SocketServerControl()
		{
			InitializeComponent();
			this.MessageList.Columns.Add("Member", -2, HorizontalAlignment.Left);
			this.MessageList.Columns.Add("Value", -2, HorizontalAlignment.Left);
			this.MessageList.Columns.Add("Timestamp", -2, HorizontalAlignment.Left);
			this.MessageList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.MessageList.FullRowSelect = true;
		}
		
		
		private void ClientsChanged(object sender, EventArgs e)
		{
			this.Invoke((MethodInvoker) delegate
			            {
			            	this.ActiveConnections.Text = "Active Connections: " + this.server.Clients.ToString();
			            });
		}
		
		private void DataChanged(SocketValue data)
		{
			this.Invoke((MethodInvoker) delegate
			            {
			            	this.MessageList.BeginUpdate();
			            	this.MessageList.Items.Clear();
			            	
			            	foreach (KeyValuePair<string, SocketValue> entry in this.server.MemCache)
			            	{
			            		ListViewItem newItem = new ListViewItem(entry.Key);
			            		newItem.SubItems.Add(entry.Value.Data);
			            		newItem.SubItems.Add(entry.Value.Timestamp.ToString());
			            		this.MessageList.Items.Add(newItem);
			            	}
			            	
			            	this.MessageList.Columns[0].Width = -2;
			            	this.MessageList.Columns[1].Width = -2;
			            	this.MessageList.Columns[2].Width = -2;
			            	/*
			            	this.MessageList.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
			            	this.MessageList.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
			            	this.MessageList.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
							*/
			            	this.MessageList.EndUpdate();
			            });
		}
	}
}
