﻿
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
	public class HttpRequest : HttpBase
	{
		private TcpClient socket;
		private NetworkStream stream;
		private Encoding encoding;
		
		private HttpServer parent;

		private String httpMethod;
		private String httpUrl;
		private String httpProtocolVersion;
		
		private bool headerRecived;
		
		#region properties

		public string HttpMethod
		{
			get { return httpMethod; }
		}

		public string HttpUrl
		{
			get { return httpUrl; }
		}

		public string HttpProtocolVersion
		{
			get { return httpProtocolVersion; }
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
				this.Log(ex);
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
			byte[] buffer = new byte[0] {};
			
			while (!incomingLoopShutdown && !incomingRequestRecived)
			{
				try
				{
					byte[] packetBytes = new byte[0] {};
					buffer = StringHelper.CombineByteArrays(buffer, this.ReadIncomingPacket());
					
					this.ProcessIncomingBuffer(ref buffer);
					if (!incomingLoopShutdown) Thread.Sleep(1);
				}
				catch (Exception ex)
				{
					incomingLoopShutdown = true;
					this.Log(ex);
				}
			}
			
			// FIXME: send proper responce
			string result = "";
			string content = "";
			string contentType = "text/html";
			content += "<html><body><h1>test server</h1>" + "\n";
			content += "<form method=post action=/form>" + "\n";
			content += "<input type=text name=foo value=foovalue>" + "\n";
			content += "<input type=submit name=bar value=barvalue>" + "\n";
			content += "</form>" + "\n";
			content += "</html>" + "\n";
			content += "\r\n";
			byte[] contentBytes = this.encoding.GetBytes(content);

			lock (socket)
			{
				StreamWriter output = new StreamWriter(new BufferedStream(socket.GetStream()));
				
				output.WriteLine("HTTP/1.0 200 OK");
				output.WriteLine("Content-Type: " + contentType);
				output.WriteLine("Content-Length: " + contentBytes.Length.ToString());
				output.WriteLine("Connection: close");
				output.WriteLine("");
				output.Flush();	
								
				stream.Write(contentBytes, 0, contentBytes.Length);
				stream.Flush();
				
				output = null;
				stream = null;

				result += "" + "\n";
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
				this.Log(ex);
				// The NetworkStream is closed.
				//this.Disconnect();
			}
			catch (IOException ex)
			{
				this.Log(ex);
				// The underlying Socket is closed.
				//this.Disconnect();
			}
			catch (SocketException)
			{
				
			}
			catch (Exception ex)
			{
				this.Log(ex);
			}
			
			return new byte[0] {};
		}
		
		private void ProcessIncomingBuffer(ref byte[] bufferIn)
		{
			ReadHeader(ref bufferIn);
			if (!headerRecived) return;
			
			// TODO: read binary data from POST
			incomingRequestRecived = true;
		}
		
		private void ReadHeader(ref byte[] bufferIn)
		{
			if (headerRecived) return;
			string line = "";
			
			do
			{
				line = ReadLine(ref bufferIn);
				
				if (string.IsNullOrEmpty(line))
				{
					headerRecived = true;
				}
				
				// TODO: parse header line here
			} while (!headerRecived || line == "fault");
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
			
			return "fault";
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
