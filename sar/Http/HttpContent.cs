/* Copyright (C) 2017 Kevin Boronka
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
using sar.Json;

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
			this.ContentType = "application/json";
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
			request = request.TrimWhiteSpace().Replace(@"/", @"\").ToLower();
			string filePath = server.Root + @"\" + request;
			
			if (server.Cache.Contains(request))
			{
				return new HttpContent(server.Cache.Get(request), baseContent);
			}
			else if (File.Exists(filePath))
			{
				return new HttpContent(server.Cache.Get(filePath), baseContent);
			}
			else if (filePath.EndsWith("favicon.ico"))
			{
				return new HttpContent(server.Cache.Get("sar.http.libs.art.favicon.ico"), baseContent);
			}
			else
			{
				throw new FileNotFoundException("did not find " + filePath);
			}
		}
		
		private static HttpContent Read(HttpCache cache, string request, Dictionary<string, HttpContent> baseContent)
		{
			request = request.TrimWhiteSpace().Replace(@"/", @"\").ToLower();
			
			if (cache.Contains(request))
			{
				return new HttpContent(cache.Get(request));
			}
			else
			{
				throw new FileNotFoundException("did not find " + request);
			}
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

		protected byte[] content;
		protected Dictionary<string, HttpContent> baseContent;
		
		public DateTime LastModified { get; protected set; }
		public string ContentType { get; protected set; }
		public string ETag { get; private set; }
		public bool ParsingRequired { get; protected set; }
		
		protected HttpContent() : this(Encoding.ASCII.GetBytes(""), "text/plain") { }
		public HttpContent(string content) : this(Encoding.ASCII.GetBytes(content), "text/plain") { }
		public HttpContent(Dictionary<string, object> json) : this(Encoding.ASCII.GetBytes(json.ToJson()), "application/json") { }
		public HttpContent(List<Dictionary<string, object>> json) : this(Encoding.ASCII.GetBytes(json.ToJson()), "application/json") { }


		public HttpContent(byte[] content, string contentType)
		{
			this.baseContent = new Dictionary<string, HttpContent>();
			this.ContentType = contentType;
			this.content = content;
			this.ParsingRequired = false;
			this.LastModified = DateTime.UtcNow;
			this.ETag = "";
		}
		
		public HttpContent(HttpCachedFile file) : this(file, new Dictionary<string, HttpContent>()) { }
		public HttpContent(HttpCachedFile file, Dictionary<string, HttpContent> baseContent)
		{
			this.baseContent = baseContent;
			this.content = file.Data;
			this.ParsingRequired = file.ParsingRequired;
			this.LastModified = file.LastModified;
			this.ContentType = file.ContentType;
			this.ETag = file.ETag;
		}
		
		#region render
		
		public byte[] Render(HttpCache cache)
		{
			return (this.ParsingRequired) ? Render(cache, baseContent) : this.content;
		}

		private string RenderText(HttpCache cache, Dictionary<string, HttpContent> baseContent)
		{
			return StringHelper.GetString(Render(cache, baseContent));
		}


		public const string INCLUDE_RENDER_SYNTAX = @"\<%@ Include:\s*([^@]+)\s*\%\>";
		public const string CONTENT_RENDER_SYNTAX = @"\<%@ Content:\s*([^@]+)\s*\%\>";

		private byte[] Render(HttpCache cache, Dictionary<string, HttpContent> baseContent)
		{
			if (this.ContentType.Contains("text") || this.ContentType.Contains("xml"))
			{
				string text = Encoding.ASCII.GetString(this.content);
				
				// include linked externals
				MatchCollection matches = Regex.Matches(text, INCLUDE_RENDER_SYNTAX);
				if (matches.Count > 0)
				{
					foreach (Match match in matches)
					{
						string key = match.Groups[1].Value.TrimWhiteSpace();
						string replacmentContent = HttpContent.Read(cache, key, baseContent).RenderText(cache, baseContent);
						text = Regex.Replace(text, match.Groups[0].Value, replacmentContent);
					}
				}

				// include linked externals
				matches = Regex.Matches(text, CONTENT_RENDER_SYNTAX);
				if (matches.Count > 0)
				{
					foreach (Match match in matches)
					{
						string key = match.Groups[1].Value.TrimWhiteSpace();
						if (baseContent.ContainsKey(key))
						{
							HttpContent replacmentContent = baseContent[key];
							text = Regex.Replace(text, match.Groups[0].Value, replacmentContent.RenderText(cache, baseContent));
						}
					}
				}
				
				return Encoding.ASCII.GetBytes(text);
			}
			
			return this.content;
		}

		
		#endregion
		
	}
}
