/* Copyright (C) 2015 Kevin Boronka
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
using System.Xml;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using sar.Tools;

namespace sar.Http
{
	public class HttpServer : HttpBase
	{
		private TcpListener listener;
		protected int port;
		protected string root;
		private string favIcon;
		
		#region properties

		public string Root
		{
			get { return root; }
		}
		
		public int Port
		{
			get { return port; }
		}
		
		public string FavIcon { get; set; }

		#endregion
		
		#region constructor

		public HttpServer(int port, string wwwroot)
		{
			this.port = port;
			this.root = wwwroot;
			
			this.Start();
		}
		
		public HttpServer(int port)
		{
			this.port = port;
			this.root = ApplicationInfo.CurrentDirectory + @"\views\";
			
			this.Start();
		}

		public HttpServer(string wwwroot)
		{
			this.port = sar.Socket.SocketHelper.FindAvailablePort(80, 100);
			this.root = wwwroot;
			
			this.Start();
		}
		
		public HttpServer(XML.Reader reader)
		{
			if (reader != null) this.Deserialize(reader);
			this.Start();
		}
				
		public HttpServer()
		{
			this.port = sar.Socket.SocketHelper.FindAvailablePort(80, 100);
			this.root = ApplicationInfo.CurrentDirectory + @"\views\";
			this.Start();
		}
		
		private void Start()
		{
			if (this.root.EndsWith(@"\", StringComparison.Ordinal))
			{
				this.root = StringHelper.TrimEnd(this.root);
			}
			
			if (!Directory.Exists(this.root)) Directory.CreateDirectory(this.root);
			
			HttpController.LoadControllers();
			
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
				while (reader.Read())
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
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Base.Program.Log(ex);
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
			while (!listenerLoopShutdown)
			{
				try
				{
					if (this.listener == null)
					{
						this.listener = new TcpListener(IPAddress.Any, this.port);
						this.listener.Start();
					}
					else
					{
						this.ServiceListener();
					}
					
					Thread.Sleep(1);
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

		private void ServiceListener()
		{
			lock (this.listener)
			{
				if (this.listener.Pending())
				{
					var client = new HttpRequest(this, this.listener.AcceptTcpClient());
				}
			}
		}
		
		#endregion
		
		#endregion
	}
}
