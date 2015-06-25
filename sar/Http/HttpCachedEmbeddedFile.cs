using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

using sar.Tools;

namespace sar.Http
{
	public class HttpCachedEmbeddedFile : HttpCachedFile
	{
		public HttpCachedEmbeddedFile(string path) : base(path, EmbeddedResource.Get(path))
		{
			this.embedded = true;
			this.LastModified = DateTime.UtcNow;
		}
	}
}
