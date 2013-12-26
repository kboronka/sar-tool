
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
		private Dictionary<string, SocketValue> memCache = new Dictionary<string, SocketValue>();
		
		public SocketServer Server
		{
			get { return this.server; }
			set
			{
				if (this.server != null)
				{
					this.server.DataChanged -= new SocketValue.DataChangedHandler(this.DataChanged);
					this.memCache = null;
				}
				
				this.server = value;
				
				if (this.server != null)
				{
					this.server.DataChanged += new SocketValue.DataChangedHandler(this.DataChanged);
					this.memCache = this.server.MemCache;
				}
			}
		}
				
		public SocketClient Client
		{
			get { return this.client; }
			set
			{
				if (this.client != null)
				{
					this.client.DataChanged -= new SocketValue.DataChangedHandler(this.DataChanged);
					this.memCache = null;
				}
				
				this.client = value;
				
				if (this.client != null)
				{
					this.client.DataChanged += new SocketValue.DataChangedHandler(this.DataChanged);
					this.memCache = this.client.MemCache;
				}
			}
		}
		
		public SocketMemCacheList()
		{
			this.Columns.Add("Member", -2, HorizontalAlignment.Left);
			this.Columns.Add("Value", -2, HorizontalAlignment.Left);
			this.Columns.Add("Timestamp", -2, HorizontalAlignment.Left);
			this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.FullRowSelect = true;
			this.View = View.Details;
		}
		
		private void DataChanged(SocketValue data)
		{
			this.Invoke((MethodInvoker) delegate
			            {
			            	this.BeginUpdate();
			            	this.Items.Clear();
			            	
			            	foreach (KeyValuePair<string, SocketValue> entry in this.memCache)
			            	{
			            		ListViewItem newItem = new ListViewItem(entry.Key);
			            		newItem.SubItems.Add(entry.Value.Data);
			            		newItem.SubItems.Add(entry.Value.Timestamp.ToString());
			            		this.Items.Add(newItem);
			            	}
			            	
			            	this.Columns[0].Width = -2;
			            	this.Columns[1].Width = -2;
			            	this.Columns[2].Width = -2;
			            	this.EndUpdate();
			            });
		}
	}
}
