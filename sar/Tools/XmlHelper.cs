/* Copyright (C) 2017 Kevin Boronka
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
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace sar.Tools
{
	public static class XML
	{
		public const string DATE = "yyyy-MM-dd";
		public const string DATETIME = "yyyy-MM-dd HH:mm:ss";
		public const string DATETIME_LONG = "yyyy-MM-dd HH:mm:ss.fff";
		
		public const string TIME = "HH:mm:ss";
		public const string TIME_LONG = "HH:mm:ss.fff";
		
		public static IFormatProvider NumberFormat = CultureInfo.InvariantCulture.NumberFormat;

		public class Reader : IDisposable
		{
			private XmlReader reader;
			
			public XmlNodeType NodeType {
				get { return reader.NodeType; }
			}
			
			public string Name {
				get { return reader.Name; }
			}
			
			public string Value {
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
				try {
					string attributeValue = this.reader.GetAttribute(name);
					return (String.IsNullOrEmpty(attributeValue)) ? "" : this.reader.GetAttribute(name);
				} catch {
					
				}
				
				return "";
			}
			
			public DateTime GetAttributeTimestamp(string name)
			{
				try {
					string attributeValue = this.reader.GetAttribute(name);
					return DateTime.ParseExact(attributeValue, XML.DATETIME_LONG, DateTimeFormatInfo.InvariantInfo);
				} catch {
					
				}

				return new DateTime(2001, 1, 1);
			}
			
			public TimeSpan GetAttributeTimeSpan(string name)
			{
				try {
					string attributeValue = this.reader.GetAttribute(name);
					return DateTime.ParseExact(attributeValue, XML.TIME, DateTimeFormatInfo.InvariantInfo).TimeOfDay;
				} catch {
					
				}

				return new TimeSpan(0, 0, 0);
			}

			public bool GetAttributeBoolean(string name)
			{
				try {
					string attributeValue = GetAttributeString(name);
					return (attributeValue == "true");
				} catch {
					
				}
				
				return false;
			}
			
			public long GetAttributeLong(string name)
			{
				try {
					string attributeValue = GetAttributeString(name);
					return long.Parse(attributeValue);
				} catch {
					
				}
				
				return 0;
			}
			
			public string GetValueString()
			{
				try {
					reader.Read();
					//TODO: add some error checking to verify element type
					return reader.Value;
				} catch {
					
				}
				
				return "";
			}
			
			public DateTime GetValueTimestamp()
			{
				try {
					string value = this.GetValueString();
					return DateTime.ParseExact(value, XML.DATETIME_LONG, DateTimeFormatInfo.InvariantInfo);
				} catch {
					
				}

				return new DateTime(2001, 1, 1);
			}
			
			public TimeSpan GetValueTimeSpan()
			{
				try {
					string value = this.GetValueString();
					return DateTime.ParseExact(value, XML.TIME, DateTimeFormatInfo.InvariantInfo).TimeOfDay;
				} catch {
					
				}

				return new TimeSpan(0, 0, 0);
			}

			public bool GetValueBoolean()
			{
				try {
					string value = this.GetValueString();
					return (value == "true");
				} catch {
					
				}
				
				return false;
			}
			
			public long GetValueLong()
			{
				try {
					string value = this.GetValueString();
					return long.Parse(value);
				} catch {
					
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
			
			private static XmlReaderSettings ReaderSettings {
				get {
					var settings = new XmlReaderSettings();
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
				GC.SuppressFinalize(this);
			}
			
			protected virtual void Dispose(bool disposing)
			{
				if (disposing) {

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
				if (!String.IsNullOrEmpty(value))
					this.writer.WriteAttributeString(name, value);
			}
			
			public void WriteAttributeString(string name, DateTime value)
			{
				string attributeValue = value.ToString(XML.DATETIME_LONG);
				this.WriteAttributeString(name, attributeValue);
			}

			public void WriteAttributeString(string name, TimeSpan value)
			{
				string attributeValue = value.ToString();
				this.WriteAttributeString(name, attributeValue);
			}
			
			public void WriteAttributeString(string name, bool value)
			{
				string elementValue = value ? "true" : "false";
				this.WriteElement(name, elementValue);
			}
			
			public void WriteAttributeString(string name, long value)
			{
				string attributeValue = value.ToString();
				this.WriteAttributeString(name, attributeValue);
			}
			
			public void WriteElement(string element, string value)
			{
				if (String.IsNullOrEmpty(value))
					return;
				if (String.IsNullOrEmpty(element))
					return;
				
				this.WriteStartElement(element);
				this.WriteValue(value);
				this.WriteEndElement();
			}

			public void WriteElement(string element, DateTime value)
			{
				string elementValue = value.ToString(XML.DATETIME_LONG);
				this.WriteElement(element, elementValue);
			}

			public void WriteElement(string element, TimeSpan value)
			{
				string elementValue = value.ToString();
				this.WriteElement(element, elementValue);
			}

			public void WriteElement(string element, bool value)
			{
				string elementValue = value ? "true" : "false";
				this.WriteElement(element, elementValue);
			}
			
			public void WriteElement(string element, long value)
			{
				string elementValue = value.ToString();
				this.WriteElement(element, elementValue);
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
			
			private static XmlWriterSettings WriterSettings {
				get {
					var settings = new XmlWriterSettings();
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
				GC.SuppressFinalize(this);
			}
			
			protected virtual void Dispose(bool disposing)
			{
				if (disposing) {

				}
			}
		}
	}
}
