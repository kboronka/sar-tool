
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
	public abstract class SocketMemCache
	{
		public SocketMemCache()
		{
		}
		
		#region memcache
		public abstract long ID
		{
			get;
			set;
		}
		
		protected Dictionary<string, SocketValue> memCache = new Dictionary<string, SocketValue>();
		
		public void RegisterCallback(string member, SocketValue.DataChangedHandler handler)
		{
			lock(this.memCache)
			{
				if (!this.memCache.ContainsKey(member))
				{
					MemberExists(member);
					//this.SendData("get", member, "");
				}
				
				this.memCache[member].DataChanged += handler;
			}
		}
		
		protected bool MemberExists(string member)
		{
			if (!this.memCache.ContainsKey(member))
			{
				this.memCache[member] = new SocketValue(member);
				this.memCache[member].Data = "";
				this.memCache[member].SourceID = 0;
				this.memCache[member].Timestamp = new DateTime(2001, 1, 1);
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
					this.memCache[member].Timestamp = message.Timestamp;
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
					this.memCache[member].Timestamp = DateTime.Now;
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
		
		protected abstract void OnMemCacheChanged(SocketValue data);
		
		#endregion
		
		#region messageQueue
		
		private long messageID;
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

	}
}
