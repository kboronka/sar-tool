
using System;

namespace sar_testing.Http
{
	public static class TestController
	{
		public static HttpContent Show(HttpRequest request)
		{
			return HttpContent.Read("sar_testing.Http.Views.Test.test.html", lastException);
		}
	}
}
