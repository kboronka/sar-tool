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
using System.IO;
using System.Net;

namespace sar.Tools
{
	public static class WebHelper
	{
		private static WebClient client;
		
		private static WebClient Client
		{
			get
			{
				if (client == null) client = new WebClient();
				return client;
			}
		}
		
		public static void Download(string url, string localfile)
		{
			Client.DownloadFile(url, localfile);
		}
		
		public static string ReadURL(string url)
		{
			return Client.DownloadString(url);
		}
		
		public static string ReadJSON(string url)
		{
			var request = WebRequest.Create(url);
			request.Credentials = CredentialCache.DefaultCredentials;
			
			var response = request.GetResponse();
			var responceStream = response.GetResponseStream ();
			var reader = new StreamReader(responceStream);
			
			string json = reader.ReadToEnd();
			
			reader.Close();
			response.Close();
			
			return json;
		}
	}
}
