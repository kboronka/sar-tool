
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


using sar.Tools;

namespace sar.Http
{
	public class HttpContent
	{
		#region static

		public static HttpContent Read(HttpServer server, string request)
		{
			string filePath = server.Root + request.Replace(@"/", @"\");
			
			if (File.Exists(filePath))
			{
				return HttpContent.Read(filePath);
			}
			else if (HttpEmbeddedResource.Contains(request.Substring(1)))
			{
				return HttpContent.Read(request.Substring(1));
			}
			else
			{
				throw new FileNotFoundException("did not find " + filePath);
			}
		}
		
		public static HttpContent Read(string filePath)
		{
			return Read(filePath, new Dictionary<string, HttpContent>() {});
		}
		
		public static HttpContent Read(string filePath, Dictionary<string, HttpContent> baseContent)
		{
			filePath = StringHelper.TrimWhiteSpace(filePath);
			
			if (File.Exists(filePath))
			{
				return ReadFile(filePath, baseContent);
			}
			else if (HttpEmbeddedResource.Contains(filePath))
			{
				return ReadEmbeddedFile(filePath, baseContent);
			}
			else
			{
				throw new FileNotFoundException("did not find " + filePath);
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
			if (HttpEmbeddedResource.Contains(resource))
			{
				string extension = IO.GetFileExtension(resource).ToLower();
				string contentType = HttpHelper.GetMimeType(extension);
				byte[] content = HttpEmbeddedResource.Get(resource);
				
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
		
		byte[] content;
		string contentType;
		
		private string RenderText(Dictionary<string, HttpContent> baseContent)
		{
			return StringHelper.GetString(Render(baseContent));
		}

		public byte[] Render()
		{
			return Render(new Dictionary<string, HttpContent>() {});
		}
		
		public byte[] Render(Dictionary<string, HttpContent> baseContent)
		{
			if (this.contentType.Contains("text") || this.contentType.Contains("xml"))
			{
				string text = Encoding.ASCII.GetString(this.content);
				
				// include linked externals
				MatchCollection matches = Regex.Matches(text, @"<%@ Include:\s*(.*)\s*%>");
				if (matches.Count > 0)
				{
					foreach (Match match in matches)
					{
						string replacmentContent = HttpContent.Read(match.Groups[1].Value, baseContent).RenderText(baseContent);
						text = Regex.Replace(text, match.Groups[0].Value, replacmentContent);
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
		
		private HttpContent()
		{
			this.contentType = "text/plain";
		}
		
		public HttpContent(string content)
		{
			this.contentType = "text/plain";
			this.content = Encoding.ASCII.GetBytes(content);
		}

		private HttpContent(byte[] content, string contentType)
		{
			this.contentType = contentType;
			this.content = content;
		}

		
		public HttpContent(byte[] content, string contentType, Dictionary<string, HttpContent> baseContent)
		{
			this.contentType = contentType;
			this.content = content;
		}
		
		public int Length
		{
			get { return this.content.Length; }
		}
	}
}
