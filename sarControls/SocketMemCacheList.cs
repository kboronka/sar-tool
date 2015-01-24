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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

using sar.Tools;
using sar.Socket;

namespace sar.Controls
{
	public class SocketMemCacheList : System.Windows.Forms.ListView
	{
		private SocketServer server;
		private SocketClient client;
		private Dictionary<string, SocketValue> memCache;
		private ListViewColumnSorter columnSorter;
		
		private string controlLock = "thread lock";
		private bool updatesAvailable = true;
		private System.Timers.Timer updateTimer;
		private ErrorLogger errorLog;
		private FileLogger debugLog;
		
		#region Properties

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SocketServer Server
		{
			get { return this.server; }
			set
			{
				try
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
					
					this.Enabled = (this.server != null);
				}
				catch
				{
					
				}
			}
		}
		
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
						this.client.ConnectionChange -= new EventHandler(this.OnConnectChange);
						this.client.DataChanged -= new SocketValue.DataChangedHandler(this.DataChanged);
						this.MemCache = null;
					}
					
					this.client = value;
					
					if (this.client != null)
					{
						this.client.ConnectionChange += new EventHandler(this.OnConnectChange);
						this.client.DataChanged += new SocketValue.DataChangedHandler(this.DataChanged);
						this.MemCache = this.client.MemCache;
						this.Enabled = this.client.Connected;
					}
				}
				catch
				{
					
				}
			}
		}
		
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		private Dictionary<string, SocketValue> MemCache
		{
			set
			{
				try
				{
					if (value == null)
					{
						this.memCache = new Dictionary<string, SocketValue>();
					}
					
					this.memCache = value;
					this.InitializeList();
				}
				catch
				{
					
				}
			}
		}
		
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ErrorLogger ErrorLog
		{
			get { return errorLog; }
			set { errorLog = value; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FileLogger DebugLog
		{
			get { return debugLog; }
			set { debugLog = value; }
		}
		
		
		#endregion

		#region constructors
		
		public SocketMemCacheList()
		{
			this.InitilizeControl();
		}
		
		private void InitilizeControl()
		{
			try
			{
				this.columnSorter = new ListViewColumnSorter();
				this.Columns.Add("Member", -2, HorizontalAlignment.Left);
				this.Columns.Add("Value", -2, HorizontalAlignment.Left);
				this.Columns.Add("Timestamp", -2, HorizontalAlignment.Left);
				this.Columns.Add("Source", -2, HorizontalAlignment.Left);
				this.ListViewItemSorter = this.columnSorter;
				this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
				this.FullRowSelect = true;
				this.View = View.Details;
				
				this.updateTimer = new System.Timers.Timer(5000);
				this.updateTimer.Enabled = true;
				this.updateTimer.Elapsed += new ElapsedEventHandler(this.UpdateTick);
			}
			catch
			{
				
			}
		}
		
		#endregion
		
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
			catch (System.ObjectDisposedException)
			{
				
			}
			catch (Exception ex)
			{
				if (this.errorLog != null) this.errorLog.Write(ex);
			}
		}
		
		private void OnConnectChange(object sender, EventArgs e)
		{
			lock (this.controlLock)
			{
				try
				{
					bool connected = (bool)sender;
					if (this.Enabled == connected) return;
					
					if (InvokeRequired)
					{
						this.Invoke((MethodInvoker) delegate { this.Enabled = connected; } );
					}
					else
					{
						this.Enabled = connected;
					}
				}
				catch (System.ObjectDisposedException)
				{
					
				}
				catch (Exception ex)
				{
					if (this.errorLog != null) this.errorLog.Write(ex);
				}
			}
		}
		
		private void DataChanged(SocketValue data)
		{
			lock (this.controlLock)
			{
				try
				{
					if (this.memCache == null) return;
					this.updatesAvailable = true;
				}
				catch (System.ObjectDisposedException)
				{
					
				}
				catch (Exception ex)
				{
					if (this.errorLog != null) this.errorLog.Write(ex);
				}
			}
		}

		private void UpdateTick(object source, ElapsedEventArgs e)
		{
			try
			{
				if (this.memCache == null) return;
				
				if (this.updatesAvailable)
				{
					this.UpdateList();
					this.updatesAvailable = false;
				}
			}
			catch (System.ObjectDisposedException)
			{
				
			}
			catch (Exception ex)
			{
				if (this.errorLog != null) this.errorLog.Write(ex);
			}
		}
		
		private void UpdateList()
		{
			try
			{
				if (InvokeRequired)
				{
					this.Invoke((MethodInvoker) delegate { this.UpdateListInner(); });
				}
				else
				{
					this.UpdateListInner();
				}
			}
			catch
			{
			}
		}
		
		private void UpdateListInner()
		{
			try
			{
				ListViewHelper.EnableDoubleBuffer(this);
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
				ListViewHelper.DisableDoubleBuffer(this);
			}
			catch (System.ObjectDisposedException)
			{
				
			}
			catch (Exception ex)
			{
				if (this.errorLog != null) this.errorLog.Write(ex);
			}
		}
	}
	
	public class ListViewColumnSorter : IComparer
	{
		private int ColumnToSort;
		private SortOrder OrderOfSort;

		private CaseInsensitiveComparer ObjectCompare;


		public ListViewColumnSorter()
		{
			ColumnToSort = 0;
			OrderOfSort = SortOrder.Ascending;
			ObjectCompare = new CaseInsensitiveComparer();
		}

		public int Compare(object x, object y)
		{
			int compareResult;
			ListViewItem listviewX, listviewY;

			// Cast the objects to be compared to ListViewItem objects
			listviewX = (ListViewItem)x;
			listviewY = (ListViewItem)y;

			// Compare the two items
			compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text,listviewY.SubItems[ColumnToSort].Text);
			
			if (OrderOfSort == SortOrder.Ascending)
			{
				return compareResult;
			}
			else if (OrderOfSort == SortOrder.Descending)
			{
				return (-compareResult);
			}
			else
			{
				return 0;
			}
		}
		
		public int SortColumn
		{
			set
			{
				ColumnToSort = value;
			}
			get
			{
				return ColumnToSort;
			}
		}

		public SortOrder Order
		{
			set
			{
				OrderOfSort = value;
			}
			get
			{
				return OrderOfSort;
			}
		}
		
	}
}
