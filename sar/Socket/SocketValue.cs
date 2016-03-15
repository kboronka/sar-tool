/* Copyright (C) 2016 Kevin Boronka
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
		private string name;
		private string data;
		private long sourceID;
		private DateTime timestamp;
		private DateTime lastUpdate;
		private ErrorLogger errorLog;
		
		public SocketValue(string name, ErrorLogger errorLog)
		{
			this.name = name;
			this.data = "";
			this.timestamp = new DateTime(2001, 1, 1);
			this.lastUpdate = new DateTime(2001, 1, 1);
			this.sourceID = 0;	
			this.errorLog = errorLog;		
		}
		
		#region properties
		
		public string Name
		{
			get { return this.name; }
		}
		
		public string Data
		{
			set
			{
				this.lastUpdate = DateTime.Now;
				if (this.data != value)
				{
					this.timestamp = DateTime.Now;
					this.data = value;
				}
				
				OnDataChange(this);
			}
			get { return this.data; }
		}
		
		public DateTime Timestamp
		{
			get { return this.timestamp; }
		}
		
		public DateTime LastUpdate
		{	
			get { return lastUpdate; }
		}
		
		public long SourceID
		{
			set { this.sourceID = value; }
			get { return this.sourceID; }
		}
		
		#endregion
		
		#region events
		
		#region data changed
		
		public delegate void DataChangedHandler(SocketValue sv);
		private DataChangedHandler dataChanged = null;
		
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
		
		private void OnDataChange(SocketValue data)
		{
			try
			{
				DataChangedHandler handler;
				if (null != (handler = (DataChangedHandler)this.dataChanged))
				{
					handler(data);
				}
			}
			catch (Exception ex)
			{
				if (this.errorLog != null) this.errorLog.Write(ex);
			}
		}

		#endregion

		#endregion
		
		public override string ToString()
		{
			return this.data;
		}
	}
}
