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

using sar.Http;
using sar.Tools;

namespace sar_testing.Http
{
	[SarController]
	[PrimaryController]
	public static class TestController
	{
		[PrimaryView]
		public static HttpContent Test(HttpRequest request)
		{
			return HttpController.RequestAction("Test", "Show", request);
		}
		
		public static HttpContent Index(HttpRequest request)
		{
			return HttpContent.Read(request, "index.html");
		}

		public static HttpContent Show(HttpRequest request)
		{
			return HttpContent.Read(request, "sar_testing.Http.Views.Test.Test.html");
		}
		
		public static HttpContent UpdateTable(HttpRequest request)
		{
			var json = new Dictionary<string, object>();
			
			string[] row1 = { "blabla", HttpHelper.LabelSuccess("passed"), Guid.NewGuid().ToString(), "booo" };
			string[] row2 = { "blabla", HttpHelper.LabelDanger("failed"), Guid.NewGuid().ToString(), "booo" };
			string[] row3 = { "blabla", HttpHelper.LabelDefault("whatever"), Guid.NewGuid().ToString(), "booo" };
			
			string[][] table = { row1, row2, row3 };
			
			json.Add("testTabelData", table.ToHTML());
			return new HttpContent(json);
		}
		
		public static HttpContent Data(HttpRequest request)
		{
			var json = new List<Dictionary<string, object>>();
			int alternate = 1;
			
			var rand = new Random();
			rand.Next(100,220);
			
			int offset = rand.Next(100,220);
			
			for (int columnNumber=1; columnNumber<=10; columnNumber++)
			{
				if (alternate > 2) alternate = 1;
				var labelJSON = new Dictionary<string, object>();
				labelJSON.Add("col1", columnNumber + offset);
				labelJSON.Add("col2", alternate++);
				json.Add(labelJSON);
			}
			
			return new HttpContent(json);
		}
		
		public static HttpContent GetIntArrayJSON(HttpRequest request)
		{
			var json = new Dictionary<string, object>();
			json.Add("data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 });
			
			return new HttpContent(json);
		}
	}
}
