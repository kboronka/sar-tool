
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

using sar.Tools;

namespace sar.Http
{
	public class HttpConnection
	{
		public const int MAX_TIME = 300;

		private readonly System.Timers.Timer timeout;
		
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
			
			this.serviceRequestThread = new Thread(this.ServiceRequests);
			this.serviceRequestThread.IsBackground = true;
			this.serviceRequestThread.Start();

			
			timeout = new System.Timers.Timer();
			timeout.Interval = (MAX_TIME + 20) * 1000;
			timeout.Start();
			timeout.Elapsed += delegate { this.Open = false; };
		}
		
		~HttpConnection()
		{

		}
		
		#region service
		
		#region service request
		
		private Thread serviceRequestThread;
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
						// reset timeout
						timeout.Stop();
						timeout.Start();
						
						// return initial header
						lock (Socket)
						{
							const string INIT_HEADER = "HTTP/1.1";
							var bytes = Encoding.ASCII.GetBytes(INIT_HEADER);
							this.Stream.Write(bytes, 0, bytes.Length);
						}
						
						// process request and get responce
						var request = new HttpRequest(this);
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
					}
					
					Thread.Sleep(1);
					this.Open &= this.Socket.Connected;
				}
				catch (Exception ex)
				{
					Program.Log(ex);
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
		
		#endregion
	}
}
