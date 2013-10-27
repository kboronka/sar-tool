using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

using sar.Tools;

namespace sar.Socket
{
	public class SocketMessage
	{
		private string command;
		private long id;
		private long length;
		private string member;
		private string data;
		private long fromID;
		private long toID;
		
		private string messasge;
		
		#region properties
		
		public string Command
		{
			get { return command.ToLower(); }
		}

		public long Id
		{
			get { return id; }
		}

		public string Member
		{
			get { return member; }
		}

		public string Data
		{
			get { return data; }
		}
		
		public long FromID
		{
			get { return fromID; }
		}

		public long ToID
		{
			get { return toID; }
		}
		
		#endregion
		
		public SocketMessage(SocketClient client, string command, long messageID, string member, string data, long toID)
		{
			this.command = command;
			this.id = messageID;
			this.member = member;
			this.data = data;
			this.fromID = client.ID;
			this.toID = toID;
			this.length = data.Length;
			
			using (StringWriter sw = new StringWriter())
			{
				using (XmlWriter writer = XmlWriter.Create(sw, StringHelper.WriterSettings))
				{
					writer.WriteStartElement("SocketMessage");
					writer.WriteAttributeString("id", this.id.ToString());
					writer.WriteAttributeString("command", this.command);
					writer.WriteAttributeString("member", this.member);
					writer.WriteAttributeString("len", this.length.ToString());
					writer.WriteAttributeString("from", this.fromID.ToString());
					writer.WriteAttributeString("to", this.toID.ToString());
					writer.WriteValue(this.data);
					writer.WriteEndElement();		// Message
				}
				
				this.messasge = sw.ToString();
			}
		}
		
		public SocketMessage(string rawString)
		{
			try
			{
				if (!String.IsNullOrEmpty(rawString))
				{
					using (StringReader sr = new StringReader(rawString))
					{
						using (XmlReader reader = XmlReader.Create(sr, StringHelper.ReaderSettings))
						{
							while (reader.Read())
							{
								if (reader.NodeType == XmlNodeType.Element)
								{
									switch (reader.Name)
									{
										case "SocketMessage":
											this.id = long.Parse(reader.GetAttribute("id"));
											this.command = reader.GetAttribute("command");
											this.member = reader.GetAttribute("member");
											this.fromID = long.Parse(reader.GetAttribute("from"));
											this.toID = long.Parse(reader.GetAttribute("to"));
											this.length = long.Parse(reader.GetAttribute("len"));
											if (this.length > 0) reader.Read();
											this.data = reader.Value;
											break;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{

			}
		}
		
		public SocketMessage()
		{
		}
		
		public override string ToString()
		{
			return this.messasge;
		}
	}
}
