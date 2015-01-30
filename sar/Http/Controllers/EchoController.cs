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
