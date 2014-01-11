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
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

using sar.Tools;

namespace sar.Socket
{
	public class SocketClient : SocketMemCache
	{
		private TcpClient socket;
		private NetworkStream stream;
		private Encoding encoding;
		
		protected string hostname;
		protected int port;
		
		private bool connected;
		private bool initilized;
		
		private long packetsIn;
		private long packetsOut;
		private long id;
		private SocketServer parent;
		
		#region properties
		
		public bool HasRequest
		{
			get { return this.messagesIn.Count > 0; }
		}
		
		public SocketMessage Read
		{
			get
			{
				if (this.HasRequest)
				{
					lock (this.messagesIn)
					{
						SocketMessage message = this.messagesIn[0];
						this.messagesIn.RemoveAt(0);
						return message;
					}
				}
				else
				{
					return null;
				}
			}
		}
		
		public bool Connected
		{
			get
			{
				if (this.socket == null) return false;
				return this.connected;
			}
		}
		
		public int Port
		{
			get { return this.port; }
		}
		
		public string Hostname
		{
			get { return this.hostname; }
		}
		
		public override long ID
		{
			get { return this.id; }
			set { this.id = value; }
		}
		
		public long PacketsIn
		{
			get { return packetsIn; }
		}

		public long PacketsOut
		{
			get { return packetsOut; }
		}
		
		public Dictionary<string, SocketValue> MemCache
		{
			get { return this.memCache; }
		}
		
		private bool IsHost
		{
			get { return (this.parent != null); }
		}
		
		#endregion

		#region events
		
		#region ConnectionChange

		public EventHandler ConnectionChange = null;
		
