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
		private HttpContent content;
		public byte[] bytes;
		
		public byte[] Bytes
		{
			get { return this.bytes; }
		}
		
		public HttpResponce(HttpRequest request, TcpClient socket)
		{
			this.request = request;
			this.encoding = Encoding.ASCII;
			this.socket = socket;
			this.stream = this.socket.GetStream();
			
			try
			{
				if (this.request.FullUrl.ToLower() == @"/info")
				{
					this.content = GetInfo();
				}
				else if (HttpController.ActionExists(this.request))
				{
					this.content = HttpController.RequestAction(this.request);
				}
				else
				{
					this.content = HttpContent.Read(this.request.Server, this.request.FullUrl);
				}
				
				this.bytes = this.ConstructResponce(HttpStatusCode.OK);
			}
			catch (FileNotFoundException ex)
			{
				Program.Log(ex);
				this.content = ErrorController.Display(this.request, ex, HttpStatusCode.NOTFOUND);
				this.bytes = this.ConstructResponce(HttpStatusCode.SERVERERROR);			}
			catch (Exception ex)
			{
				Program.Log(ex);
				this.content = ErrorController.Display(this.request, ex, HttpStatusCode.SERVERERROR);
				this.bytes = this.ConstructResponce(HttpStatusCode.SERVERERROR);
			}
		}
		
		private HttpContent GetInfo()
		{
			// Construct responce
			string content = "";
			content += "<html><body><h1>test server</h1>" + "<br>\n";
			content += "Method: " + request.Method.ToString() + "<br>\n";
			content += "URL: " + request.FullUrl + "<br>\n";
			content += "Version: " + request.ProtocolVersion + "<br>\n";
			content += "<form method=post action=/form>" + "<br>\n";
			content += "<input type=text name=foo value=foovalue>" + "<br>\n";
			content += "<input type=submit name=bar value=barvalue>" + "<br>\n";
			content += "</form>" + "\n";
			content += "</html>" + "\n";
			content += "\r\n";

			return new HttpContent(content);
		}
		
		private byte[] ConstructResponce(HttpStatusCode status)
		{
			// Construct responce header

			// status line
			string responcePhrase = Enum.GetName(typeof(HttpStatusCode), status);
			string responce = "HTTP/1.0" + " " + status.ToString() + " " + responcePhrase + "\n\r";
			
			byte [] contentBytes = this.content.Render();
			// content details
			responce += "Content-Type: " + this.content.ContentType + "\n\r";
			responce += "Content-Length: " + (contentBytes.Length + 1).ToString() + "\n\r";
			
			// other
			responce += "Connection: close" + "\n\r";
			responce += "" + "\n\r";
			
			return StringHelper.CombineByteArrays(Encoding.ASCII.GetBytes(responce), contentBytes);
		}
	}
}
