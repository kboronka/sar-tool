using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using sar.Tools;


namespace sar.Base
{
	public abstract class Configuration
	{
		protected string path;
		
		public Configuration()
		{
			this.path = ApplicationInfo.CommonDataDirectory + sar.Tools.AssemblyInfo.Name + ".xml";
			
			if (File.Exists(this.path))
			{
				XmlReader reader = XmlReader.Create(this.path, StringHelper.ReaderSettings());
				this.Deserialize(reader);
				reader.Close();
			}	
		}
		
		public Configuration(string path)
		{
			this.path = path;
			
			if (File.Exists(this.path))
			{
				XmlReader reader = XmlReader.Create(this.path, StringHelper.ReaderSettings());
				this.Deserialize(reader);
				reader.Close();
			}
		}
		
		public void Save()
		{
			this.Save(this.path);
		}
		
		public void Save(string path)
		{
			this.path = path;
			XmlWriter writer = XmlWriter.Create(path, StringHelper.WriterSettings());
			
			writer.WriteStartDocument();
			writer.WriteStartElement(sar.Tools.AssemblyInfo.Name);
			writer.WriteAttributeString("version", sar.Tools.AssemblyInfo.Version);
			this.Serialize(writer);
			writer.WriteEndElement();
			
			writer.Close();
		}
		
		protected abstract void Deserialize(XmlReader reader);
		protected abstract void Serialize(XmlWriter writer);
	}
}
