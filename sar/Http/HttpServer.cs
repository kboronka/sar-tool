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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;

using sar.Tools;

namespace sar.Http
{
	public class HttpServer : HttpBase
	{
		private AutoResetEvent connectionWaitHandle;
		private TcpListener listener;
		
		protected int port;
		protected string root;
		
		#region properties

		public List<HttpConnection> Connections { get; private set; }
		public Dictionary<string, HttpSession> Sessions { get; private set; }

		public string Root { get { return root; } }
		
		public int Port { get { return port; } }
		
		public string FavIcon { get; set; }
		
		public HttpCache Cache { get; private set; }

		#endregion
		
		#region constructor

		public HttpServer(XML.Reader reader)
		{
			if (reader != null) this.Deserialize(reader);
			
			this.Start();
		}

		public HttpServer(int port, string wwwroot)
		{
			this.port = port;
			this.root = wwwroot;
			this.root = Path.GetFullPath(this.root);
			
			this.Start();
		}
		
		public HttpServer(int port) : this(port, ApplicationInfo.CurrentDirectory + @"views\") { }
		public HttpServer(string wwwroot) : this(sar.Socket.SocketHelper.FindAvailablePort(80, 100), wwwroot) { }
		public HttpServer() : this(sar.Socket.SocketHelper.FindAvailablePort(80, 100), ApplicationInfo.CurrentDirectory + @"views\") { }
		
		private void Start()
		{
			if (this.root.EndsWith(@"\", StringComparison.Ordinal))
			{
				this.root = StringHelper.TrimEnd(this.root);
			}
			
			if (!Directory.Exists(this.root)) Directory.CreateDirectory(this.root);
			
			this.Connections = new List<HttpConnection>();
			this.Sessions = new Dictionary<string, HttpSession>();
			
			this.Cache = new HttpCache(this);
			HttpController.LoadControllers();
			HttpWebSocket.LoadControllers();
			
			this.connectionWaitHandle = new AutoResetEvent(false);
			
			this.listenerLoopThread = new Thread(this.ListenerLoop);
			this.listenerLoopThread.IsBackground = true;
			this.listenerLoopThread.Start();
		}
		
		~HttpServer()
		{
			this.Stop();
		}
		
		public void Stop()
		{
			try
			{
				this.listenerLoopShutdown = true;
				connectionWaitHandle.Set();
				if (this.listenerLoopThread.IsAlive) this.listenerLoopThread.Join();
			}
			catch (Exception ex)
			{
				Program.Log(ex);
			}
		}
		
		#endregion
		
		#region settings
		
		private void Deserialize(XML.Reader reader)
		{
			try
			{
				string elementName = reader.Name;
				while (reader.Read() && (reader.NodeType != XmlNodeType.EndElement || reader.Name != elementName))
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						switch (reader.Name)
						{
							case "port":
								this.port = (int)reader.GetValueLong();
								break;
								
							case "wwwroot":
								this.root =  reader.GetValueString();
								this.root = Path.GetFullPath(this.root);
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
			}
		}
		
		public void Serialize(XML.Writer writer)
		{
			writer.WriteStartElement("http-server");
			writer.WriteElement("port", this.port);
			writer.WriteElement("wwwroot", this.root);
			writer.WriteEndElement();
		}
		
		#endregion
		
		#region service
		
		#region listners
		
		private Thread listenerLoopThread;
		private bool listenerLoopShutdown = false;
		
		private void ListenerLoop()
		{
			Thread.Sleep(300);
			this.listener = new TcpListener(IPAddress.Any, this.port);
			this.listener.Start();

			while (!listenerLoopShutdown)
			{
				try
				{
					IAsyncResult result =  this.listener.BeginAcceptTcpClient(this.AcceptTcpClientCallback, this.listener);
					this.connectionWaitHandle.WaitOne();
					this.connectionWaitHandle.Reset();
				}
				catch (Exception ex)
				{
					Program.Log(ex);
					Thread.Sleep(5000);
				}
			}
			
			// shutdown listner
			try
			{
				this.listener.Stop();
			}
			catch (Exception ex)
			{
				Program.Log(ex);
			}
			
			this.listener = null;
		}

		
		private void AcceptTcpClientCallback(IAsyncResult ar)
		{
			try
			{
				var connection = (TcpListener)ar.AsyncState;
				var client = connection.EndAcceptTcpClient(ar);
				
				connectionWaitHandle.Set();
				
				lock (Connections)
				{
					Connections.Add(new HttpConnection(this, client));
					Connections.RemoveAll(c =>
					                      {
					                      	if (c.Stopped)
					                      	{
					                      		c.Dispose();
					                      		return true;
					                      	}
					                      	
					                      	return false;
					                      });
				}
			}
			catch (Exception ex)
			{
				connectionWaitHandle.Set();
				Logger.Log(ex);
			}
		}
		
		#endregion
		
		#endregion
	}
}
