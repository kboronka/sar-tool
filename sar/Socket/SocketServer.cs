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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using sar.Tools;

namespace sar.Socket
{
	public class SocketServer : SocketMemCache
	{
		private List<SocketClient> clients;
		private TcpListener listener;
		private Encoding encoding;
		private long lastClientID = 100;
		protected int port;

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
		
		public override long ID
		{
			get
			{
				return -1;
			}
			set
			{
				
			}
		}
		
		#endregion
		
		#region events
		
		#region Newclient

		public EventHandler NewClient = null;
		
		private void OnNewClient(SocketClient client)
		{
			try
			{
				this.Set("Server.Clients", this.clients.Count.ToString());
				
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
				this.Set("Server.Clients", this.clients.Count.ToString());
				
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

		#region DataChanged

		private SocketValue.DataChangedHandler dataChanged = null;
		public event SocketValue.DataChangedHandler DataChanged
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
		
		protected override void OnMemCacheChanged(SocketValue data)
		{
			try
			{
				SocketValue.DataChangedHandler handler;
				if (null != (handler = (SocketValue.DataChangedHandler)this.dataChanged))
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
		
		#region constructor
		
		public SocketServer(int port)
		{
			this.port = port;
			this.StartServer(this.port);
		}
		
		public SocketServer(int port, FileLogger errorLog)
		{
			this.ErrorLog = errorLog;
			this.port = port;
			this.StartServer(this.port);
		}
		
		private void StartServer(int port)
		{
			this.encoding = Encoding.ASCII;
			this.clients = new List<SocketClient>();
			this.listener = new TcpListener(IPAddress.Any, this.port);
			this.listener.Start();
			this.serviceListenerTimer = new Timer(this.ServiceListenerTick, null, 1, Timeout.Infinite);
			this.serviceClientsTimer = new Timer(this.ServiceClientsTick, null, 1, Timeout.Infinite);
			this.pingTimer = new Timer(this.Ping, null, 1000, Timeout.Infinite);
			this.Store("Server.Version", AssemblyInfo.SarVersion);
			this.Store("Server.Port", this.port.ToString());
			this.Store("Server.Clients", this.clients.Count.ToString());
			this.Store("Application.Product", AssemblyInfo.Product);
			this.Store("Application.Version", AssemblyInfo.Version);
		}
		
		#endregion
		
		#region methods
		
		public void Shutdown()
		{
			lock (this.listener)
			{
				this.listener.Stop();
			}
			
			lock (this.clients)
			{
				foreach (SocketClient client in this.clients)
				{
					client.Disconnect();
				}
				
				this.clients = new List<SocketClient>();
			}
		}
		
		public void Set(string member, string data)
		{
			this.Store(member, data);
			this.Broadcast("set", member, data);
		}
		
		public void Broadcast(SocketMessage message)
		{
			lock (this.clients)
			{
				foreach (SocketClient client in this.clients)
				{
					client.SendData(message);
				}
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
		
		public override void SendData(SocketMessage message)
		{
			// FIXME... execute a broadcast
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
						this.Store(message);
						
						if (message.ToID == -1)
						{
							this.Broadcast(message);
						}

						break;
						
						
					case "get":
						lock (this.memCache)
						{
							client.SendValue(this.Get(message.Member), -1);
						}
						
						break;
						
					case "get-all":
						lock (this.memCache)
						{
							foreach (KeyValuePair<string, SocketValue> entry in this.memCache)
							{
								SocketValue val = entry.Value;
								client.SendValue(entry.Value, message.FromID);
							}
						}
						
						break;
					default:
						break;
				}
			}
		}
		
		#endregion
		
		#region service
		
		#region listners
		
		private System.Threading.Timer serviceListenerTimer;
		
		private void ServiceListener()
		{
			try
			{
				lock (this.listener)
				{
					if (this.listener.Pending())
					{
						SocketClient client = new SocketClient(this, this.listener.AcceptTcpClient(), ++this.lastClientID, this.ErrorLog);
						this.clients.Add(client);
						this.OnNewClient(client);
					}
				}
			}
			catch (Exception ex)
			{
				this.Log(ex);
			}
		}
		
		private void ServiceListenerTick(Object state)
		{
			try
			{
				this.ServiceListener();
			}
			catch (Exception ex)
			{
				this.Log(ex);
			}
			finally
			{
				this.serviceListenerTimer.Change(10, Timeout.Infinite );
			}
		}
		
		#endregion
		
		#region clients
		
		private System.Threading.Timer serviceClientsTimer;

		private void ServiceClients()
		{
			try
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
			catch (Exception ex)
			{
				this.Log(ex);
			}
		}
		
		private void ServiceClientsTick(Object state)
		{
			try
			{
				this.ServiceClients();
			}
			catch (Exception ex)
			{
				this.Log(ex);
			}
			finally
			{
				this.serviceClientsTimer.Change(10, Timeout.Infinite );
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
				//this.pingTimer.Change(1000, Timeout.Infinite );
			}
		}

		#endregion
		
		#endregion
	}
}
