using System;
using System.Collections.Generic;
using System.Text;

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
			try
			{
				Dictionary<string, string> result = new Dictionary<string, string>();
				result.Add("request", Encoding.ASCII.GetString(request.Data));
				result.Add("other", Guid.NewGuid().ToString());
				
				if (result["request"] == "error") throw new ApplicationException("error test");
				
				return new HttpContent(result.ToJSON());
			}
			catch (Exception ex)
			{
				return new HttpErrorContent(ex);
			}
		}
	}
}
