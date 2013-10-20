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
		protected int port;

		#region properties
		
		public int Clients
		{
			get { return this.clients.Count; }
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
			catch
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
		
		public void Broadcast(string message)
		{
			lock (this.clients)
			{
				foreach (SocketClient client in this.clients)
				{
					client.SendData(message);
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
						SocketClient client = new SocketClient(this.listener.AcceptTcpClient(), this.encoding);
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
						string request = client.ReadString;
						
						if (!string.IsNullOrEmpty(request))
						{
							string command = StringHelper.FirstWord(request).ToLower();
							
							switch (command)
							{
								case "ping":
									client.SendData("echo");
									break;
								default:
									break;
							}
						}
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
				this.Broadcast("ping");
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
