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

using sar.Tools;

namespace sar.Socket
{
	public class SocketClient
	{
		private TcpClient socket;
		private NetworkStream stream;
		private Encoding encoding;
		
		protected string hostname;
		protected int port;
		
		private bool connected;
		
		#region properties
		
		public bool HasRequest
		{
			get { return this.messagesIn.Count > 0; }
		}
		
		public byte[] ReadBytes
		{
			get
			{
				if (this.HasRequest)
				{
					lock (this.messagesIn)
					{
						byte[] message = this.messagesIn[0];
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
		
		public string ReadString
		{
			get { return this.encoding.GetString(this.ReadBytes); }
		}
		
		public bool Connected
		{
			get { return this.socket.Connected; }
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
		
		private void OnMessageRecived(string message)
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
		
		private void OnMessageSent(string message)
		{
			try
			{
				if (MessageSent != null)
				{
					message = message.Substring(0, message.Length - "<message-end>".Length);
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

		public SocketClient(TcpClient socket, Encoding encoding)
		{
			//this.socket.ReceiveBufferSize;
			this.socket = socket;
			this.encoding = encoding;
			this.Initilize();
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
			this.messagesOut = new List<byte[]>();
			this.messagesIn = new List<byte[]>();
			this.resendAttempts = 0;
			this.lastActivity = DateTime.UtcNow;
			this.serviceTimer = new Timer(ServiceTick, null, 100, Timeout.Infinite);
		}
		
		#endregion

		#region messageQueue

		private List<byte[]> messagesOut;
		private List<byte[]> messagesIn;
		
		public void SendData(string data)
		{
			this.SendData(this.encoding.GetBytes(data + "<message-end>"));
		}

		private void SendData(byte[] data)
		{
			lock (messagesOut)
			{
				messagesOut.Add(data);
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
									foreach (string message in messages.Split(new string[] { "<message-end>" }, StringSplitOptions.None))
									{
										this.OnMessageRecived(message);
										this.lastActivity = DateTime.Now;
										this.messagesIn.Add(this.encoding.GetBytes(message));
									}
								}
							}
						}
					}
					catch (ObjectDisposedException ex)
					{
						// The NetworkStream is closed.
						this.Disconnect();
					}
					catch (IOException ex)
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
						lock (stream)
						{
							stream.Write(messagesOut[0], 0, messagesOut[0].Length);
						}
						
						this.OnMessageSent(this.encoding.GetString(messagesOut[0]));
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
				throw ex;
			}
			finally
			{
				this.serviceTimer.Change(100, Timeout.Infinite );
			}
		}
		
		#endregion
	}
}
