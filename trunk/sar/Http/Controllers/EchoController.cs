using System;
using System.Collections.Generic;
using sar.Tools;

namespace sar.Http
{
	// used for testing ajax calls
	public static class echoController
	{
		public static HttpContent html(HttpRequest request)
		{
			return new HttpContent(request.Data, "text/html");
		}
		
		public static HttpContent json(HttpRequest request)
		{
			return new HttpContent("tbd-json");
		}
		
		public static HttpContent xml(HttpRequest request)
		{
			return new HttpContent("tbd-xml");
		}
	}
}
