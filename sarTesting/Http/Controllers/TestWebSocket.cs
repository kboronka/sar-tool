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

using System;
using System.Collections.Generic;
using System.Text;

using sar;
using sar.Http;
using sar.Tools;

namespace sar_testing.Http
{
	[SarWebSocketController]
	public class TestWebSocket : sar.Http.HttpWebSocket
	{
		#region static
		
		public static List<TestWebSocket> clients = new List<TestWebSocket>();
		private static TestWebSocket Singleton { get; set; }

		public static void JogForward()
		{
			TestMessage();
			foreach (var c in clients)
			{
				if (c.Open)
				{
					var text = Encoding.ASCII.GetString(HttpWebSocketFrame.EncodeFrame("jog +").EncodedFrame);
					c.SendString(text);
				}
			}
		}
		
		#endregion 

		public TestWebSocket(HttpRequest request) : base(request)
		{
			Singleton = this;
		}
		
		override public void NewData(string json)
		{
			
		}
		
		public static void TestMessage()
		{
			var mdn = System.Text.Encoding.ASCII.GetBytes("MDN");
			var frame = new byte[]{ 129, 131, 61, 84, 35, 6, 112, 16, 109 };
			var msg = HttpWebSocketFrame.DecodeFrame(frame);
			
			Logger.Log(StringHelper.ArrayToString("frame", frame));
			Logger.Log(StringHelper.ArrayToString("mdn", mdn));
			Logger.Log(StringHelper.ArrayToString("payload", msg.Payload));
		}
	}
}
