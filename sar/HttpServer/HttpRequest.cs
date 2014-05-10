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

namespace sar.HttpServer
{
	public enum HttpMethod {GET, PUT, HEAD};
	public enum HttpStatusCode {OK = 200, FOUND = 302, NOTFOUND = 404, SERVERERROR=500};
	
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
			while (!incomingLoopShutdown && !incomingRequestRecived)
			{
				try
				{
					byte[] packetBytes = new byte[0] {};
					buffer = StringHelper.CombineByteArrays(buffer, this.ReadIncomingPacket());
					
					if (buffer.Length > 0)
					{
						this.ProcessIncomingBuffer(ref buffer);
					}
					
					if (!incomingLoopShutdown) Thread.Sleep(1);
				}
				catch (Exception ex)
				{
					incomingLoopShutdown = true;
					Program.Log(ex);
				}
			}
			

			// Construct responce
			// FIXME: send proper responce
			string content = "";
			string contentType = "text/html";
			byte[] contentBytes;
			
			content += "<html><body><h1>test server</h1>" + "\n";
			content += "<form method=post action=/form>" + "\n";
			content += "<input type=text name=foo value=foovalue>" + "\n";
			content += "<input type=submit name=bar value=barvalue>" + "\n";
			content += "</form>" + "\n";
			content += "</html>" + "\n";
			content += "\r\n";
			contentBytes = this.encoding.GetBytes(content);
			
			// Construct responce header
			string responce = "";
			HttpStatusCode statusCode = HttpStatusCode.OK;
			string responcePhrase = Enum.GetName(typeof(HttpStatusCode), statusCode);
			string version = "HTTP/1.0";
			
			string statusLine = version + " " + statusCode.ToString() + " " + responcePhrase + "\n\r";
			
			responce += statusLine;
			if (responce == null)
				return;
			responce += "Content-Type: " + contentType + "\n\r";
			responce += "Content-Length: " + contentBytes.Length.ToString() + "\n\r";
			responce += "Connection: close" + "\n\r";
			responce += "" + "\n\r";
			contentBytes = StringHelper.CombineByteArrays(Encoding.ASCII.GetBytes(responce), contentBytes);

			#if DEBUG
			string line = ">> ";
			foreach (byte chr in contentBytes)
			{
				line += chr.ToString() + " ";
				if (chr == 13)
				{
					Program.Log(line);
					line = ">> ";
				}
			}
			#endif
			
			// Send responce
			lock (socket)
			{
				stream.Write(contentBytes, 0, contentBytes.Length);
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
			
			string line = "";
			
			// Request Line
			string requestLine = ReadLine(ref bufferIn);
			if (string.IsNullOrEmpty(requestLine)) throw new InvalidDataException("request line missing");			
			Program.Log("<< \"" + requestLine + "\"");
			
			string[] initialRequest = line.Split(' ');
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

		/*
		private void oldServiceRequest()
		{
			try
			{
				ParseRequest();
				ReadHeaders();
				
				if (httpMethod.Equals("GET"))
				{
					handleGETRequest();
				}
				else if (httpMethod.Equals("POST"))
				{
					handlePOSTRequest();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: " + e.ToString());
				writeFailure();
			}
			
			
			outputStream.Flush();
			inputStream = null;
			outputStream = null;
			socket.Close();
		}
		 */
		
		#endregion
		
		#region handle request
		
		/*
		private Thread incomingLoopThread;
		
		private void IncomingLoop()
		{
			Thread.Sleep(0);
			string incomingBuffer = "";
			while (!incomingLoopShutdown)
			{
				try
				{
					incomingBuffer += this.ReadIncomingPacket();
					this.ProcessIncomingBuffer(ref incomingBuffer);
					Thread.Sleep(1);
				}
				catch (Exception ex)
				{
					this.Log(ex);
					Thread.Sleep(1000);
				}
			}
			
			this.Log(this.ID.ToString() +  ": " + "Incoming Loop Shutdown Gracefully");
		}
		

		 */
		#endregion
		
		#endregion
		
	}
}
