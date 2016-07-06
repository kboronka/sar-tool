/* Copyright (C) 2016 Kevin Boronka
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
using System.Security.Cryptography;
using System.IO;
using System.Net.Sockets;
using System.Text;

using sar.Tools;

namespace sar.Http
{
	// http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
	public enum HttpStatusCode
	{
		[Description("Switching Protocols")]
		SWITCHING_PROTOCOLS = 101,

		[Description("OK")]
		OK = 200,

		FOUND = 302,
		
		[Description("Not Modified")]
		NOT_MODIFIED = 304,

		NOTFOUND = 404,

		SERVERERROR=500
	};

	public class HttpResponse
	{
		private readonly HttpRequest request;
		private HttpContent content;
		
		
		public byte[] bytes;
		
		public byte[] Bytes
		{
			get { return this.bytes; }
		}
		
		public HttpResponse(HttpRequest request)
		{
			this.request = request;
			const string PDF_IDENT = "-pdf";
			
			try
			{
				if (this.request.Path == @"")
				{
					if (HttpController.Primary == null) throw new ApplicationException("Primary Controller Not Defined");
					if (HttpController.Primary.PrimaryAction == null) throw new ApplicationException("Primary Action Not Defined");
					
					this.content = HttpController.RequestPrimary(this.request);
				}
				else if (this.request.Path.ToLower().EndsWith(PDF_IDENT, StringComparison.CurrentCulture))
				{
					string url = "http://localhost:" + request.Server.Port.ToString() + this.request.FullUrl;
					
					url = url.Replace(this.request.Path, StringHelper.TrimEnd(this.request.Path, PDF_IDENT.Length));
					
					this.content = new HttpContent(HtmlToPdfHelper.ReadPDF(url), "application/pdf");
				}
				else if (this.request.IsWebSocket && HttpWebSocket.WebSocketControllerExists(this.request))
				{
					var type = HttpWebSocket.GetWebSocketController(this.request);
					this.request.WebSocket = (HttpWebSocket)Activator.CreateInstance(type, this.request);
				}
				else if (HttpController.ActionExists(this.request))
				{
					this.content = HttpController.RequestAction(this.request);
				}
				else
				{
					this.content = HttpContent.Read(this.request.Server, this.request.Path);
				}
				
				if (this.content is HttpErrorContent)
				{
					this.bytes = this.ConstructResponse(HttpStatusCode.SERVERERROR);
				}
				else if (this.request.IsWebSocket)
				{
					this.bytes = this.ConstructResponse(HttpStatusCode.SWITCHING_PROTOCOLS);
				}
				else if (this.content.ETag == this.request.ETag && !this.content.ParsingRequired)
				{
					this.bytes = this.ConstructResponse(HttpStatusCode.NOT_MODIFIED);
				}
				else
				{
					this.bytes = this.ConstructResponse(HttpStatusCode.OK);
				}
			}
			catch (FileNotFoundException ex)
			{
				Program.Log(ex);
				this.content = ErrorController.Display(this.request, ex, HttpStatusCode.NOTFOUND);
				this.bytes = this.ConstructResponse(HttpStatusCode.SERVERERROR);
			}
			catch (Exception ex)
			{
				Program.Log(ex);
				this.content = ErrorController.Display(this.request, ex, HttpStatusCode.SERVERERROR);
				this.bytes = this.ConstructResponse(HttpStatusCode.SERVERERROR);
			}
		}
		
		private byte[] ConstructResponse(HttpStatusCode status)
		{
			// Construct response header
			
			const string GMT = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
			const string eol = "\r\n";
			string response = "";
			var contentBytes = new byte[] {};
			
			// status line
			string responsePhrase = Enum.GetName(typeof(HttpStatusCode), status);
			response += " " + ((int)status).ToString() + " " + responsePhrase + eol;
			
			if (!request.IsWebSocket)
			{
				response += @"Server: " + @"sar\" + AssemblyInfo.SarVersion + eol;
				response += @"Date: " + DateTime.UtcNow.ToString(GMT) + eol;
				response += @"ETag: " + this.content.ETag + eol;
				response += @"Set-Cookie: sarSession=" + this.request.Session.ID + @"; Path=/; expires=" + this.request.Session.ExpiryDate.ToString(GMT) + ";"	+ eol;
				response += @"Last-Modified: " + this.content.LastModified.ToString(GMT) + eol;
				if (this.request.PdfReader) response += "X-Content-Type-Options: " + "pdf-render" + eol;
				
				// content details
				if (status != HttpStatusCode.NOT_MODIFIED)
				{
					contentBytes = this.content.Render(request.Server.Cache);				
					response += @"Content-Type: " + this.content.ContentType + eol;
					response += @"Content-Length: " + (contentBytes.Length).ToString() + eol;
					//response += @"Expires: " + DateTime.UtcNow.AddDays(1).ToString(GMT) + eol;
				}
				
				/*
				response += @"Access-Control-Allow-Origin: *" + eol;
				response += @"Access-Control-Allow-Methods: POST, GET" + eol;
				response += @"Access-Control-Max-Age: 1728000" + eol;
				response += @"Access-Control-Allow-Credentials: true" + eol;
				 */
				
				// keep-alive
				response += "Keep-Alive: timeout=" + HttpConnection.MAX_TIME.ToString() + eol;
				response += "Connection: keep-alive";
				response += eol + eol;
			}
			else
			{
				response += @"Connection: Upgrade" + eol;
				response += @"Upgrade: websocket" + eol;

				var hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(this.request.WebSocketKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
				response += @"Sec-WebSocket-Accept: " + Convert.ToBase64String(hash) + eol;
				
				if (!String.IsNullOrEmpty(this.request.WebSocketProtocol))
				{
					response += @"Sec-WebSocket-Protocol: " + this.request.WebSocketProtocol + eol;
				}
				
				response += eol;
			}
			
			
			return (contentBytes.Length > 0) ? StringHelper.CombineByteArrays(Encoding.ASCII.GetBytes(response), contentBytes) : Encoding.ASCII.GetBytes(response);
		}
	}
}
