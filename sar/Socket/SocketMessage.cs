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
using System.Diagnostics;
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
		private string member;
		private string data;
		private long fromID;
		private long toID;
		private DateTime timestamp;
		
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
		
		public DateTime Timestamp
		{
			get { return timestamp; }
		}
		
		#endregion
		
		public SocketMessage(SocketMemCache client, string command, long messageID, string member, string data, long destinationID)
		{
			this.command = command;
			this.id = messageID;
			this.member = member;
			this.data = data;
			this.fromID = client.ID;
			this.toID = destinationID;
			this.timestamp = DateTime.Now;
		}
		
		public SocketMessage(long messageID, SocketValue data, long destinationID)
		{
			this.command = "set";
			this.id = messageID;
			this.member = data.Name;
			this.data = data.Data;
			this.fromID = data.SourceID;
			this.toID = destinationID;
			this.timestamp = data.Timestamp;
		}
		
		public SocketMessage(XML.Reader reader)
		{
			try
			{
				if (reader.NodeType != XmlNodeType.Element) throw new XmlException("SocketMessage Element Required");
				if (reader.Name != "SocketMessage") throw new XmlException("SocketMessage Element Required");
				
				this.id = reader.GetAttributeLong("id");
				this.command = reader.GetAttributeString("command");
				this.member = reader.GetAttributeString("member");
				this.fromID = reader.GetAttributeLong("from");
				this.toID = reader.GetAttributeLong("to");
				long length = reader.GetAttributeLong("len");
				this.timestamp = reader.GetAttributeTimestamp("timestamp");
				if (length > 0) reader.Read();
				this.data = reader.Value;
			}
			catch (Exception)
			{

			}
		}
		
		public SocketMessage()
		{
		}
		
		public void Serialize(XML.Writer writer)
		{
			writer.WriteStartElement("SocketMessage");
			writer.WriteAttributeString("id", this.id);
			writer.WriteAttributeString("command", this.command);
			writer.WriteAttributeString("member", this.member);
			writer.WriteAttributeString("from", this.fromID);
			writer.WriteAttributeString("to", this.toID);
			writer.WriteAttributeString("timestamp", this.timestamp);
			
			if (String.IsNullOrEmpty(this.data))
			{
				writer.WriteAttributeString("len", 0);
				writer.WriteValue("");
			}
			else
			{
				writer.WriteAttributeString("len", this.data.Length);
				writer.WriteValue(this.data);
			}
			
			writer.WriteEndElement();		// SocketMessage
		}
	}
}
