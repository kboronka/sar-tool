/*
 * Created by SharpDevelop.
 * User: kboronka
 * Date: 7/4/2016
 * Time: 4:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using sar.Http;
using sar.Tools;

namespace sar_testing.Http
{
	[SarWebSocketController]
	public class TestWebSocket : sar.Http.HttpWebSocket
	{
		#region static
		
		private static TestWebSocket Singleton { get; set; }

		public static void JogForward()
		{
			TestMessage();
			if (Singleton != null) Singleton.Send(HttpWebSocketFrame.EncodeFrame("jog +").EncodedFrame);
		}
		
		#endregion 

		public TestWebSocket(HttpRequest request) : base(request)
		{
			Singleton = this;
		}
		
		override public void NewData(byte[] data)
		{
			
		}
		
		public static void TestMessage()
		{
			var mdn = System.Text.Encoding.ASCII.GetBytes("MDN");
			var frame = new byte[]{ 129, 131, 61, 84, 35, 6, 112, 16, 109 };
			var msg = HttpWebSocketFrame.DecodeFrame(frame);
			
			sar.Testing.Program.Log(StringHelper.ArrayToString("frame", frame));
			sar.Testing.Program.Log(StringHelper.ArrayToString("mdn", mdn));
			sar.Testing.Program.Log(StringHelper.ArrayToString("payload", msg.Payload));
		}
	}
}