		private void OnConnectionChange(bool connected)
		{
			try
			{
				if (connected != this.connected)
				{
					this.connected = connected;
					if (this.connected) this.SendData("get-all");

					if (ConnectionChange != null)
					{
						ConnectionChange(connected, new System.EventArgs());
					}
				}
			}
			catch (Exception ex)
			{
				this.Log(ex);
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

		#region constructors

		public SocketClient(SocketServer parent, TcpClient socket, long clientID, ErrorLogger errorLog, FileLogger debugLog) : base(errorLog, debugLog)
		{
			try
			{
				this.encoding = Encoding.ASCII;
				this.parent = parent;
				this.ID = clientID;
				this.socket = socket;
				this.stream = this.socket.GetStream();
				
				this.connectionLoopThread = new Thread(this.ConnectionLoop);
				this.outgoingLoopThread = new Thread(this.OutgoingLoop);
				this.incomingLoopThread = new Thread(this.IncomingLoop);
				this.pingLoopThread = new Thread(this.PingLoop);
				
				this.Log("Host Constructor");

				this.messagesOut = new List<SocketMessage>();
				this.messagesIn = new List<SocketMessage>();
				
				// connected
				this.initilized = true;
				this.OnConnectionChange(true);
				
				this.outgoingLoopThread.Start();
				this.incomingLoopThread.Start();
				this.connectionLoopThread.Start();
				
				// send clientID
				this.SendData("set", "Me.ID", clientID.ToString(), this.ID);
				
				// send all values
				lock (parent.MemCache)
				{
					foreach (KeyValuePair<string, SocketValue> entry in parent.MemCache)
					{
						SocketValue val = entry.Value;
						this.SendValue(entry.Value, this.ID);
					}
				}
				
				// send initilization code to client
				this.SendData("startup", this.ID);
			}
			catch (Exception ex)
			{
				this.Log(ex);
			}
		}
		
		public SocketClient(string hostname, int port, ErrorLogger errorLog, FileLogger debugLog)  : base(errorLog, debugLog)
		{
			this.Log("Client Constructor");

			this.encoding = Encoding.ASCII;
			this.parent = null;
			this.hostname = hostname;
			this.port = port;
			this.messagesOut = new List<SocketMessage>();
			this.messagesIn = new List<SocketMessage>();
			this.initilized = false;
			this.connected = false;
			
			this.connectionLoopThread = new Thread(this.ConnectionLoop);
			this.outgoingLoopThread = new Thread(this.OutgoingLoop);
			this.incomingLoopThread = new Thread(this.IncomingLoop);
			this.pingLoopThread = new Thread(this.PingLoop);
			
			this.connectionLoopThread.Start();
			this.incomingLoopThread.Start();
			this.outgoingLoopThread.Start();
			this.pingLoopThread.Start();
		}
		
		public override void Stop()
		{
			try
			{
				this.Log("Shutdown Started");
				
				this.connectionLoopShutdown = true;
				if (this.connectionLoopThread.IsAlive) this.connectionLoopThread.Join();
				
				this.pingLoopShutdown = true;
				this.outgoingLoopShutdown = true;
				this.incomingLoopShutdown = true;
				
				if (this.pingLoopThread.IsAlive) this.pingLoopThread.Join();
				if (this.outgoingLoopThread.IsAlive) this.outgoingLoopThread.Join();
				if (this.incomingLoopThread.IsAlive) this.incomingLoopThread.Join();
				
				this.Disconnect();

				this.Log("Shutdown Completed");
			}
			catch (Exception ex)
			{
				this.Log(ex);
			}
		}

		#endregion

		#region messageQueue

		private List<SocketMessage> messagesOut;
		private List<SocketMessage> messagesIn;

		public override void SendData(SocketMessage message)
		{
			try
			{
				lock (messagesOut)
				{
					messagesOut.Add(message);
				}
			}
			catch (Exception ex)
			{
				this.Log(ex);
			}
		}
		
		#endregion
		
		#region methods
		
		public void Disconnect()
		{
			try
			{
				this.Log("Disconnect");

				if (this.stream != null) this.stream.Close();
				if (this.socket != null) this.socket.Close();
				
				OnConnectionChange(false);
				this.initilized = false;
				this.memCache = new Dictionary<string, SocketValue>();
				this.messagesOut.Clear();
				this.messagesIn.Clear();
			}
			catch (Exception ex)
			{
				this.Log(ex);
			}
			finally
			{
				this.socket = null;
				this.id = 0;
				OnConnectionChange(false);
			}
		}
		
		private bool ProcessMessage(SocketMessage message)
		{
			if (message != null)
			{
				switch (message.Command)
				{
					case "startup":
						this.initilized = true;
						return true;
					case "echo":
						this.PingReciveEcho(message);
						return true;
					case "ping":
						this.PingSendEcho(message);
						return true;
					case "set":
						this.Store(message);
						//this.OnMessageRecived(message);
						return (message.ToID == this.ID);
					case "get":
						if (this.IsHost) return false;	// allow parent to respond to "get" commands
						this.SendData("set", message.Member, this.GetValue(message.Member), message.FromID);
						//this.OnMessageRecived(message);
						return true;
					case "get-all":
						if (this.IsHost) return false;	// allow parent to respond to "get" commands
						return true;
					default:
						//this.OnMessageRecived(message);
						break;
				}
				
				return false;
			}
			
			return true;
		}
		
		#endregion
		
		#region service
		
		#region connection monitor
		
		private Thread connectionLoopThread;
		private bool connectionLoopShutdown = false;
		
		private void ConnectionLoop()
		{
			if (this.IsHost) Thread.Sleep(250);
			
			this.Log(this.ID.ToString() +  ": " + "Connection Loop Started");

			while (!connectionLoopShutdown)
			{
				try
				{
					if (this.socket == null)
					{
						this.OpenConnection();
						Thread.Sleep(500);
					}
					else
					{
						this.MonitorConnection();
						Thread.Sleep(100);
					}
				}
				catch (Exception ex)
				{
					this.Log(ex);
					Thread.Sleep(1000);
				}
			}
			
			this.Log(this.ID.ToString() +  ": " + "Connection Loop Shutdown Gracefully");
		}
		
		private void MonitorConnection()
		{
			if (!this.initilized) return;
			if (!this.connected) this.socket = null;
			
			lock (socket)
			{
				if (!socket.Connected)
				{
					this.Disconnect();
					return;
				}
				
				try
				{
					if( this.socket.Client.Poll(1, SelectMode.SelectRead))
					{
						byte[] buff = new byte[1];
						if(this.socket.Client.Receive(buff,  SocketFlags.Peek) == 0)
						{
							this.Disconnect();
						}
					}
				}
				catch
				{
					this.Disconnect();
				}
			}
		}

		#endregion

		#region new connection

		private static int newConnectionID = 1000;
		
		public void OpenConnection()
		{
			if (this.socket != null) return;
			if (this.connected) return;
			if (this.initilized) return;
			if (this.IsHost) return;
			if (string.IsNullOrEmpty(this.hostname)) return;
			
			int connectionAttempt = newConnectionID++;
			this.Log(connectionAttempt.ToString() + ": " + " NewConnection");
			
			try
			{
				this.connected = false;
				this.initilized = false;
				
				// connect
				this.socket = new TcpClient(this.hostname, this.port);
				this.stream = this.socket.GetStream();
				
				//this.connected = true;
				this.Log(connectionAttempt.ToString() + ": " + " Socket Open");
				
				Stopwatch timeout = new Stopwatch();
				timeout.Start();

				while(!this.initilized)
				{
					if (this.connectionLoopShutdown) return;
					Thread.Sleep(100);
					//ServiceIncoming();
					
					if (timeout.ElapsedMilliseconds > 6000)
					{
						this.Log(connectionAttempt.ToString() + ": " + " Initilization Timeout");
						this.Disconnect();
						return;
					}
				}
				
				this.ID = int.Parse(GetValue("Me.ID"));

				this.Log(connectionAttempt.ToString() + ": " + " Initilized");
				this.Log(connectionAttempt.ToString() + ": " + " Connected --> " + this.id.ToString());

				this.OnConnectionChange(this.initilized);
			}
			catch (System.Net.Sockets.SocketException)
			{
				this.Disconnect();
				this.socket = null;
			}
			catch (Exception ex)
			{
				this.Log(ex);
				this.Disconnect();
				this.socket = null;
			}
		}
		
		#endregion
		
		#region incoming
		
		private Thread incomingLoopThread;
		private bool incomingLoopShutdown = false;
		
		private void IncomingLoop()
		{
			Thread.Sleep(0);
			this.Log(this.ID.ToString() +  ": " + "Incoming Loop Started");
			
			while (!incomingLoopShutdown)
			{
				try
				{
					this.ServiceIncoming();
					Thread.Sleep(1);
				}
				catch (Exception ex)
				{
					this.Log(ex);
					Thread.Sleep(1000);
				}
			}
			
			this.Log(this.ID.ToString() +  ": " + "Incoming Loop Shutdown Gracefully");
		}
		
		private void ServiceIncoming()
		{
			try
			{
				List<SocketMessage> messageQueue = new List<SocketMessage>();
				
				if (socket == null) return;
				if (stream == null) return;
				
				lock (socket)
				{
					lock (stream)
					{
						if (socket.Available > 0)
						{
							if (stream.DataAvailable)
							{
								byte[] packetBytes = new byte[socket.Available];
								int packetSize = stream.Read(packetBytes, 0, packetBytes.Length);
								string packetString = this.encoding.GetString(packetBytes, 0, packetSize);
								
								if (!String.IsNullOrEmpty(packetString))
								{
									foreach (string rawString in packetString.Split(new string[] { "<?xml version=\"1.0\" encoding=\"utf-16\"?>" }, StringSplitOptions.None))
									{
										if (!String.IsNullOrEmpty(rawString))
										{
											using (StringReader sr = new StringReader(rawString))
											{
												try
												{
													using (XML.Reader reader = new XML.Reader(sr))
													{
														while (reader.Read())
														{
															if (reader.NodeType == XmlNodeType.Element)
															{
																switch (reader.Name)
																{
																	case "SocketMessage":
																		messageQueue.Add(new SocketMessage(reader));
																		break;
																	default:
																		break;
																}
															}
														}
													}
												}
												catch (Exception ex)
												{
													this.Log(ex);
												}
											}
										}
									}
								}
							}
						}
					}
				}
				
				
				foreach (SocketMessage message in messageQueue)
				{
					this.packetsIn++;
					if (!this.ProcessMessage(message))
					{
						lock (this.messagesIn)
						{
							this.messagesIn.Add(message);
						}
					}
				}
			}
			catch (ObjectDisposedException)
			{
				//this.Log(ex);
				// The NetworkStream is closed.
				//this.Disconnect();
			}
			catch (IOException)
			{
				//this.Log(ex);
				// The underlying Socket is closed.
				//this.Disconnect();
			}
			catch (Exception ex)
			{
				this.Log(ex);
			}
			
			return;
		}
		
		#endregion

		#region outgoing
		
		private Thread outgoingLoopThread;
		private bool outgoingLoopShutdown = false;
		private int resendAttempts;
		
		private void OutgoingLoop()
		{
			Thread.Sleep(100);
			this.Log(this.ID.ToString() +  ": " + "Outgoing Loop Started");
			
			while (!outgoingLoopShutdown)
			{
				try
				{
					this.ServiceOutgoing();
					Thread.Sleep(1);
				}
				catch (Exception ex)
				{
					this.Log(ex);
					Thread.Sleep(1000);
				}
			}
			
			this.Log(this.ID.ToString() +  ": " + "Outgoing Loop Shutdown Gracefully");
		}
		
		private void ServiceOutgoing()
		{
			if (socket == null) return;
			if (stream == null) return;
			if (messagesOut.Count == 0) return;
			
			List<SocketMessage> messageQueue = new List<SocketMessage>();
			
			try
			{
				// assemble packet
				lock (messagesOut)
				{
					messageQueue.AddRange(messagesOut);
					messagesOut.Clear();
				}
				
				string packetString = "";
				using (StringWriter sw = new StringWriter())
				{
					using (XML.Writer writer = new XML.Writer(sw))
					{
						writer.WriteStartElement("SocketMessages");
						
						foreach (SocketMessage message in messageQueue)
						{
							message.Serialize(writer);
						}
						
						writer.WriteEndElement();
					}
					
					packetString = sw.ToString();
				}

				// write data to socket
				byte[] packetBytes = this.encoding.GetBytes(packetString.ToString());
				lock (socket)
				{
					lock (stream)
					{
						stream.Write(packetBytes, 0, packetBytes.Length);
					}
				}

				this.packetsOut += messageQueue.Count;
			}
			catch (System.IO.IOException)
			{
				lock (messagesOut)
				{
					messagesOut.AddRange(messageQueue);
				}
				
				resendAttempts++;
				if (resendAttempts >3)
				{
					this.Log("Failed three send attempts");
					Thread.Sleep(1000);
					// TODO: log error
					//this.messagesOut.Clear();
					resendAttempts = 0;
					//this.Disconnect();
				}
			}
			catch (ObjectDisposedException)
			{
				lock (messagesOut)
				{
					messagesOut.AddRange(messageQueue);
				}
				
				//this.Disconnect();
			}
		}

		#endregion

		#region pingLoop

		private Thread pingLoopThread;
		private bool pingLoopShutdown = false;

		private int lastPingID;
		private Stopwatch pingStopWatch;

		private void PingLoop()
		{
			Thread.Sleep(1000);
			this.Log(this.ID.ToString() +  ": " + "Ping Loop Started");
			pingStopWatch = new Stopwatch();
			pingStopWatch.Start();
			
			while (!pingLoopShutdown)
			{
				try
				{
					this.Ping();
					Thread.Sleep((this.IsHost ? 10000 : 500));
				}
				catch (Exception ex)
				{
					this.Log(ex);
					Thread.Sleep(1000);
				}
			}
			
			this.Log(this.ID.ToString() +  ": " + "Ping Loop Shutdown Gracefully");
		}

		private void Ping()
		{
			if (!this.connected) return;
			this.SendData("ping", (++lastPingID).ToString(), pingStopWatch.ElapsedMilliseconds.ToString(), this.ID);
		}

		private void PingSendEcho(SocketMessage message)
		{
			try
			{
				this.SendData("echo", message.Member, message.Data, message.FromID);
			}
			catch (Exception ex)
			{
				this.Log(ex);
			}
		}

		private void PingReciveEcho(SocketMessage message)
		{
			try
			{
				if (pingStopWatch == null) return;
				
				long pingTime = long.Parse(message.Data);
				int pingID = int.Parse(message.Member);
				long pingDuration = this.pingStopWatch.ElapsedMilliseconds - pingTime;

				if (pingID == this.lastPingID)
				{
					this.Store("Me.Ping", pingDuration.ToString());
				}
			}
			catch (Exception ex)
			{
				this.Log(ex);
			}
		}

		#endregion

		#endregion

		public override string ToString()
		{
			return (this.IsHost ? "host" : "client") + "-" + this.id.ToString();
		}
	}
}

