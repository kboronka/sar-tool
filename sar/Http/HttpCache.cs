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
using System.Linq;

using sar.Tools;

namespace sar.Http
{
	public class HttpCache
	{
		private Dictionary<string, HttpCachedFile> cache;
		private HttpServer server;
		
		public HttpCache(HttpServer server)
		{
			this.cache = new Dictionary<string, HttpCachedFile>();
			this.server = server;
			
			// read all embedded resources
			foreach (var file in EmbeddedResource.GetAllResources())
			{
				var request = file.ToLower();
				cache.Add(request, new HttpCachedEmbeddedFile(file));
			}
			
			// read all files on root folder
			foreach (var file in IO.GetAllFiles(server.Root))
			{
				var request = file.Substring(server.Root.Length + 1).ToLower();
				System.Diagnostics.Debug.WriteLine(request);
				cache.Add(request, new HttpCachedFile(file));
			}
		}
		
		public bool Contains(HttpRequest request)
		{
			var requestPath = request.Path.TrimWhiteSpace();
			string filePath = server.Root + @"\" + requestPath.Replace(@"/", @"\");
			
			return Contains(filePath);
		}
		
		public bool Contains(string filePath)
		{
			return this.cache.ContainsKey(filePath);
		}
		
		public HttpCachedFile Get(HttpRequest request)
		{
			var requestPath = request.Path.TrimWhiteSpace();
			
			string filePath = server.Root + @"\" + requestPath.Replace(@"/", @"\");
			return Get(filePath);
		}

		public HttpCachedFile Get(string filePath)
		{
			
			if (this.cache.ContainsKey(filePath))
			{
				return this.cache[filePath];
			}
			
			if (File.Exists(filePath))
			{
				var newFile = new HttpCachedFile(filePath);
				this.cache.Add(filePath, newFile);
				
				return newFile;
			}
			
			throw new FileNotFoundException(filePath);
		}
	}
}
