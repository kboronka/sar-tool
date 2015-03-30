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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


using sar.Tools;

namespace sar.Http
{
	public class HttpErrorContent : HttpContent
	{
		public HttpErrorContent(Exception ex) : base()
		{
			Exception inner = ExceptionHelper.GetInner(ex);
			string message = inner.Message;
			
			message = inner.Message;
			message += Environment.NewLine;
			message += ExceptionHelper.GetStackTrace(inner);
			
			this.content = message.ToBytes();
			this.contentType = "application/json";
		}
	}
	
	public class HttpContent
	{
		#region static

		public static HttpContent Read(HttpRequest request, string requestView)
		{
			return Read(request, requestView, new Dictionary<string, HttpContent>() {});
		}
		
		public static HttpContent Read(HttpRequest request, string requestView, Dictionary<string, HttpContent> baseContent)
		{
			return Read(request.Server, requestView, baseContent);
		}

		public static HttpContent Read(HttpServer server, string request)
		{
			return Read(server, request, new Dictionary<string, HttpContent>() {});
		}
		
		public static HttpContent Read(HttpServer server, string request, Dictionary<string, HttpContent> baseContent)
		{
			request = request.TrimWhiteSpace();
			string filePath = server.Root + @"\" + request.Replace(@"/", @"\");
			
			if (File.Exists(filePath))
			{
				return HttpContent.ReadFile(filePath, baseContent);
			}
			else if (EmbeddedResource.Contains(request))
			{
				return HttpContent.ReadEmbeddedFile(request, baseContent);
			}
			else if (filePath.EndsWith("favicon.ico") &&
			         server.FavIcon.IsNotNull() &&
			         filePath != server.FavIcon)
			{
				return HttpContent.ReadFile(server.FavIcon, baseContent);
			}
			else if (filePath.EndsWith("favicon.ico") &&
			         EmbeddedResource.Contains("sar.Http.libs.art.favicon.ico"))
			{
				return HttpContent.ReadEmbeddedFile("sar.Http.libs.art.favicon.ico", baseContent);
			}
			else if (filePath.EndsWith("sar.Http.Views.Error.Display.html", StringComparison.InvariantCulture))
			{
				return new HttpContent("error locating error template");
			}
			else
			{
				throw new FileNotFoundException("did not find " + filePath);
			}
		}
		
		private static HttpContent Read(string request)
		{
			return Read(request, new Dictionary<string, HttpContent>() {});
		}
		
		private static HttpContent Read(string request, Dictionary<string, HttpContent> baseContent)
		{
			request = request.TrimWhiteSpace();
			
			if (File.Exists(request))
			{
				return HttpContent.ReadFile(request, baseContent);
			}
			else if (EmbeddedResource.Contains(request))
			{
				return HttpContent.ReadEmbeddedFile(request, baseContent);
			}
			else
			{
				throw new FileNotFoundException("did not find " + request);
			}
		}
		
		private static HttpContent ReadFile(string filePath, Dictionary<string, HttpContent> baseContent)
		{
			string extension = IO.GetFileExtension(filePath).ToLower();
			string contentType = HttpHelper.GetMimeType(extension);
			byte[] content = File.ReadAllBytes(filePath);
			
			return new HttpContent(content, contentType, baseContent);
		}
		
		private static HttpContent ReadEmbeddedFile(string resource, Dictionary<string, HttpContent> baseContent)
		{
			if (EmbeddedResource.Contains(resource))
			{
				string extension = IO.GetFileExtension(resource).ToLower();
				string contentType = HttpHelper.GetMimeType(extension);
				byte[] content = EmbeddedResource.Get(resource);
				
				return new HttpContent(content, contentType, baseContent);
			}
			
			return new HttpContent();
		}
		
		private static byte[] GetFile(string filepath)
		{
			string extension = IO.GetFileExtension(filepath).ToLower();
			if (extension == "php")
			{
				return GetPHP(filepath);
			}
			else
			{
				return File.ReadAllBytes(filepath);
			}
		}
		
		private static string phpPath;
		private static byte[] GetPHP(string filepath)
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
			return Encoding.ASCII.GetBytes(output);
		}
		
		#endregion
		
		#region PDF Convert Tool
		
