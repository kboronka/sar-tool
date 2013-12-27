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
					OnDataChange(this);
				}
			}
			get { return this.data; }
		}
		
		public DateTime Timestamp
		{
			get { return timestamp; }
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
			catch
			{

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
