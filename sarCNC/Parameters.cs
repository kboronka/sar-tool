
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Xml;

using sar;
using sar.Tools;
using sar.Http;

namespace sar.CNC
{
	public class Parameters : sar.Base.Configuration
	{
		#region global configuration

		public string CommPort { get; set; }
		public List<string> CamFiles { get; set; }
		
		#endregion
		
		#region load/save
		
		protected override void InitDefaults()
		{
			var serialPorts = SerialPort.GetPortNames();
			
			this.CommPort = serialPorts[0];
			this.CamFiles = new List<string>();
		}
		
		protected override void Deserialize(XML.Reader reader)
		{
			this.CamFiles = new List<string>();
			
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
				Logger.Log(ex);
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
				Logger.Log(ex);
			}
		}
		
		#endregion

	}
}
