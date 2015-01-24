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
