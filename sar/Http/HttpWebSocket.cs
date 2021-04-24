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

using sar.Json;
using sar.Tools;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace sar.Http
{
	public abstract class HttpWebSocket
	{
		#region static

		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
		public class SarWebSocketController : Attribute
		{

		}

		private static Dictionary<string, Type> controllers;
		private static int nextID;

		public static void LoadControllers()
		{
			HttpWebSocket.controllers = new Dictionary<string, Type>();

			foreach (Assembly assembly in AssemblyInfo.Assemblies)
			{

				foreach (Type type in assembly.GetTypes())
				{
					if (type.Name.EndsWith("WebSocket"))
					{
						foreach (object attribute in type.GetCustomAttributes(false))
						{
							if (attribute is SarWebSocketController)
							{
								// add the sar controller
								string controllerName = type.Name.Substring(0, type.Name.Length - "WebSocket".Length);
								HttpWebSocket.controllers.Add(controllerName, type);
							}
						}
					}
				}
			}
		}

		public static bool WebSocketControllerExists(HttpRequest request)
		{
			string[] urlSplit = request.Path.Split('/');
			if (urlSplit.Length != 1)
				return false;

			string controllerName = urlSplit[0];
			return controllers.ContainsKey(controllerName);
		}

		public static Type GetWebSocketController(HttpRequest request)
		{
			string[] urlSplit = request.Path.Split('/');
			if (urlSplit.Length != 1)
				return null;

			string controllerName = urlSplit[0];
			return (controllers.ContainsKey(controllerName)) ? controllers[controllerName] : null;
		}

		#endregion

		private TcpClient socket;
		private NetworkStream stream;
		public HttpRequest request;

		public int ID { get; private set; }

		private bool open;
		public bool Open
		{
			get
			{
				return open;
			}
			set
			{
				if (!value && open)
				{
					OnDisconnectedClient(this);
					try
					{
						this.socket.Close();
					}
					catch
					{

					}
				}

				open = value;
			}
		}

		public HttpWebSocket(HttpRequest request)
		{
			this.open = true;
			this.request = request;
			this.ID = nextID++;
			OnNewClient(this);
		}

		~HttpWebSocket()
		{
		}

		public void SetSocket(TcpClient socket, NetworkStream stream)
		{
			this.socket = socket;
			this.stream = stream;
		}

		abstract public void NewData(string json);

		public void ReadNewData()
		{
			// Read and parse request
			var buffer = new byte[0] { };
			// TODO: add request timeout
			while (!this.request.RequestError)
			{
				try
				{
					byte[] incomingPacket = this.request.ReadIncomingPacket(stream, socket);
					buffer = StringHelper.CombineByteArrays(buffer, incomingPacket);

					if (buffer.Length > 0 && incomingPacket.Length == 0)
					{
						OnFrameRecived(HttpWebSocketFrame.DecodeFrame(buffer));
						break;
					}
					else
					if (incomingPacket.Length != 0)
					{
						// wait until entire request is recived
						Thread.Sleep(1);
					}

					Thread.Sleep(1);
				}
				catch
				{
					this.Open = false;
					return;
				}
			}

			NewData(JsonHelper.BytesToJson(HttpWebSocketFrame.DecodeFrame(buffer).Payload));
		}

		private void Send(byte[] data)
		{
			// send responce
			lock (socket)
			{
				try
				{
					const int MAX_LENGTH = 8192;
					for (int b = 0; b <= data.Length; b += MAX_LENGTH)
					{
						int length = Math.Min(data.Length - b, MAX_LENGTH);
						this.stream.Write(data, b, length);
					}

					this.stream.Flush();
				}
				catch
				{
					this.Open = false;
				}
			}
		}

		public void SendString(string message)
		{
			Send(HttpWebSocketFrame.EncodeFrame(message).EncodedFrame);
		}

		#region events

		#region new connection

		public delegate void ConnectedClientHandler(HttpWebSocket client);
		private static ConnectedClientHandler clientConnected = null;
		public static event ConnectedClientHandler ClientConnected
		{
			add
			{
				clientConnected += value;
			}
			remove
			{
				clientConnected -= value;
			}
		}

		private static void OnNewClient(HttpWebSocket client)
		{
			try
			{
				ConnectedClientHandler handler;
				if (null != (handler = (ConnectedClientHandler)clientConnected))
				{
					handler(client);
				}
			}
			catch
			{
				client.Open = false;
			}
		}

		#endregion

		#region disconnected

		public delegate void ClientDisconnectedHandler(HttpWebSocket client);
		private static ClientDisconnectedHandler clientDisconnected = null;
		public static event ClientDisconnectedHandler ClientDisconnected
		{
			add
			{
				clientDisconnected += value;
			}
			remove
			{
				clientDisconnected -= value;
			}
		}

		private static void OnDisconnectedClient(HttpWebSocket client)
		{
			try
			{
				ClientDisconnectedHandler handler;
				if (null != (handler = (ClientDisconnectedHandler)clientDisconnected))
				{
					handler(client);
				}
			}
			catch
			{

			}
		}

		#endregion

		#region frame recived

		public delegate void FrameRecivedHandler(HttpWebSocketFrame frame);
		private FrameRecivedHandler frameRecived = null;
		public event FrameRecivedHandler FrameRecived
		{
			add
			{
				this.frameRecived += value;
			}
			remove
			{
				this.frameRecived -= value;
			}
		}

		private void OnFrameRecived(HttpWebSocketFrame frame)
		{
			try
			{
				FrameRecivedHandler handler;
				if (null != (handler = (FrameRecivedHandler)this.frameRecived))
				{
					handler(frame);
				}
			}
			catch
			{
				this.Open = false;
				Thread.Sleep(100);
			}
		}

		#endregion

		#endregion

	}
}
