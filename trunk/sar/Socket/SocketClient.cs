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
	public class SocketClient
	{
		private TcpClient socket;
		private NetworkStream stream;
		private Encoding encoding;
		private long messageID;
		
		protected string hostname;
		protected int port;
		
		private bool connected;
		
		private Dictionary<string, string> lookup = new Dictionary<string, string>();
		
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
			get { return this.socket.Connected; }
		}
		
		public int Port
		{
			get { return this.port; }
		}
		
		public string Hostname
		{
			get { return this.hostname; }
		}
		
		public long ID
		{
			get
			{
				if (this.lookup.ContainsKey("clientID"))
				{
					return long.Parse(this.lookup["clientID"]);
				}

				return 0;
			}
			set
			{
				if (this.lookup.ContainsKey("clientID"))
				{
					this.lookup["clientID"] = value.ToString();
				}
				else
				{
					this.lookup.Add("clientID", value.ToString());
				}
			}
		}
		
		public Dictionary<string, string> Lookup
		{
			get { return lookup; }
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
					if (ConnectionChange != null)
					{
						ConnectionChange(connected, new System.EventArgs());
					}
				}
			}
			catch
			{
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
			catch
			{
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
			catch
			{
			}
		}

		#endregion

		#endregion

		#region constructors

		public SocketClient(TcpClient socket, long clientID, Encoding encoding)
		{
			//this.socket.ReceiveBufferSize;
			this.ID = clientID;
			this.socket = socket;
			this.encoding = encoding;
			this.Initilize();
			this.SendData("set", "clientID", this.ID.ToString(), -1);
		}
		
		public SocketClient(string hostname, int port, Encoding encoding)
		{
			this.hostname = hostname;
			this.port = port;
			this.encoding = encoding;
			this.socket = new TcpClient(this.hostname, this.port);
			this.Initilize();
		}
		
		private void Initilize()
		{
			this.OnConnectionChange(this.socket.Connected);
			this.stream = this.socket.GetStream();
			this.messagesOut = new List<SocketMessage>();
			this.messagesIn = new List<SocketMessage>();
			this.resendAttempts = 0;
			this.lastActivity = DateTime.UtcNow;
			this.serviceTimer = new Timer(ServiceTick, null, 100, Timeout.Infinite);
		}
		
		#endregion

		#region messageQueue

		private List<SocketMessage> messagesOut;
		private List<SocketMessage> messagesIn;
		
		public void SendData(string command)
		{
			this.SendData(command, "", "", -1);
		}
		
		public void SendData(string command, long toID)
		{
			this.SendData(command, "", "", toID);
		}

		public void SendData(string command, string data, long toID)
		{
			this.SendData(command, "", data, toID);
		}

		public void SendData(string command, string member, string data, long toID)
		{
			this.messageID++;
			SendData(new SocketMessage(this, command, this.messageID++, member, data, toID));
		}
		
		public void SendData(SocketMessage message)
		{
			lock (messagesOut)
			{
				messagesOut.Add(message);
			}
		}
		
		private void SendData(byte[] data)
		{

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
			catch
			{
				
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
			if (string.IsNullOrEmpty(this.hostname)) return;
			
			this.socket = new TcpClient(this.hostname, this.port);
			this.Initilize();
		}
		
		private void PreProcessMessage(SocketMessage message)
		{
			switch (message.Command)
			{
				case "ping":
					this.SendData("echo", message.FromID);
					break;
				case "set":
					if (this.lookup.ContainsKey(message.Member))
					{
						this.lookup[message.Member] = message.Data;
					}
					else
					{
						this.lookup.Add(message.Member, message.Data);
					}
					
					break;
					
				default:
					break;
			}
		}
		
		#endregion
		
		#region service
		
		private System.Threading.Timer serviceTimer;
		private int resendAttempts;
		private DateTime lastActivity;
		
		private void ServiceIncoming()
		{
			lock (socket)
			{
				if (!socket.Connected)
				{
					messagesIn.Clear();
				}
				
				lock (stream)
				{
					string messages = "";
					try
					{
						//this.stream = this.socket.GetStream().ReadTimeout = 6000;
						if (socket.Available > 0)
						{
							if (stream.DataAvailable)
							{
								byte[] bytes = new byte[socket.Available];
								int bytesRead = stream.Read(bytes, 0, bytes.Length);
								messages = this.encoding.GetString(bytes, 0, bytesRead);
								
								if (!String.IsNullOrEmpty(messages))
								{
									foreach (string rawMessage in messages.Split(new string[] { "<?xml version=\"1.0\" encoding=\"utf-16\"?>" }, StringSplitOptions.None))
									{
										try
										{
											if (!String.IsNullOrEmpty(rawMessage))
											{
												SocketMessage message = new SocketMessage(rawMessage);
												this.PreProcessMessage(message);
												this.OnMessageRecived(message);
												this.lastActivity = DateTime.Now;
												this.messagesIn.Add(message);
											}
										}
										catch (Exception ex)
										{
											//System.Diagnostics.Debug.WriteLine(messages);
											System.Diagnostics.Debug.WriteLine(ex.Message);
											System.Diagnostics.Debug.WriteLine("rawMessage");
											System.Diagnostics.Debug.WriteLine(rawMessage);
										}
									}
								}
							}
						}
					}
					catch (ObjectDisposedException)
					{
						// The NetworkStream is closed.
						this.Disconnect();
					}
					catch (IOException)
					{
						// The underlying Socket is closed.
						this.Disconnect();
					}
					
				}
			}
		}
		
		private void ServiceOutgoing()
		{
			lock (socket)
			{
				if (messagesOut.Count == 0) return;
				
				lock (messagesOut)
				{
					if (!socket.Connected)
					{
						messagesOut.Clear();
						this.Disconnect();
					}
					
					try
					{
						byte[] messageBytes = this.encoding.GetBytes(this.messagesOut[0].ToString());

						lock (stream)
						{
							stream.Write(messageBytes, 0, messageBytes.Length);
						}
						
						this.OnMessageSent(messagesOut[0]);
						this.lastActivity = DateTime.Now;
						messagesOut.RemoveAt(0);
					}
					catch (System.IO.IOException)
					{
						resendAttempts++;
						if (resendAttempts >3)
						{
							// log error
							messagesOut.RemoveAt(0);
							resendAttempts = 0;
							this.Disconnect();
						}
					}
					catch (ObjectDisposedException)
					{
						this.Disconnect();
					}
				}
			}
		}
		
		private void ServiceTick( Object state )
		{
			try
			{
				if (this.socket.Connected)
				{
					this.ServiceIncoming();
					this.ServiceOutgoing();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				//throw ex;
			}
			finally
			{
				this.serviceTimer.Change(100, Timeout.Infinite );
			}
		}
		
		#endregion
	}
}
