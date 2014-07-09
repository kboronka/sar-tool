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
			
			try
			{
				// FIXME: send proper responce
				string filePath = this.request.Server.Root + this.request.Url.Replace(@"/", @"\");
				if (File.Exists(filePath))
				{
					this.content = GetFile(filePath);
				}
				else if (this.request.Url.ToLower() == @"/info")
				{
					this.content = GetInfo();
				}
				else
				{
					//this.SendInfo();
					throw new FileNotFoundException("did not find " + filePath);
				}
			}
			catch (Exception ex)
			{
				Program.Log(ex);
				this.content = GetException(ex);
				this.ConstructResponce(HttpStatusCode.SERVERERROR);
			}
		}
		
		private byte[] GetFile(string filepath)
		{
			string extension = IO.GetFileExtension(filepath).ToLower();
			
			this.contentType = HttpHelper.GetMimeType(extension);
			
			if (extension == "php")
			{
				return GetPHP(filepath);
			}
			
			return File.ReadAllBytes(filepath);
		}
		
		private byte[] GetInfo()
		{
			this.contentType = "text/html";
			
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

			return this.encoding.GetBytes(content);
		}
		
		private byte[] GetException(Exception ex)
		{
			this.contentType = "text/html";
			Exception inner = ExceptionHandler.GetInnerException(ex);
			
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
			return this.encoding.GetBytes(content);
		}
		
		private byte[] ConstructResponce(HttpStatusCode status)
		{
			// Construct responce header

			// status line
			string responcePhrase = Enum.GetName(typeof(HttpStatusCode), status);
			string responce = "HTTP/1.0" + " " + status.ToString() + " " + responcePhrase + "\n\r";
			
			// content details
			responce += "Content-Type: " + this.contentType + "\n\r";
			responce += "Content-Length: " + this.content.Length.ToString() + "\n\r";
			
			// other
			responce += "Connection: close" + "\n\r";
			responce += "" + "\n\r";
			
			this.content = StringHelper.CombineByteArrays(Encoding.ASCII.GetBytes(responce), this.content);

			#if DEBUG
			string line = ">> ";
			foreach (byte chr in this.content)
			{
				line += chr.ToString() + " ";
				if (chr == 13)
				{
					Program.Log(line);
					line = ">> ";
				}
			}
			#endif

			return this.content;
		}
		
		private static string phpPath;
		private byte[] GetPHP(string filepath)
		{
			// locate php.exe
			if (String.IsNullOrEmpty(phpPath))
			{
				if (File.Exists(@"c:\php\php.exe"))
				{
					phpPath = @"c:\php\php.exe";
				}
				else
				{
					phpPath = sar.Tools.IO.FindApplication("php.exe");
				}
			}
			
			if (String.IsNullOrEmpty(phpPath)) throw new ApplicationException("PHP not found");

			string output;
			string error;
			
			ConsoleHelper.Run(phpPath, filepath, out output, out error);
			// TODO: handle errors
			return encoding.GetBytes(output);
		}
	}
}
