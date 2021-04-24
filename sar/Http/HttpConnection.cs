﻿/* Copyright (C) 2017 Kevin Boronka
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

using sar.Timing;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace sar.Http
{
	public class HttpConnection : IDisposable
	{
#if DEBUG
		public const int MAX_TIME = 3;
#else
		public const int MAX_TIME = 300;
#endif

		private Thread serviceRequestThread;
		private Thread timeoutMonitorThread;
		private Interval timeout;

		public bool Open { get; private set; }
		public bool Stopped { get; private set; }

		public HttpServer Parent { get; private set; }
		public NetworkStream Stream { get; private set; }
		public TcpClient Socket { get; set; }

		public HttpConnection(HttpServer parent, TcpClient socket)
		{
			this.Open = true;
			this.Socket = socket;
			this.Stream = socket.GetStream();

			this.Parent = parent;

			timeout = new Interval((MAX_TIME + 20) * 1000);
			var clientIp = ((IPEndPoint)socket.Client.RemoteEndPoint).Address.ToString();

			this.serviceRequestThread = new Thread(this.MonitorTimeout);
			this.serviceRequestThread.Name = "HttpConnection Service Request " + clientIp;
			this.serviceRequestThread.IsBackground = true;
			this.serviceRequestThread.Start();

			this.timeoutMonitorThread = new Thread(this.ServiceRequests);
			this.timeoutMonitorThread.Name = "HttpConnection Timeout Monitor " + clientIp;
			this.timeoutMonitorThread.Priority = ThreadPriority.Lowest;
			this.timeoutMonitorThread.IsBackground = true;
			this.timeoutMonitorThread.Start();
		}

		~HttpConnection()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		bool disposed = false;
		protected void Dispose(bool disposing)
		{
			if (disposed)
				return;

			if (disposing)
			{
				// abort thread
				try
				{
					serviceRequestThread.Abort();
				}
				catch
				{

				}

				// close connections
				try
				{
					this.Stream.Close();
					this.Socket.Close();
				}
				catch
				{

				}
			}

			disposed = true;
		}

		#region timeout monitor

		private void MonitorTimeout()
		{
			while (this.Open)
			{
				Thread.Sleep(1000);

				if (timeout.Ready)
				{
					this.Open = false;
				}
			}
		}

		#endregion

		#region service request

		private bool RequestReady()
		{
			lock (Socket)
			{
				lock (this.Stream)
				{
					if (Socket.Available > 0)
					{
						if (this.Stream.DataAvailable)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		private void ServiceRequests()
		{
			// Read and parse request
			while (this.Open)
			{
				try
				{
					if (this.Socket.Connected && this.RequestReady())
					{
						timeout.Reset();

						// return initial header
						lock (Socket)
						{
							const string INIT_HEADER = "HTTP/1.1";
							var bytes = Encoding.ASCII.GetBytes(INIT_HEADER);
							this.Stream.Write(bytes, 0, bytes.Length);
						}

						// process request and get responce
						var request = new HttpRequest(this);

						if (request.RequestError)
						{
							this.Open = false;
							break;
						}

						var response = request.Responce.bytes;

						// send responce
						lock (Socket)
						{
							try
							{
								const int MAX_LENGTH = 8192;
								for (int b = 0; b <= response.Length; b += MAX_LENGTH)
								{
									int length = Math.Min(response.Length - b, MAX_LENGTH);
									this.Stream.Write(response, b, length);
								}

								this.Stream.Flush();
							}
							catch
							{
								// TODO: close connection?
							}
						}

						if (request.IsWebSocket)
						{
							request.WebSocket.SetSocket(this.Socket, this.Stream);
							while (request.WebSocket.Open && this.Socket.Connected)
							{
								// reset timeout
								timeout.Reset();

								if (this.RequestReady())
								{
									request.WebSocket.ReadNewData();
								}

								Thread.Sleep(1);
							}

							request.WebSocket.Open = false;
						}
					}

					Thread.Sleep(1);
					this.Open &= this.Socket.Connected;
				}
				catch (Exception)
				{
					//Logger.Log(ex);
					this.Open = false;
				}
			}

			// close connections
			try
			{
				this.Stream.Close();
				this.Socket.Close();
			}
			catch
			{

			}

			this.Stopped = true;
		}

		#endregion

	}
}
