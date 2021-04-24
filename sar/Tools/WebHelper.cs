/* Copyright (C) 2021 Kevin Boronka
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

using sar.Json;
using sar.Net;
using System.IO;
using System.Net;

namespace sar.Tools
{
	public static class WebHelper
	{
		private static WebClientEx client;

		private static WebClientEx Client
		{
			get
			{
				if (client == null)
					client = new WebClientEx(WebClientEx.DEFAULT_HTTP_TIMEOUT);
				return client;
			}
		}

		public static void Download(string url, string localfile, int timeoutMs = WebClientEx.DEFAULT_HTTP_TIMEOUT)
		{
			Client.Timeout = timeoutMs;
			Client.DownloadFile(url, localfile);
		}

		public static string ReadUrl(string url, int timeoutMs = WebClientEx.DEFAULT_HTTP_TIMEOUT)
		{
			Client.Timeout = timeoutMs;
			return Client.DownloadString(url);
		}

		public static string ReadJson(string url, int timeoutMs = WebClientEx.DEFAULT_HTTP_TIMEOUT)
		{
			var request = WebRequest.Create(url);
			request.Timeout = timeoutMs;
			request.Credentials = CredentialCache.DefaultCredentials;
			var response = request.GetResponse();
			var responceStream = response.GetResponseStream();

			try
			{
				var ms = new MemoryStream();
				responceStream.CopyTo(ms);
				byte[] data = ms.ToArray();

				return JsonHelper.BytesToJson(data);
			}
			catch
			{
				return "";
			}
			finally
			{
				responceStream.Close();
				response.Close();
			}
		}
	}
}
