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
using System.Text;

using sar.Tools;

namespace sar.Http
{
	// used for testing ajax calls
	[SarController]
	public static class DebugController
	{
		public static HttpContent Info(HttpRequest request)
		{
			string data = "";
			if (request.Data != null)
			{
				data = StringHelper.GetString(request.Data);
			}
			
			var baseContent = new Dictionary<string, HttpContent>() {};
			baseContent.Add("Method", new HttpContent(request.Method.ToString()));
			baseContent.Add("URL", new HttpContent(request.FullUrl));
			baseContent.Add("Version", new HttpContent(request.ProtocolVersion));
			baseContent.Add("Data", new HttpContent(data));

			return HttpContent.Read(request.Server, "sar.Http.Views.Debug.Info.html", baseContent);
		}
		
		public static HttpContent Header(HttpRequest request)
		{
			var baseContent = new Dictionary<string, HttpContent>() {};
			baseContent.Add("Header", new HttpContent(request.ToString()));
			return HttpContent.Read(request.Server, "sar.Http.Views.Debug.Header.html", baseContent);
		}		
	}
}
