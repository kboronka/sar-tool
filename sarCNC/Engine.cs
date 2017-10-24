/*
 * Created by SharpDevelop.
 * User: Kevin
 * Date: 1/24/2017
 * Time: 8:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO.Ports;

using sar;
using sar.Tools;
using sar.Http;

using sar.CNC.Http;

namespace sar.CNC
{
	public static class Engine
	{
		public static HttpServer HttpServer { get; private set; }
		public static Grbl Port { get; private set; }
		public static Parameters Parameters { get; private set; }
		public static bool Stopped { get; private set; }
		
		public static void Start()
		{
			Logger.Log("Starting Engine");
			Engine.Stopped = false;
			
			// start client handler before starting http server
			GrblWebSocket.Start();
			
			// launch http server
			var root = ApplicationInfo.CurrentDirectory;
			
			#if DEBUG
			root += @"\..\..\Http\Views";
			#else
			root += @"\views";
			#endif
			Engine.HttpServer = new HttpServer(root);
			Engine.HttpServer.FavIcon = "favicon.ico";
			Logger.Log("HTTP server running on port " + Engine.HttpServer.Port.ToString());

			// load settings from XML
			Engine.Parameters = new Parameters();
			
			// TODO: find the correct comm port automatically
			Logger.Log("Starting GRBL");
			var serialPorts = SerialPort.GetPortNames();
			Engine.Parameters.CommPort = serialPorts[0];
			
			// create grbl object
			Port = new Grbl(Engine.Parameters.CommPort);

			//Port.ResponceRecived += new GrblPort.ResponceRecivedHandler(Logger.LogResponce);
		}
		
		public static void Stop()
		{
			HttpServer.Stop();
			GrblWebSocket.Stop();
			Engine.Stopped = true;
		}
	}
}
