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
		protected bool readOnly;
		
		public Configuration()
		{
			// check if local read-only configuration file exists
			var localPath = ApplicationInfo.CurrentDirectory + sar.Tools.AssemblyInfo.Name + ".xml";
			var standardPath = ApplicationInfo.CommonDataDirectory + sar.Tools.AssemblyInfo.Name + ".xml";
			
			if (File.Exists(localPath))
			{
				this.path = localPath;
				readOnly = true;
			}
			else
			{
				this.path = standardPath;
				readOnly = false;
			}
			
			// read the file
			if (File.Exists(this.path))
			{
				var reader = new XML.Reader(this.path);
				this.Deserialize(reader);
				reader.Close();
			}
			else
			{
				this.InitDefaults();
				this.Save();
			}
		}
		
		public Configuration(string path)
		{
			this.path = path;
			this.readOnly = true;
			
			if (File.Exists(this.path))
			{
				var reader = new XML.Reader(this.path);

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
			if (!readOnly)
			{
				this.Save(this.path);
			}
		}
		
		public void Save(string path)
		{
			if (!readOnly)
			{
				this.path = path;
				var writer = new XML.Writer(path);
				
				try
				{
					this.Serialize(writer);
				}
				catch
				{
					
				}
				
				writer.Close();
			}
		}
		
		protected abstract void Deserialize(XML.Reader reader);
		protected abstract void Serialize(XML.Writer writer);
		protected abstract void InitDefaults();
	}
}
