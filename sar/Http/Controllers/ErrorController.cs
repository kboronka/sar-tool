/* Copyright (C) 2016 Kevin Boronka
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
using sar.Tools;

namespace sar.Http
{
	[SarController]
	public static class ErrorController
	{
		static Dictionary<string, HttpContent> lastException;
		
		public static HttpContent Display(HttpRequest request, Exception ex, HttpStatusCode status)
		{
			Exception inner = ExceptionHelper.GetInner(ex);
			
			var baseContent = new Dictionary<string, HttpContent>() {};
			baseContent.Add("Title", new HttpContent(status.ToString()));
			baseContent.Add("ResponseCode", new HttpContent(((int)status).ToString()));
			baseContent.Add("ExceptionType", new HttpContent(inner.GetType().ToString()));
			baseContent.Add("RequestURL", new HttpContent(request.FullUrl));
			baseContent.Add("ExceptionMessage", new HttpContent(inner.Message));
			
			string stackTrace = "";
			
			if (inner != ex)
			{
				stackTrace += "<span><strong>Outer:</strong><br>";
				stackTrace += "<cite>" + ExceptionHelper.GetStackTrace(ex) + "</cite></span>";
				stackTrace += Environment.NewLine;
				stackTrace += "<span><strong>Inner:</strong><br>";
				stackTrace += "<cite>" + ExceptionHelper.GetStackTrace(inner) + "</cite></span>";
			}
			else
			{
				stackTrace += "<cite>" + ExceptionHelper.GetStackTrace(ex) + "</cite>";
			}
			
			stackTrace = stackTrace.Replace("\t", "");
			
			baseContent.Add("ExceptionStackTrace", new HttpContent(stackTrace.ToHTML()));
			lastException = baseContent;
			
			return HttpContent.Read(request.Server, "sar.Http.Views.Error.Display.html", baseContent);
		}
		
		[PrimaryView]
		public static HttpContent ShowLast(HttpRequest request)
		{
			if (lastException == null) throw new ApplicationException("This is the first exception");
			return HttpContent.Read(request.Server, "sar.Http.Views.Error.Display.html", lastException);
		}
	}
}
