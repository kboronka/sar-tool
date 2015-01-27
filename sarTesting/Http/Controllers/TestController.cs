
using System;
using System.Collections.Generic;

using sar.Http;
using sar.Tools;

namespace sar_testing.Http
{
	public static class TestController
	{
		public static HttpContent Show(HttpRequest request)
		{
			return HttpContent.View("Test", "Test.html");
			//return HttpContent.Read("sar_testing.Http.Views.Test.Test.html");
		}
	}
}
