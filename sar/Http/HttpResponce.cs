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
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text;

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
				if (this.request.FullUrl == @"/" && HttpController.Primary != null && HttpController.Primary.PrimaryAction != null)
				{
					this.content = HttpController.RequestPrimary(this.request);
				}
				else if (this.request.FullUrl.ToLower() == @"/info")
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
				
				if (this.content is HttpErrorContent)
				{
					this.bytes = this.ConstructResponce(HttpStatusCode.SERVERERROR);
				}
				else
				{
					this.bytes = this.ConstructResponce(HttpStatusCode.OK);
				}
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
			string responce = "";
			responce += "<html><body><h1>test server</h1>" + "<br>\n";
			responce += "Method: " + request.Method.ToString() + "<br>\n";
			responce += "URL: " + request.FullUrl + "<br>\n";
			responce += "Version: " + request.ProtocolVersion + "<br>\n";
			responce += "<form method=post action=/form>" + "<br>\n";
			responce += "<input type=text name=foo value=foovalue>" + "<br>\n";
			responce += "<input type=submit name=bar value=barvalue>" + "<br>\n";
			responce += "</form>" + "\n";
			responce += "</html>" + "\n";
			responce += "\r\n";

			return new HttpContent(responce);
		}
		
		private byte[] ConstructResponce(HttpStatusCode status)
		{
			// Construct responce header

			// status line
			string responcePhrase = Enum.GetName(typeof(HttpStatusCode), status);
			string responce = "HTTP/1.0" + " " + ((int)status).ToString() + " " + responcePhrase + "\n";
			
			byte [] contentBytes = this.content.Render();
			// content details
			responce += "Content-Type: " + this.content.ContentType + "\n";
			responce += "Content-Length: " + (contentBytes.Length).ToString() + "\n";
			
			// other
			responce += "Connection: close" + "\n";
			responce += "" + "\n";
			
			return StringHelper.CombineByteArrays(Encoding.ASCII.GetBytes(responce), contentBytes);
		}
	}
}
