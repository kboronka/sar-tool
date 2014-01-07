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
using System.Drawing;
using System.Windows.Forms;

using sar.Socket;

namespace sar.Controls
{
	public class SocketMemCacheList : System.Windows.Forms.ListView
	{
		private SocketServer server;
		private SocketClient client;
		private Dictionary<string, SocketValue> memCache;
		
		#region Properties

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SocketServer Server
		{
			get { return this.server; }
			set
			{
				if (this.server != null)
				{
					this.server.DataChanged -= new SocketValue.DataChangedHandler(this.DataChanged);
					this.MemCache = null;
				}
				
				this.server = value;
				
				if (this.server != null)
				{
					this.server.DataChanged += new SocketValue.DataChangedHandler(this.DataChanged);
					this.MemCache = this.server.MemCache;
				}
			}
		}
		
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SocketClient Client
		{
			get { return this.client; }
			set
			{
				if (this.client != null)
				{
					this.client.DataChanged -= new SocketValue.DataChangedHandler(this.DataChanged);
					this.MemCache = null;
				}
				
				this.client = value;
				
				if (this.client != null)
				{
					this.client.DataChanged += new SocketValue.DataChangedHandler(this.DataChanged);
					this.MemCache = this.client.MemCache;
				}
			}
		}
		
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		private Dictionary<string, SocketValue> MemCache
		{
			set
			{
				if (value == null)
				{
					this.memCache = new Dictionary<string, SocketValue>();
				}
				
				this.memCache = value;
				this.InitializeList();
			}
		}
		
		#endregion

		public SocketMemCacheList()
		{
			this.Columns.Add("Member", -2, HorizontalAlignment.Left);
			this.Columns.Add("Value", -2, HorizontalAlignment.Left);
			this.Columns.Add("Timestamp", -2, HorizontalAlignment.Left);
			this.Columns.Add("Source", -2, HorizontalAlignment.Left);
			this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.FullRowSelect = true;
			this.View = View.Details;
		}
		
		private void InitializeList()
		{
			try
			{
				if (this.memCache == null) return;
				
				this.BeginUpdate();
				this.Items.Clear();
				
				foreach (KeyValuePair<string, SocketValue> entry in this.memCache)
				{
					ListViewItem newItem = new ListViewItem(entry.Value.Name);
					newItem.Name = entry.Value.Name;
					newItem.SubItems.Add(entry.Value.Data);
					newItem.SubItems.Add(entry.Value.Timestamp.ToString());
					newItem.SubItems.Add(entry.Value.SourceID.ToString());
					this.Items.Add(newItem);
				}
				
				this.Columns[0].Width = -2;
				this.Columns[1].Width = -2;
				this.Columns[2].Width = -2;
				this.Columns[3].Width = -2;
				this.EndUpdate();
			}
			catch
			{
				
			}
		}
		
		private void DataChanged(SocketValue data)
		{
			if (this.memCache == null) return;
			
			this.Invoke((MethodInvoker) delegate
			            {
			            	this.BeginUpdate();

			            	if (this.Items.ContainsKey(data.Name))
			            	{
			            		ListViewItem[] existingItems = this.Items.Find(data.Name, true);
			            		ListViewItem existingItem = existingItems[0];
			            		existingItem.SubItems[0].Text = data.Name;
			            		existingItem.SubItems[1].Text = data.Data;
			            		existingItem.SubItems[2].Text = data.Timestamp.ToString();
			            		existingItem.SubItems[3].Text = data.SourceID.ToString();
			            	}
			            	else
			            	{
			            		ListViewItem newItem = new ListViewItem(data.Name);
			            		newItem.Name = data.Name;
			            		newItem.SubItems.Add(data.Data);
			            		newItem.SubItems.Add(data.Timestamp.ToString());
			            		newItem.SubItems.Add(data.SourceID.ToString());
			            		this.Items.Add(newItem);
			            	}
			            	
			            	this.Columns[0].Width = -2;
			            	this.Columns[1].Width = -2;
			            	this.Columns[2].Width = -2;
			            	this.Columns[3].Width = -2;
			            	this.EndUpdate();
			            });
		}
	}
}