		// documentation: http://wkhtmltopdf.org/usage/wkhtmltopdf.txt
		private static string pdfWriter;
		
		private static string PdfWriter
		{
			get
			{
				if (string.IsNullOrEmpty(pdfWriter))
				{
					pdfWriter = IO.FindApplication("wkhtmltopdf.exe", @"wkhtmltopdf\bin");
				}
				
				return pdfWriter;
			}
		}
		
		public static HttpContent GetPDF(string url)
		{
			if (string.IsNullOrEmpty(PdfWriter)) throw new ApplicationException("PDF output is not supported");
			
			string tempfile = ApplicationInfo.DataDirectory + Guid.NewGuid().ToString() + ".pdf";
			string output = "";
			
			int exitCode = ConsoleHelper.Run(PdfWriter, url.QuoteDouble()+ " " + tempfile.QuoteDouble(), out output);
			
			
			HttpContent result = HttpContent.Read(tempfile);
			File.Delete(tempfile);
			
			return result;
		}
		
		#endregion

		protected byte[] content;
		protected string contentType;
		protected Dictionary<string, HttpContent> baseContent;
		
		private string RenderText(Dictionary<string, HttpContent> baseContent)
		{
			return StringHelper.GetString(Render(baseContent));
		}

		public byte[] Render()
		{
			return Render(baseContent);
		}
		
		public byte[] Render(Dictionary<string, HttpContent> baseContent)
		{
			if (this.contentType.Contains("text") || this.contentType.Contains("xml"))
			{
				string text = Encoding.ASCII.GetString(this.content);
				
				// include linked externals
				MatchCollection matches = Regex.Matches(text, @"\<%@ Include:\s*([^@]+)\s*\%\>");
				if (matches.Count > 0)
				{
					foreach (Match match in matches)
					{
						string key = match.Groups[1].Value.TrimWhiteSpace();
						string replacmentContent = HttpContent.Read(key, baseContent).RenderText(baseContent);
						text = Regex.Replace(text, match.Groups[0].Value, replacmentContent);
					}
				}
				
				// include linked externals
				matches = Regex.Matches(text, @"\<%@ Content:\s*([^@]+)\s*\%\>");
				if (matches.Count > 0)
				{
					foreach (Match match in matches)
					{
						string key = match.Groups[1].Value.TrimWhiteSpace();
						if (baseContent.ContainsKey(key))
						{
							HttpContent replacmentContent = baseContent[key];
							text = Regex.Replace(text, match.Groups[0].Value, replacmentContent.RenderText(baseContent));
						}
					}
				}
				
				return Encoding.ASCII.GetBytes(text);
			}
			
			return this.content;
		}
		
		public string ContentType
		{
			get { return this.contentType; }
		}
		
		protected HttpContent()
		{
			this.baseContent = new Dictionary<string, HttpContent>();
			this.contentType = "text/plain";
		}
		
		public HttpContent(string content)
		{
			this.baseContent = new Dictionary<string, HttpContent>();
			this.contentType = "text/plain";
			this.content = Encoding.ASCII.GetBytes(content);
		}
		
		public HttpContent(Dictionary<string, object> json)
		{
			this.baseContent = new Dictionary<string, HttpContent>();
			this.contentType = "text/plain";
			this.content = Encoding.ASCII.GetBytes(json.ToJSON());
		}
		
		public HttpContent(List<Dictionary<string, object>> json)
		{
			this.baseContent = new Dictionary<string, HttpContent>();
			this.contentType = "text/plain";
			this.content = Encoding.ASCII.GetBytes(json.ToJSON());
		}

		public HttpContent(string content, string contentType)
		{
			this.baseContent = new Dictionary<string, HttpContent>();
			this.contentType = contentType;
			this.content = Encoding.UTF8.GetBytes(content);
		}

		public HttpContent(byte[] content, string contentType)
		{
			this.baseContent = new Dictionary<string, HttpContent>();
			this.contentType = contentType;
			this.content = content;
		}

		
		public HttpContent(byte[] content, string contentType, Dictionary<string, HttpContent> baseContent)
		{
			this.baseContent = baseContent;
			this.contentType = contentType;
			this.content = content;
		}
	}
}
