
using System;
using System.Collections.Generic;
using sar.Tools;

namespace sar.Http
{
	public static class ErrorController
	{
		public static HttpContent Display(HttpRequest request, Exception ex, HttpStatusCode status)
		{
			Exception inner = ExceptionHandler.GetInnerException(ex); 
			
			Dictionary<string, HttpContent> baseContent = new Dictionary<string, HttpContent>() {};
			baseContent.Add("Title", new HttpContent(status.ToString()));
			baseContent.Add("ResponceCode", new HttpContent(((int)status).ToString()));
			baseContent.Add("ExceptionType", new HttpContent(inner.GetType().ToString()));
			baseContent.Add("RequestURL", new HttpContent(request.Url));
			baseContent.Add("ExceptionMessage", new HttpContent(inner.Message));
			baseContent.Add("ExceptionStackTrace", new HttpContent(inner.GetStackTrace().ToHTML()));

			return HttpContent.Read("sar.Http.error.views.display.html", baseContent);			
		}
	}
}
