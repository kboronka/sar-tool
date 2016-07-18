
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using sar.Tools;
using sar.Http;
using Base = sar.Base;

namespace sar.CNC
{
	public class App : sar.Base.Configuration
	{
		#region singleton
		
		private static App configuration;
		static HttpServer server;
		public static GrblPort Port { get; private set; }
		
		public static void Load()
		{
			// defaults
			CommPort = "COM3";
			CamFiles = new List<string>();

			// load settings from XML
			App.configuration = new App();
			
			Port = new GrblPort(App.CommPort);
			//Port.ResponceRecived += new GrblPort.ResponceRecivedHandler(Program.LogResponce);
			
			// launch HTTP server
			var root = ApplicationInfo.CurrentDirectory;
			
			#if DEBUG
			root += @"\..\..\Http\Views";
			#else
			root += @"\views";
			#endif
			server = new HttpServer(root);
			server.FavIcon = "favicon.ico";
			
			// manage websocket list 
			HttpWebSocket.ClientConnected += new HttpWebSocket.ConnectedClientHandler(AddClient);
			HttpWebSocket.ClientDisconnected += new HttpWebSocket.ClientDisconnectedHandler(DropClient);

			
			configuration.Save();
		}
		
		public static void Update()
		{
			App.configuration.Save();
		}
		
		#endregion
		
		#region global configuration

		public static string CommPort { get; set; }
		public static List<string> CamFiles { get; private set; }
		
		#endregion
		
		#region load/save
		
		protected override void Deserialize(XML.Reader reader)
		{
			try
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						switch (reader.Name)
						{
							case "CommPort":
								CommPort = reader.GetValueString();
								break;
								
							case "CamFile":
								CamFiles.Add(reader.GetValueString());
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Program.Log(ex);
			}
		}
		
		protected override void Serialize(XML.Writer writer)
		{
			try
			{
				writer.WriteElement("CommPort", CommPort);
				
				var root = ApplicationInfo.CurrentDirectory;
				foreach (var file in CamFiles)
				{
					if (File.Exists(root + file))
					{
						writer.WriteElement("CamFile", file);
					}
				}
			}
			catch (Exception ex)
			{
				Program.Log(ex);
			}
		}
		
		#endregion
		
		#region websocket clients
		
		private static List<HttpWebSocket> clients = new List<HttpWebSocket>();
	
		private static void AddClient(HttpWebSocket client)
		{
			Program.Log("Client " + client.ID.ToString() + " Connected");
			clients.Add(client);
		}
		
		private static void DropClient(HttpWebSocket client)
		{
			Program.Log("Client " + client.ID.ToString() + " Disconnected");
			clients.Remove(client);
		}
		
		public static void SendToWebSocketClients(string raw)
		{
			foreach (var client in clients)
			{
				client.SendString(raw);
			}
		}
		
		#endregion		
	}
}
