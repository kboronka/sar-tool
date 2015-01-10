using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using sar.Tools;

namespace sar.Http
{
	public class HttpEmbeddedResource
	{
		#region static

		private static Dictionary<string, HttpEmbeddedResource> embeddedResources = new Dictionary<string, HttpEmbeddedResource>();
		
		private static HttpEmbeddedResource Find(string resource)
		{
			if (embeddedResources.ContainsKey(resource))
			{
				return embeddedResources[resource];
			}
			else
			{
				HttpEmbeddedResource embeddedResource = new HttpEmbeddedResource(resource);
				embeddedResources.Add(resource, embeddedResource);

				return embeddedResource;
			}
		}
		
		public static bool Contains(string resource)
		{
			try
			{
				return Find(resource) != null;
			}
			catch
			{
				
			}
			
			return false;
		}
		
		public static byte[] Get(string resource)
		{
			return Find(resource).Bytes;
		}

		#endregion
		
		private byte[] buffer = new byte[0] {};

		private HttpEmbeddedResource(string resource)
		{
			resource = resource.Replace(@"/", @".");
			Assembly sarAssembly = Assembly.GetExecutingAssembly();
			string x = sarAssembly.FullName;
			Stream stream = sarAssembly.GetManifestResourceStream(resource);
			if (stream != null)
			{
				this.buffer = StreamHelper.ReadToEnd(stream);
			}
			else
			{
				Assembly callerAssembly = Assembly.GetEntryAssembly();
				x = callerAssembly.FullName;
				stream = callerAssembly.GetManifestResourceStream(resource);
				this.buffer = StreamHelper.ReadToEnd(stream);
			}
		}
		
		public byte[] Bytes
		{
			get { return this.buffer; }
		}
		
		public override string ToString()
		{
			if (buffer.Length > 0)
			{
				return StringHelper.GetString(buffer);
			}
			
			return "";
		}
	}
}
