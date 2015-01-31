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

		public static string[] GetAllResources()
		{
			Assembly sarAssembly = Assembly.GetExecutingAssembly();
			Assembly callerAssembly = Assembly.GetEntryAssembly();
			
			string[] sarFiles = sarAssembly.GetManifestResourceNames();
			string[] callerFiles = callerAssembly.GetManifestResourceNames();
			
			string[] files = new string[sarFiles.Length + callerFiles.Length];
			Array.Copy(sarFiles, 0, files, 0, sarFiles.Length);
			Array.Copy(callerFiles, 0, files, sarFiles.Length, callerFiles.Length);
			
			return files;
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
