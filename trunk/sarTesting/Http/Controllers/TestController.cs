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
		public static HttpContent Show(HttpRequest request)
		{
			//return HttpContent.View("Test", "Test.html");
			return HttpContent.Read("sar_testing.Http.Views.Test.Test.html");
		}
		
		public static HttpContent UpdateTable(HttpRequest request)
		{
			var json = new Dictionary<string, string>();
			
			string[] row1 = { "blabla", HttpHelper.LabelSuccess("passed"), Guid.NewGuid().ToString(), "booo" };
			string[] row2 = { "blabla", HttpHelper.LabelDanger("failed"), Guid.NewGuid().ToString(), "booo" };
			string[] row3 = { "blabla", HttpHelper.LabelDefault("whatever"), Guid.NewGuid().ToString(), "booo" };
			                         	
			string[][] table = { row1, row2, row3 };
			
			json.Add("testTabelData", table.ToHTML());
			return new HttpContent(json);
		}
	}
}
