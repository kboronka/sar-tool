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
		
		public byte[] Content
		{
			get { return ConstructResponce(HttpStatusCode.OK); }
		}
		
		public HttpResponce(HttpRequest request, TcpClient socket)
		{
			this.request = request;
			this.encoding = Encoding.ASCII;
			this.socket = socket;
			this.stream = this.socket.GetStream();
			
			try
			{
				if (this.request.Url.ToLower() == @"/info")
				{
					this.content = GetInfo();
				}
				else
				{
					HttpContent content = 
					this.content = HttpContent.Read(this.request.Server, this.request.Url);
				}
			}
			catch (Exception ex)
			{
				Program.Log(ex);
				this.content = GetException(ex, HttpStatusCode.SERVERERROR);
				this.ConstructResponce(HttpStatusCode.SERVERERROR);
			}
		}
		
		private HttpContent GetInfo()
		{
			// Construct responce
			string content = "";
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

			return new HttpContent(content);
		}
		
		private HttpContent GetException(Exception ex, HttpStatusCode status)
		{
			Exception inner = ExceptionHandler.GetInnerException(ex);
			
			Dictionary<string, HttpContent> baseContent = new Dictionary<string, HttpContent>() {};
			baseContent.Add("Title", new HttpContent(status.ToString()));
			baseContent.Add("ResponceCode", new HttpContent(((int)status).ToString()));
			baseContent.Add("ExceptionType", new HttpContent(inner.GetType().ToString()));
			baseContent.Add("ExceptionMessage", new HttpContent(inner.Message));
			baseContent.Add("ExceptionStackTrace", new HttpContent(ExceptionHandler.GetStackTrace(inner)));

			return HttpContent.Read("sar.Http.error.views.display.html", baseContent);
			                                                
			string content = "";
			content += "<html><body><h1>ERROR</h1>\n";
			content += ConsoleHelper.HR + "\r\n";
			content += "Time: " + DateTime.Now.ToString() + "\r\n";
			content += "Type: " + inner.GetType().ToString() + "\r\n";
			content += "Method: " + request.Method.ToString() + "\r\n";
			content += "URL: " + request.Url + "\r\n";
			content += "Version: " + request.ProtocolVersion + "\r\n";
			content += "Error: " + inner.Message + "\r\n";
			content += ConsoleHelper.HR + "\r\n";
			content += "<p>" + ExceptionHandler.GetStackTrace(inner) + "</p>\r\n";
			content += "</html>" + "\n";
			content += "\r\n";
			
			content = content.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
			content = content.Replace(Environment.NewLine, "<br>" + Environment.NewLine);
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
			responce += "Content-Length: " + contentBytes.Length.ToString() + "\n\r";
			
			// other
			responce += "Connection: close" + "\n\r";
			responce += "" + "\n\r";
			
			byte[] result = StringHelper.CombineByteArrays(Encoding.ASCII.GetBytes(responce), contentBytes);

			#if DEBUG
			/*
			string line = ">> ";
			foreach (byte chr in result)
			{
				line += chr.ToString() + " ";
				if (chr == 13)
				{
					Program.Log(line);
					line = ">> ";
				}
			}
			*/
			#endif

			return result;
		}
	}
}
