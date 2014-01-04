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
		
		private long packetsIn;
		private long packetsOut;
		private SocketServer parent;
		
		private Exception lastError;
		
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
			get
			{
				try
				{
					return long.Parse(this.GetValue("Me.ID"));
				}
				catch
				{
					
				}
				
				return 0;
			}
			set
			{
				this.Store("Me.ID", value.ToString());
			}
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
		
		private Exception LastError
		{
			set
			{
				System.Diagnostics.Debug.WriteLine(value.Message);
				System.Diagnostics.Debug.WriteLine(value.StackTrace);
				this.lastError = value;
			}
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
				this.LastError = ex;
			}
		}

		#endregion

		#region MessageRecived

		public EventHandler MessageRecived = null;
		
		private void OnMessageRecived(SocketMessage message)
		{
			try
			{
				if (MessageRecived != null)
				{
					MessageRecived(message, new System.EventArgs());
				}
			}
			catch (Exception ex)
			{
				this.LastError = ex;
			}
		}

		#endregion

		#region MessageSent

		public EventHandler MessageSent = null;
		
		private void OnMessageSent(SocketMessage message)
		{
			try
			{
				if (MessageSent != null)
				{
					MessageSent(message, new System.EventArgs());
				}
			}
			catch (Exception ex)
			{
				this.LastError = ex;
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

		public SocketClient(SocketServer parent, TcpClient socket, long clientID, Encoding encoding)
		{
			this.socket = socket;
			this.stream = this.socket.GetStream();
			this.parent = parent;
			this.ID = clientID;
			this.encoding = encoding;
			this.messagesOut = new List<SocketMessage>();
			this.messagesIn = new List<SocketMessage>();
			
			// send ClientID
			try
			{
				this.SendData("set", "Me.ID", this.ID.ToString(), this.ID);
				ServiceOutgoing();
			}
			catch
			{
				
			}
			
			this.OnConnectionChange(true);
			
			this.serviceConnectionTimer = new Timer(this.ServiceConnectionTick, null, 10, Timeout.Infinite);
			this.serviceTimer = new Timer(this.ServiceTick, null, 100, Timeout.Infinite);
			this.heartbeatTimer = new Timer(this.HeartbeatTick, null, 200, Timeout.Infinite);
		}
		
		public SocketClient(string hostname, int port, Encoding encoding)
		{
			this.parent = null;
			this.hostname = hostname;
			this.port = port;
			this.encoding = encoding;
			this.messagesOut = new List<SocketMessage>();
			this.messagesIn = new List<SocketMessage>();
			
			this.serviceConnectionTimer = new Timer(this.ServiceConnectionTick, null, 10, Timeout.Infinite);
			this.serviceTimer = new Timer(this.ServiceTick, null, 100, Timeout.Infinite);
			this.heartbeatTimer = new Timer(this.HeartbeatTick, null, 200, Timeout.Infinite);
		}
		
		#endregion

		#region messageQueue

		private List<SocketMessage> messagesOut;
		private List<SocketMessage> messagesIn;

		public override void SendData(SocketMessage message)
		{
			lock (messagesOut)
			{
				messagesOut.Add(message);
			}
		}
		
		#endregion
		
		#region methods
		
		public void Disconnect()
		{
			try
			{
				if (!this.connected) return;
				
				if (this.stream != null)
				{
					this.stream.Close();
				}
				
				if (this.socket != null)
				{
					this.socket.Close();
				}
			}
			catch (Exception ex)
			{
				this.LastError = ex;
			}
			finally
			{
				//this.socket = null;
				this.OnConnectionChange(false);
			}
		}
		
		public void Connect()
		{
			if (this.connected) return;
			if (this.parent != null) return;
			if (string.IsNullOrEmpty(this.hostname)) return;
			
			try
			{
				
				this.socket = new TcpClient();
				IAsyncResult asyncConnection = this.socket.BeginConnect(this.hostname, this.port, null, null);
				System.Threading.WaitHandle wh = asyncConnection.AsyncWaitHandle;
				try
				{
					if (!asyncConnection.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(2000), false))
					{
						this.socket.Close();
						throw new TimeoutException();
					}
					
					this.socket.EndConnect(asyncConnection);
				}
				finally
				{
					wh.Close();
				}
				
				this.socket = new TcpClient(this.hostname, this.port);
				this.stream = this.socket.GetStream();
				
				// send ClientID
				try
				{
					Stopwatch timeout = new Stopwatch();
					timeout.Start();
					
					this.ID = ServiceIncoming();
					while (this.ID != 0)
					{
						Thread.Sleep(10);
						this.ID = ServiceIncoming();
						if (timeout.ElapsedMilliseconds > 5000)
						{
							this.Disconnect();
							throw new ApplicationException("Did not recive client ID");
						}
					}

				}
				catch
				{
					
				}
				
				this.OnConnectionChange(true);
			}
			catch
			{
				this.socket = null;
			}
		}
		
		private bool ProcessMessage(SocketMessage message)
		{
			if (message != null)
			{
				if (this.parent != null)
				{
					// if client is attached to a server, then the server should respond
					return false;
				}
				
				switch (message.Command)
				{
					case "heartbeat":
						return true;
					case "ping":
						this.SendData("echo", message.FromID);
						this.OnMessageRecived(message);
						return true;
					case "set":
						this.Store(message);
						this.OnMessageRecived(message);
						return (message.ToID == this.ID);
					case "get":
						this.SendData("set", message.Member, this.GetValue(message.Member), message.FromID);
						this.OnMessageRecived(message);
						return true;
					default:
						this.OnMessageRecived(message);
						break;
				}
				
				return false;
			}
			
			return true;
		}
		
		#endregion
		
		#region service
		
		#region connection
		
		private System.Threading.Timer serviceConnectionTimer;
		
		private void ServiceConnection()
		{
			if (this.socket == null)
			{
				try
				{
					this.Connect();
				}
				catch
				{
					this.socket = null;
				}
			}
			
			
			if (!this.connected) this.socket = null;
			if (this.socket == null) return;
			
			lock (socket)
			{
				if (!socket.Connected)
				{
					this.Disconnect();
					return;
				}
				
				try
				{
					/*
					if (!(this.socket.Client.Poll(1, SelectMode.SelectRead) && socket.Available == 0))
					{
						this.Disconnect();
					}
					 */

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

		private void ServiceConnectionTick( Object state )
		{
			try
			{
				this.ServiceConnection();
			}
			catch (Exception ex)
			{
				this.LastError = ex;
			}
			finally
			{
				this.serviceConnectionTimer.Change(250, Timeout.Infinite );
			}
		}

		#endregion

		#region input/output
		
		private System.Threading.Timer serviceTimer;
		private int resendAttempts;
		private DateTime lastActivity;
		
		private int ServiceIncoming()
		{
			int newClientID = 0;
			if (this.socket == null) return newClientID;
			if (!this.connected) return newClientID;
			
			lock (socket)
			{
				if (!socket.Connected)
				{
					messagesIn.Clear();
				}
				
				lock (stream)
				{
					try
					{
						//this.stream = this.socket.GetStream().ReadTimeout = 6000;
						if (socket.Available > 0)
						{
							if (stream.DataAvailable)
							{
								byte[] bytes = new byte[socket.Available];
								int bytesRead = stream.Read(bytes, 0, bytes.Length);
								string messages = this.encoding.GetString(bytes, 0, bytesRead);
								List<SocketMessage> newMessages = new List<SocketMessage>();
								
								if (!String.IsNullOrEmpty(messages))
								{
									foreach (string rawString in messages.Split(new string[] { "<?xml version=\"1.0\" encoding=\"utf-16\"?>" }, StringSplitOptions.None))
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
																		newMessages.Add(new SocketMessage(reader));
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
													this.LastError = ex;
												}
											}
										}
									}
								}
								
								lock (this.messagesIn)
								{
									foreach (SocketMessage message in newMessages)
									{
										this.packetsIn++;
										if (message.Member == "Me.ID") newClientID = int.Parse(message.Data);
										if (!this.ProcessMessage(message)) this.messagesIn.Add(message);
										this.lastActivity = DateTime.Now;
									}
								}
							}
						}
					}
					catch (ObjectDisposedException)
					{
						// The NetworkStream is closed.
						//this.Disconnect();
					}
					catch (IOException)
					{
						// The underlying Socket is closed.
						//this.Disconnect();
					}
				}
			}
			
			return newClientID;
		}
		
		private void ServiceOutgoing()
		{
			if (this.socket == null) return;
			if (!this.connected) return;
			
			lock (socket)
			{
				if (messagesOut.Count == 0) return;
				
				lock (messagesOut)
				{
					try
					{

						string messages = "";
						
						using (StringWriter sw = new StringWriter())
						{
							using (XML.Writer writer = new XML.Writer(sw))
							{
								writer.WriteStartElement("SocketMessages");
								
								foreach (SocketMessage message in this.messagesOut)
								{
									message.Serialize(writer);
								}
								
								writer.WriteEndElement();
							}
							
							messages = sw.ToString();
						}
						
						byte[] messageBytes = this.encoding.GetBytes(messages.ToString());

						lock (stream)
						{
							stream.Write(messageBytes, 0, messageBytes.Length);
						}
						
						foreach (SocketMessage message in this.messagesOut)
						{
							switch (message.Command)
							{
								case "heartbeat":
									break;
								default:
									this.packetsOut++;
									this.OnMessageSent(message);
									break;
							}
						}
						
						this.lastActivity = DateTime.Now;
						this.messagesOut.Clear();
					}
					catch (System.IO.IOException)
					{
						resendAttempts++;
						if (resendAttempts >3)
						{
							// TODO: log error
							this.messagesOut.Clear();
							resendAttempts = 0;
//							this.Disconnect();
						}
					}
					catch (ObjectDisposedException)
					{
						//this.Disconnect();
					}
				}
			}
		}
		
		private void ServiceTick( Object state )
		{
			try
			{
				this.ServiceIncoming();
				this.ServiceOutgoing();
			}
			catch (Exception ex)
			{
				this.LastError = ex;
			}
			finally
			{
				this.serviceTimer.Change(100, Timeout.Infinite );
			}
		}
		
		#endregion
		
		#region heartbeat
		
		private System.Threading.Timer heartbeatTimer;

		private void HeartbeatTick(Object state)
		{
			try
			{
				if (this.connected)
				{
					this.SendData("heartbeat");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
			}
			finally
			{
				this.heartbeatTimer.Change(this.connected ? 500 : 2000, Timeout.Infinite );
			}
		}

		#endregion
		
		#endregion
	}
}

