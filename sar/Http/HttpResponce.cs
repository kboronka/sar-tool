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
		public const string PDF_RENDER = "-pdf-render";
		private TcpClient socket;
		private NetworkStream stream;
		private Encoding encoding;

		private HttpRequest request;
		private HttpContent content;
		
		private bool pdfRender {get; set;}
		
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
				if (this.request.Path.ToLower().EndsWith(PDF_RENDER, StringComparison.CurrentCulture))
				{
					this.request.Path = StringHelper.TrimEnd(this.request.Path, PDF_RENDER.Length);
					this.pdfRender = true;
				}
				
				if (this.request.Path == @"")
				{
					if (HttpController.Primary == null) throw new ApplicationException("Primary Controller Not Defined");
					if (HttpController.Primary.PrimaryAction == null) throw new ApplicationException("Primary Action Not Defined");
	
					this.content = HttpController.RequestPrimary(this.request);
				}
				else if (this.request.Path.ToLower().EndsWith(@"-pdf", StringComparison.CurrentCulture))
				{
					string url = "http://localhost:" + request.Server.Port.ToString() + this.request.FullUrl + PDF_RENDER;
					
					url = url.Replace(this.request.Path, StringHelper.TrimEnd(this.request.Path, 4));
					
					this.content = HttpContent.GetPDF(url);
				}
				else if (this.request.Path.ToLower() == @"info")
				{
					this.content = HttpController.RequestAction("Debug", "Info", this.request);
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
			if (this.pdfRender) responce += "X-Content-Type-Options: " + "pdf-render" + "\n";

			// other
			responce += "Connection: close" + "\n";
			responce += "" + "\n";
			
			return StringHelper.CombineByteArrays(Encoding.ASCII.GetBytes(responce), contentBytes);
		}
	}
}
