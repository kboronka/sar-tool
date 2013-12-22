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
				XmlReader reader = XmlReader.Create(this.path, StringHelper.ReaderSettings);
				this.Deserialize(reader);
				reader.Close();
			}
		}
		
		public Configuration(string path)
		{
			this.path = path;
			
			if (File.Exists(this.path))
			{
				XmlReader reader = XmlReader.Create(this.path, StringHelper.ReaderSettings);

				try
				{
					this.Deserialize(reader);
				}
				catch
				{
					
				}
				
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
			XmlWriter writer = XmlWriter.Create(path, StringHelper.WriterSettings);
			
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
