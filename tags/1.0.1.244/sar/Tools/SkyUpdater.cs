/* Copyright (C) 2015 Kevin Boronka
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
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace sar.Tools
{
	public class SkyUpdaterFile
	{
		private string filename;
		private string file;
		private string url;
		
		public string Url
		{
			get { return url; }
			set { url = value; }
		}
		
		public string File
		{
			get { return file; }
			set { file = value; }
		}
		
		public string Filename
		{
			get { return filename; }
		}
		
		// TODO: use xml helper
		public SkyUpdaterFile(XmlReader reader)
		{
			if (reader.NodeType != XmlNodeType.Element || reader.Name != "File")
			{
				throw new ApplicationException("File element expected");
			}
			
			while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "File"))
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "URL":
							reader.Read();
							this.url = reader.Value;
							break;
						case "Path":
							reader.Read();
							this.file = reader.Value;
							this.filename = IO.GetFilename(this.file);
							break;
					}
				}
			}
		}
		
		public SkyUpdaterFile(string file, string url)
		{
			this.file = file;
			this.filename = IO.GetFilename(file);
			this.url = url;
		}
		
		//TODO: use xml helper
		public void Serialize(XmlWriter writer)
		{
			writer.WriteStartElement("File");

			writer.WriteStartElement("URL");
			writer.WriteValue(this.url);
			writer.WriteEndElement();
			
			writer.WriteStartElement("Path");
			writer.WriteValue(this.file);
			writer.WriteEndElement();
			
			writer.WriteEndElement();	//File
		}
	}
	
	public class SkyUpdater
	{
		private string version;
		private string applicationName;
		private List<SkyUpdaterFile> files;
		
		public string Version
		{
			get { return version; }
		}

		public List<SkyUpdaterFile> Files
		{
			get { return files; }
		}
		
		private SkyUpdater()
		{
			
		}
		
		public static SkyUpdater Make(AssemblyName applicationName)
		{
			SkyUpdater updater = new SkyUpdater();
			updater.applicationName = applicationName.Name;
			updater.version = applicationName.Version.ToString();
			updater.files = new List<SkyUpdaterFile>();
			
			return updater;
		}
		
		public static SkyUpdater Open(string file)
		{
			SkyUpdater updater = new SkyUpdater();
			XmlReader reader = XmlReader.Create(file, ReaderSettings());
			updater.Deserialize(reader);
			reader.Close();
			
			return updater;
		}
		
		public static SkyUpdater OpenURL(string url)
		{
			string tempXML = IO.Temp + "SkyUpdate." + Guid.NewGuid().ToString() + ".xml";
			WebHelper.Download(url, tempXML);
			return Open(tempXML);
		}
		
		public void Save(string path)
		{
			XmlWriter writer = XmlWriter.Create(path, WriterSettings());
			this.Serialize(writer);
			writer.Close();
		}

		public void AddFile(string file, string url)
		{
			this.files.Add(new SkyUpdaterFile(file, url));
		}
		
		private static XmlReaderSettings ReaderSettings()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.CloseInput = true;
			settings.IgnoreComments = true;
			settings.IgnoreProcessingInstructions = true;
			settings.IgnoreWhitespace = true;
			return settings;
		}

		private static XmlWriterSettings WriterSettings()
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.CloseOutput = true;
			settings.Encoding = Encoding.UTF8;
			settings.Indent = true;
			settings.IndentChars = "\t";
			return settings;
		}
		
		private void Deserialize(XmlReader reader)
		{
			this.files = new List<SkyUpdaterFile>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "Version":
							reader.Read();
							this.version = reader.Value;
							break;
						case "Application":
							reader.Read();
							this.applicationName = reader.Value;
							break;
						case "File":
							this.files.Add(new SkyUpdaterFile(reader));
							break;
					}
				}
			}
		}

		private void Serialize(XmlWriter writer)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("SkyUpdater");
			writer.WriteAttributeString("version", AssemblyInfo.Version);

			writer.WriteStartElement("Version");
			writer.WriteValue(this.version);
			writer.WriteEndElement();
			
			writer.WriteStartElement("Application");
			writer.WriteValue(this.applicationName);
			writer.WriteEndElement();
			
			writer.WriteStartElement("Files");
			foreach (SkyUpdaterFile file in this.files)
			{
				file.Serialize(writer);
			}

			writer.WriteEndElement();	// Files

			writer.WriteEndElement();	// SkyUpdater
		}
	}
}
