/* Copyright (C) 2016 Kevin Boronka
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
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;

using sar.Tools;

namespace sar.Http
{
	public abstract class HttpWebSocket
	{
		#region static
		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
		public class SarWebSocketController : Attribute { }
		
		private static Dictionary<string, Type> controllers;
		
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
			if (urlSplit.Length != 1) return false;
			
			string controllerName = urlSplit[0];
			return controllers.ContainsKey(controllerName);
		}
		
		public static Type GetWebSocketController(HttpRequest request)
		{
			string[] urlSplit = request.Path.Split('/');
			if (urlSplit.Length != 1) return null;
			
			string controllerName = urlSplit[0];
			return (controllers.ContainsKey(controllerName)) ? controllers[controllerName] : null;
		}
		
		#endregion
		
		private TcpClient socket;
		private	NetworkStream stream;
		public HttpRequest request;
		
		public bool Open { get; private set; }
		
		public HttpWebSocket(HttpRequest request)
		{
			this.Open = true;
			this.request = request;
		}
		
		public void SetSocket(TcpClient socket, NetworkStream stream)
		{
			this.socket = socket;
			this.stream = stream;
		}
		
		abstract public void NewData(byte[] data);
		
		public void ReadNewData()
		{
			// Read and parse request
			var buffer = new byte[0] {};
			// TODO: add request timeout
			while (true)
			{
				try
				{
					byte[] incomingPacket = this.request.ReadIncomingPacket(stream, socket);
					buffer = StringHelper.CombineByteArrays(buffer, incomingPacket);
					
					if (buffer.Length > 0 && incomingPacket.Length == 0)
					{
						break;
					}
					else if (incomingPacket.Length != 0)
					{
						// wait until entire request is recived
						Thread.Sleep(1);
					}
					
					Thread.Sleep(1);
				}
				catch (Exception ex)
				{
					Program.Log(ex);
					return;
				}
			}
			
			NewData(buffer);
		}
		

		
		public void Send(byte[] data)
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
					// TODO: close connection?
				}
			}
		}
	}
}
