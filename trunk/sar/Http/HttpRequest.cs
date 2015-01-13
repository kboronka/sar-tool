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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using sar.Tools;

namespace sar.Http
{
	public enum HttpMethod {GET, PUT, HEAD};
	
	public class HttpRequest : HttpBase
	{
		private TcpClient socket;
		private NetworkStream stream;
		private Encoding encoding;
		
		private HttpServer parent;

		private HttpMethod method;
		private String url;
		private String protocolVersion;
		
		private bool headerRecived;
		
		#region properties

		public HttpMethod Method
		{
			get { return method; }
		}

		public string Url
		{
			get { return url; }
		}

		public string ProtocolVersion
		{
			get { return protocolVersion; }
		}
		
		public HttpServer Server
		{
			get { return parent; }
		}
		
		#endregion
		
		#region constructor
		
		public HttpRequest(HttpServer parent, TcpClient socket)
		{
			this.encoding = Encoding.ASCII;
			this.parent = parent;
			this.socket = socket;
			this.stream = this.socket.GetStream();
			
			this.serviceRequestThread = new Thread(this.ServiceRequest);
			this.serviceRequestThread.IsBackground = true;
			this.serviceRequestThread.Start();
		}
		
		~HttpRequest()
		{
			this.Stop();
		}
		
		public void Stop()
		{
			try
			{
				this.incomingLoopShutdown = true;
				if (this.serviceRequestThread.IsAlive) this.serviceRequestThread.Join();
			}
			catch (Exception ex)
			{
				Program.Log(ex);
			}
		}
		
		#endregion
		
		#region service
		
		#region service request
		
		private Thread serviceRequestThread;
		private bool incomingLoopShutdown;
		private bool incomingRequestRecived;

		private void ServiceRequest()
		{
			// Read and parse request
			byte[] buffer = new byte[0] {};
			// TODO: add request timeout
			while (!incomingLoopShutdown && !incomingRequestRecived)
			{
				try
				{
					byte[] incomingPacket = this.ReadIncomingPacket();
					buffer = StringHelper.CombineByteArrays(buffer, incomingPacket);
					
					if (buffer.Length > 0 && incomingPacket.Length == 0)
					{
						// buffer is complete
						this.ProcessIncomingBuffer(ref buffer);
					}
					else if (incomingPacket.Length != 0)
					{
						// wait until entire request is recived
						Thread.Sleep(50);
					}
					
					if (!incomingLoopShutdown) Thread.Sleep(1);
				}
				catch (Exception ex)
				{
					incomingLoopShutdown = true;
					Program.Log(ex);
				}
			}
			
			HttpResponce responce = new HttpResponce(this, socket);
			
			lock (socket)
			{
				stream.Write(responce.Bytes, 0, responce.Bytes.Length);
				stream.Flush();
				stream = null;
				socket.Close();
			}			
		}
		
		private byte[] ReadIncomingPacket()
		{
			try
			{
				if (socket == null) return new byte[0] {};
				if (stream == null) return new byte[0] {};
				
				lock (socket)
				{
					lock (stream)
					{
						if (socket.Available > 0)
						{
							if (stream.DataAvailable)
							{
								byte[] packetBytes = new byte[socket.Available];
								stream.Read(packetBytes, 0, packetBytes.Length);
								return packetBytes;
								//return this.encoding.GetString(packetBytes, 0, packetSize);
							}
						}
					}
				}
			}
			catch (ObjectDisposedException ex)
			{
				Program.Log(ex);
				// The NetworkStream is closed.
				//this.Disconnect();
			}
			catch (IOException ex)
			{
				Program.Log(ex);
				// The underlying Socket is closed.
				//this.Disconnect();
			}
			catch (SocketException)
			{
				
			}
			catch (Exception ex)
			{
				Program.Log(ex);
			}
			
			return new byte[0] {};
		}
		
		private void ProcessIncomingBuffer(ref byte[] bufferIn)
		{
			ReadRequest(ref bufferIn);
			if (!headerRecived) return;
			
			// TODO: read binary data from POST
			incomingRequestRecived = true;
		}
		
		private void ReadRequest(ref byte[] bufferIn)
		{
			if (headerRecived) return;
			Program.Log("Reading Header");
			
			// Request Line
			string requestLine = ReadLine(ref bufferIn);
			if (string.IsNullOrEmpty(requestLine)) throw new InvalidDataException("request line missing");			
			Program.Log("<< \"" + requestLine + "\"");
			
			string[] initialRequest = requestLine.Split(' ');
			if (initialRequest.Length != 3) throw new InvalidDataException("the initial request line should contain three fields");

			this.url = CleanUrlString(initialRequest[1]);
			this.protocolVersion = initialRequest[2];
			
			switch (initialRequest[0].ToUpper())
			{
				case "GET":
					this.method = HttpMethod.GET;
					break;
				case "PUT":
					this.method = HttpMethod.PUT;
					break;
					//TODO: handle the HEAD request type
				default:
					throw new InvalidDataException("unknown request type \"" + initialRequest[0] + "\"");
			}
			
			string line = "";			
			while (!this.headerRecived)
			{
				line = ReadLine(ref bufferIn);
				Program.Log("<< \"" + line + "\"");
				
				this.headerRecived = string.IsNullOrEmpty(line);
				if (this.headerRecived) break;
				
				// TODO: parse common request Headers
				// Header format
				// Name: value
				
				// examples:
				// Host: example.com
				// User-Agent: chrome v17
			}
		}
		
		private string ReadLine(ref byte[] bufferIn)
		{
			for (int i = 0; i < bufferIn.Length; i++)
			{
				if (bufferIn[i] == '\n')
				{
					byte[] newBufferIn = new Byte[bufferIn.Length - i - 1];
					System.Buffer.BlockCopy(bufferIn, i + 1, newBufferIn, 0, newBufferIn.Length);
					
					if (i > 0 && bufferIn[i - 1] == '\r') i--;
					
					string line = this.encoding.GetString(bufferIn, 0, i);

					bufferIn = newBufferIn;
					return line;
				}
			}
			
			return null;
		}
		
		private string CleanUrlString(string url)
		{
			while (url.Contains("%"))
			{
				int index = url.LastIndexOf('%');
				string characterCode = url.Substring(index + 1, 2);
				char character = (char)Convert.ToInt32(characterCode, 16);
				
				url = url.Substring(0, index) + character.ToString() + url.Substring(index + 3);
			}
			
			return url;
		}

		#endregion
		
		#endregion
		
	}
}
