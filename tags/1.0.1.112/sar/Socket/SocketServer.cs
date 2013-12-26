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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using sar.Tools;

namespace sar.Socket
{
	public class SocketServer
	{
		private List<SocketClient> clients;
		private TcpListener listener;
		private Encoding encoding;
		private long lastClientID = 100;
		protected int port;
		private Dictionary<string, SocketValue> memCache = new Dictionary<string, SocketValue>();

		#region properties
		
		public int Clients
		{
			get { return this.clients.Count; }
		}
		
		public int Port
		{
			get { return this.port; }
		}
		
		public Dictionary<string, SocketValue> MemCache
		{
			get { return this.memCache; }
		}
		
		#endregion
		
		#region events
		
		#region Newclient

		public EventHandler NewClient = null;
		
		private void OnNewClient(SocketClient client)
		{
			try
			{
				if (NewClient != null)
				{
					NewClient(client, new System.EventArgs());
				}
			}
			catch (Exception)
			{
			}
		}

		#endregion

		#region ClientLost

		public EventHandler ClientLost = null;
		
		private void OnClientLost(SocketClient client)
		{
			try
			{
				if (ClientLost != null)
				{
					ClientLost(client, new System.EventArgs());
				}
			}
			catch
			{
			}
		}

		#endregion

		#endregion
		
		#region constructor
		
		public SocketServer(int port, Encoding encoding)
		{
			this.encoding = encoding;
			this.port = port;
			this.clients = new List<SocketClient>();
			this.listener = new TcpListener(IPAddress.Any, port);
			this.listener.Start();
			this.serviceTimer = new Timer(this.ServiceTick, null, 10, Timeout.Infinite);
			this.pingTimer = new Timer(this.Ping, null, 1000, Timeout.Infinite);
		}
		
		#endregion
		
		#region methods
		
		public string Get(string member)
		{
			if (this.memCache.ContainsKey(member))
			{
				return this.memCache[member].Data;
			}

			return "";
		}
		
		public void Set(string member, string data)
		{
			this.Store(member, data);
			this.Broadcast("set", member, data);
		}
		
		public void Store(string member, string data)
		{
			lock(this.memCache)
			{
				if (!this.memCache.ContainsKey(member))
				{
					this.memCache[member] = new SocketValue();
				}
				
				this.memCache[member].Data = data;
			}
		}
		
		
		public void Broadcast(string command, string member, string data)
		{
			lock (this.clients)
			{
				foreach (SocketClient client in this.clients)
				{
					client.SendData(command, member, data, client.ID);
				}
			}
		}
		
		private void ProcessMessage(SocketClient client, SocketMessage message)
		{
			if (message != null)
			{
				switch (message.Command.ToLower())
				{
					case "ping":
						client.SendData("echo", client.ID);
						break;
						
						
					case "set":
						this.Store(message.Member, message.Data);
						
						if (message.ToID == -1)
						{
							this.Broadcast(message.Command, message.Member, message.Data);
						}

						break;
						
						
					case "get":
						lock (this.memCache)
						{
							client.SendData("set", message.Member, this.Get(message.Member), message.FromID);
						}
						
						break;
						
						
					default:
						break;
				}
			}
		}
		
		#endregion
		
		#region service
		
		private System.Threading.Timer serviceTimer;
		
		private void ServiceListener()
		{
			try
			{
				lock (this.listener)
				{
					if (this.listener.Pending())
					{
						SocketClient client = new SocketClient(this.listener.AcceptTcpClient(), ++this.lastClientID, this.encoding);
						this.clients.Add(client);
						this.OnNewClient(client);
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				// add execption handler
				throw ex;
			}
		}
		
		private void ServiceClients()
		{
			lock (this.clients)
			{
				foreach (SocketClient client in this.clients)
				{
					if (!client.Connected)
					{
						this.clients.Remove(client);
						OnClientLost(client);
						break;
					}
					
					if (client.HasRequest)
					{
						ProcessMessage(client, client.Read);
					}
				}
			}
		}
		
		private void ServiceTick(Object state)
		{
			try
			{
				this.ServiceListener();
				this.ServiceClients();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
			}
			finally
			{
				this.serviceTimer.Change(10, Timeout.Infinite );
			}
		}
		
		#endregion
		
		#region pingLoop
		
		private System.Threading.Timer pingTimer;

		private void Ping(Object state)
		{
			try
			{
				this.Broadcast("ping", "", "");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
			}
			finally
			{
				this.pingTimer.Change(1000, Timeout.Infinite );
			}
		}

		#endregion
	}
}
