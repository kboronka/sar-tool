/* Copyright (C) 2013 Kevin Boronka
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;
using System.Xml;

namespace sar.Tools
{
	public class XML
	{
		public class Reader : IDisposable
		{
			private XmlReader reader;
			
			public XmlNodeType NodeType
			{
				get { return reader.NodeType; }
			}
			
			public string Name
			{
				get { return reader.Name; }
			}
			
			public string Value
			{
				get { return reader.Value; }
			}
			
			
			public Reader(string path)
			{
				this.reader = XmlReader.Create(path, ReaderSettings);
			}
			
			public Reader(StringReader stringReader)
			{
				this.reader = XmlReader.Create(stringReader, ReaderSettings);
			}
			
			public string GetAttributeString(string name)
			{
				try
				{
					string attributeValue = this.reader.GetAttribute(name);
					if (String.IsNullOrEmpty(attributeValue)) return "";
					
					return this.reader.GetAttribute(name);
				}
				catch
				{
					
				}
				
				return "";
			}

			public long GetAttributeLong(string name)
			{
				try
				{
					string attributeValue = GetAttributeString(name);
					return long.Parse(attributeValue);
				}
				catch
				{
					
				}
				
				return 0;
			}
			
			public bool Read()
			{
				return this.reader.Read();
			}
			
			public void Close()
			{
				this.reader.Close();
			}
			
			private static XmlReaderSettings ReaderSettings
			{
				get
				{
					XmlReaderSettings settings = new XmlReaderSettings();
					settings.CloseInput = true;
					settings.IgnoreComments = true;
					settings.IgnoreProcessingInstructions = true;
					settings.IgnoreWhitespace = true;
					return settings;
				}
			}
			
			public void Dispose()
			{
				this.Close();
				
				Dispose(true);
				// This object will be cleaned up by the Dispose method.
				// Therefore, you should call GC.SupressFinalize to
				// take this object off the finalization queue
				// and prevent finalization code for this object
				// from executing a second time.
				GC.SuppressFinalize(this);
			}
			
			protected virtual void Dispose(bool disposing)
			{
				if(disposing)
				{

				}
			}
		}

		public class Writer : IDisposable
		{
			private XmlWriter writer;
			
			public Writer(string path)
			{
				this.writer = XmlWriter.Create(path, WriterSettings);
				this.writer.WriteStartDocument();
				this.writer.WriteStartElement(sar.Tools.AssemblyInfo.Name);
				this.writer.WriteAttributeString("version", sar.Tools.AssemblyInfo.Version);
			}
			
			public Writer(StringWriter stringWriter)
			{
				this.writer = XmlWriter.Create(stringWriter, WriterSettings);
				this.writer.WriteStartDocument();
				this.WriteStartElement(sar.Tools.AssemblyInfo.Name);
				this.WriteAttributeString("version", sar.Tools.AssemblyInfo.Version);
			}
			
			public void WriteAttributeString(string name, string value)
			{
				if (!String.IsNullOrEmpty(value)) this.writer.WriteAttributeString(name, value);
			}
			
			public void WriteValue(string value)
			{
				writer.WriteValue(value);
			}
			
			public void WriteStartElement(string name)
			{
				this.writer.WriteStartElement(name);
			}

			public void WriteEndElement()
			{
				this.writer.WriteEndElement();
			}
			
			public void Close()
			{
				this.WriteEndElement();
				this.writer.WriteEndDocument();
				this.writer.Close();
			}
			
			private static XmlWriterSettings WriterSettings
			{
				get
				{
					XmlWriterSettings settings = new XmlWriterSettings();
					settings.CloseOutput = true;
					settings.Encoding = Encoding.UTF8;
					settings.Indent = true;
					settings.IndentChars = "\t";
					return settings;
				}
			}
			
			public void Dispose()
			{
				this.Close();
				
				Dispose(true);
				// This object will be cleaned up by the Dispose method.
				// Therefore, you should call GC.SupressFinalize to
				// take this object off the finalization queue
				// and prevent finalization code for this object
				// from executing a second time.
				GC.SuppressFinalize(this);
			}
			
			protected virtual void Dispose(bool disposing)
			{
				if(disposing)
				{

				}
			}
		}
	}
}
