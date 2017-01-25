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

namespace sar.CNC.Http
{
	[SarWebSocketController]
	public class TestWebSocket : sar.Http.HttpWebSocket
	{
		#region static
		
		private static TestWebSocket Singleton { get; set; }
		
		#endregion 

		public TestWebSocket(HttpRequest request) : base(request)
		{
			Singleton = this;
		}
		
		override public void NewData(byte[] data)
		{
			var rxBuffer = System.Text.Encoding.ASCII.GetString(data);
			var commands = rxBuffer.Split('\n');
			
			foreach (var command in commands)
			{
				Engine.Port.SendCommand(command, "");
			}
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
