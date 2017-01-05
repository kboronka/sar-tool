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
using System.Reflection;


namespace sar.Tools
{
	public class EmbeddedResource
	{
		#region static

		private static Dictionary<string, EmbeddedResource> embeddedResources = new Dictionary<string, EmbeddedResource>();
		
		private static EmbeddedResource Find(string resource)
		{
			if (embeddedResources.ContainsKey(resource))
			{
				return embeddedResources[resource];
			}
			else
			{
				var embeddedResource = new EmbeddedResource(resource);
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
		
		private static Dictionary<string, Assembly> embeddedFiles;
		public static Dictionary<string, Assembly> EmbeddedFiles
		{
			get
			{
				if (embeddedFiles == null)
				{
					embeddedFiles = new Dictionary<string, Assembly>();
					foreach (Assembly assembly in AssemblyInfo.Assemblies)
					{
						foreach (string resource in assembly.GetManifestResourceNames())
						{
							embeddedFiles.Add(resource, assembly);
						}
					}
				}
				
				return embeddedFiles;
			}
		}
		
		public static List<string> GetAllResources()
		{
			var keys = new List<string>();
			foreach(string key in EmbeddedFiles.Keys)
			{
				keys.Add(key);
			}
			
			return keys;
		}
		
		#endregion
		
		private byte[] buffer = new byte[0] {};

		private EmbeddedResource(string resource)
		{
			resource = resource.Replace(@"/", @".");
			if (!EmbeddedFiles.ContainsKey(resource)) throw new FileNotFoundException("resource: " + resource + " not found");
			
			Assembly assembly = EmbeddedFiles[resource];
			Stream stream = assembly.GetManifestResourceStream(resource);
			this.buffer = StreamHelper.ReadToEnd(stream);
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
