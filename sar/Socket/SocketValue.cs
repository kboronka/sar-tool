
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

using sar.Tools;

namespace sar.Socket
{
	public class SocketValue
	{
		private string data;
		private DateTime timestamp;
		
		public SocketValue()
		{
			this.data = "";
			this.timestamp = DateTime.Now;
		}
		
		#region properties
		
		public string Data
		{
			set
			{
				if(this.data != value)
				{
					this.data = value;
					this.timestamp = DateTime.Now;
					OnDataChange(this.data);
				}
			}
			get { return this.data; }
		}
		
		
		#endregion
		
		
		#region events
		
		#region DataChanged
		
		public delegate void DataChangedHandler(string s);
		public event DataChangedHandler DataChanged
		{
			add
			{
				this.dataChanged += value;
			}
			remove
			{
				this.dataChanged -= value;
			}
		}
		
		private DataChangedHandler dataChanged = null;
		
		private void OnDataChange(string data)
		{
			try
			{
				DataChangedHandler handler;
				if (null != (handler = (DataChangedHandler)this.dataChanged))
				{
					handler(data);
				}
			}
			catch
			{

			}
		}

		#endregion

		#endregion
	}
}
