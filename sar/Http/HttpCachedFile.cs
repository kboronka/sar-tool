using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using sar.Tools;

namespace sar.Http
{
	public class HttpCachedFile
	{
		public DateTime LastModified { get; protected set; }
		public string ContentType { get; private set; }
		public string ETag { get; private set; }
		public byte[] Data { get; private set; }
		public bool RenderRequired { get; protected set; }
		
		protected bool embedded;
		protected string path;
		
		private FileSystemWatcher watcher;
		
		#region constructors
		
		protected HttpCachedFile(string path, byte[] data)
		{
			this.path = path;
			this.embedded = true;

			string extension = IO.GetFileExtension(path).ToLower();
			this.ContentType = HttpHelper.GetMimeType(extension);
			this.Data = data;
			this.ETag = GetETag(this.Data);
			this.RenderRequired = false;
			
			if (this.ContentType.Contains("text") || this.ContentType.Contains("xml"))
			{
				string text = Encoding.ASCII.GetString(this.Data);
				MatchCollection matches = Regex.Matches(text, HttpContent.INCLUDE_RENDER_SYNTAX);
				if (matches.Count > 0) this.RenderRequired = true;
				
				// include linked externals
				matches = Regex.Matches(text, HttpContent.CONTENT_RENDER_SYNTAX);
				if (matches.Count > 0) this.RenderRequired = true;
			}
		}
		
		public HttpCachedFile(string path) : this(path, File.ReadAllBytes(path))
		{
			this.LastModified = File.GetLastWriteTimeUtc(path);
			
			watcher = new FileSystemWatcher();
			watcher.Path = IO.GetFileDirectory(path);
			watcher.Filter = IO.GetFilename(path);
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			watcher.Deleted += new FileSystemEventHandler(OnDelete);
			watcher.Renamed += new RenamedEventHandler(OnRenamed);
			watcher.EnableRaisingEvents = true;
		}
		
		#endregion
		
		private void OnChanged(object sender, FileSystemEventArgs e)
		{
			this.Data = File.ReadAllBytes(path);
			this.ETag = GetETag(this.Data);
			this.LastModified = File.GetLastWriteTimeUtc(path);
		}
		
		private void OnDelete(object sender, FileSystemEventArgs e)
		{
			
		}

		private void OnRenamed(object sender, RenamedEventArgs e)
		{
			
		}
		
		private static string GetETag(byte[] data)
		{
			var hash = new MD5CryptoServiceProvider().ComputeHash(data);
			
			var hex = "";
			foreach (var b in hash)
			{
				hex += b.ToString("{0:x2}");
			}
			
			return hex.ToString();
		}
	}
}
