/* Copyright (C) 2017 Kevin Boronka
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
	public abstract class SocketMemCache : System.IDisposable
	{
		public SocketMemCache()
		{
		}

		public void Dispose()
		{
	        Dispose(true);
	        GC.SuppressFinalize(this);
		}
		
		protected bool disposed;
		protected abstract void Dispose(bool disposing);
		
		public abstract void Stop();
		
		#region memcache

		public abstract long ID
		{
			get;
			set;
		}
		
		protected Dictionary<string, SocketValue> memCache = new Dictionary<string, SocketValue>();
		
		public void RegisterCallback(string member, SocketValue.DataChangedHandler handler)
		{
			this.Log("RegisterCallback -- " + member);
			
			lock(this.memCache)
			{
				if (MemberExists(member))
				{
					this.memCache[member].DataChanged += handler;
				}
			}
		}
		
		protected bool MemberExists(string member)
		{
			if (!this.memCache.ContainsKey(member))
			{
				this.memCache[member] = new SocketValue(member);
				this.memCache[member].DataChanged += new SocketValue.DataChangedHandler(this.OnMemCacheChanged);
			}
			
			return true;
		}
		
		protected void Store(SocketMessage message)
		{
			string member = message.Member;

			lock(this.memCache)
			{
				if (MemberExists(member))
				{
					this.memCache[member].SourceID = message.FromID;
					this.memCache[member].Data = message.Data;
				}
			}
		}
		
		protected void Store(string member, string data)
		{
			lock(this.memCache)
			{
				if (MemberExists(member))
				{
					this.memCache[member].SourceID = this.ID;
					this.memCache[member].Data = data;
				}
			}
		}
		
		protected SocketValue Get(string member)
		{
			lock(this.memCache)
			{
				MemberExists(member);
				return this.memCache[member];
			}
		}
		
		public string GetValue(string member)
		{
			return this.Get(member).Data;
		}
		
		public DateTime GetTimestamp(string member)
		{
			return this.Get(member).Timestamp;
		}
		
		public DateTime GetLastUpdate(string member)
		{
			return this.Get(member).LastUpdate;
		}
		
		protected abstract void OnMemCacheChanged(SocketValue data);
		
		#endregion
		
		#region messageQueue
		
		private long messageID;
		
		public void SetValue(string member, bool data)
		{
			this.SetValue(member, data, false);
		}
		
		public void SetValue(string member, bool data, bool global)
		{
			this.SetValue(member, data.ToString(), global);
		}
		
		public void SetValue(string member, string data)
		{
			this.SetValue(member, data, false);
		}
		
		public void SetValue(string member, string data, bool global)
		{
			this.Store(member, data);
			if (!global) this.SendValue(this.Get(member), this.ID);
			if (global) this.SendValue(this.Get(member), -1);
		}
		
		public void SendData(string command)
		{
			this.SendData(command, "", "", -1);
		}
		
		public void SendData(string command, long to)
		{
			this.SendData(command, "", "", to);
		}

		public void SendValue(SocketValue data, long to)
		{
			this.messageID++;
			SendData(new SocketMessage(this.messageID, data, to));
		}
		
		public void SendData(string command, string member, string data, long to)
		{
			this.messageID++;
			SendData(new SocketMessage(this, command, this.messageID, member, data, to));
		}
		
		public abstract void SendData(SocketMessage message);
		
		#endregion

		#region logger
		
		protected void Log(string line)
		{
			Logger.Log(this.ToString() + ": " + line);
		}
		
		protected void Log(Exception ex)
		{
			Logger.Log(ex);
		}
		
		#endregion		
	}
}
