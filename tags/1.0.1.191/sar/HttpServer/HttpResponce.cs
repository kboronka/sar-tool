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
	public enum HttpStatusCode
	{
		[Description("OK")]
		OK = 200,
		FOUND = 302,
		NOTFOUND = 404,
		SERVERERROR=500
	};

	public class HttpResponce
	{
		private TcpClient socket;
		private NetworkStream stream;
		private Encoding encoding;

		private HttpRequest request;
		private string contentType = "text/html";
		private byte[] content;
		
		public byte[] Content
		{
			get { return content; }
		}
		
		public HttpResponce(HttpRequest request, TcpClient socket)
		{
			this.request = request;
			this.encoding = Encoding.ASCII;
			this.socket = socket;
			this.stream = this.socket.GetStream();
			
			// FIXME: send proper responce
			string filePath = this.request.Server.Root + this.request.Url.Replace("/", "\\");
			if (File.Exists(filePath))
			{
				// TODO: replace with SendFile()
				this.SendInfo();
			}
			else
			{
				this.SendInfo();				
			}
		}
		
		private void SendInfo()
		{
			// Construct responce
			string content = "";
			string contentType = "text/html";
			byte [] contentBytes;
			
			content += "<html><body><h1>test server</h1>" + "<br>\n";
			content += "Method: " + request.Method.ToString() + "<br>\n";
			content += "URL: " + request.Url + "<br>\n";
			content += "Version: " + request.ProtocolVersion + "<br>\n";
			content += "<form method=post action=/form>" + "<br>\n";
			content += "<input type=text name=foo value=foovalue>" + "<br>\n";
			content += "<input type=submit name=bar value=barvalue>" + "<br>\n";
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
			
			this.content = StringHelper.CombineByteArrays(Encoding.ASCII.GetBytes(responce), contentBytes);

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

		}
		
		/*
		private string Header()
		{
			string header;
			
		}
		 */
	}
}
