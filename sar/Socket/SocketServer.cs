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

using skylib.Tools;

namespace skylib.Socket
{
	public class SocketServer
	{
		private List<SocketClient> connections;
		private TcpListener listener;
		
		private int port;
		private Encoding encoding;
		private System.Threading.Timer serviceTimer;
		
		public List<SocketClient> Connections
		{
			get { return this.connections; }
		}
		
		public SocketServer(int port, Encoding encoding)
		{
			this.port = port;
			this.encoding = encoding;
			this.connections = new List<SocketClient>();
			this.listener = new TcpListener(IPAddress.Any, this.port);
			this.listener.Start();
			this.serviceTimer = new Timer(ServiceTick, null, 10, Timeout.Infinite);
		}
		
		private void ServiceListener()
		{
			try
			{
				lock (this.listener)
				{
					if (this.listener.Pending())
					{
						SocketClient newConnection = new SocketClient(this.listener.AcceptTcpClient(), this.encoding);
						this.connections.Add(newConnection);
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
		
		private void ServiceConnections()
		{
			lock (this.connections)
			{
				foreach (SocketClient connection in this.connections)
				{
					if (connection.HasRequest)
					{
						string request = connection.ReadString;
						
						if (!string.IsNullOrEmpty(request))
						{
							string command = StringHelper.FirstWord(request).ToLower();
							
							switch (command)
							{
								case "ping":
									connection.SendData("echo");
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
				this.ServiceConnections();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
			}
			finally
			{
				this.serviceTimer.Change(100, Timeout.Infinite );
			}
		}
	}
}
