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
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using sar.Tools;

// specifications: http://tools.ietf.org/html/rfc905

namespace sar.S7Siemens
{
	public enum Action : byte { Read = 0x4, Write = 0x5, ExchangePDU = 0xF0 };
	public enum TransportType : byte { Bit = 0x1, Byte = 0x2, Word = 0x4 };
	
	public class Adapter : IDisposable
	{
		protected string ipAddress;
		private SimpleSocket socket;
		
		private bool connected;
		
		private static readonly byte[] CONNECT_TO_ADAPTER = 	new byte[] { 0x11, 0xE0, 0x00, 0x00, 0x00, 0x01, 0x00, 0xC0, 0x01, 0x09, 0xC1, 0x02, 0x4B, 0x54, 0xC2, 0x02, 0x03, 0x02 };
		private static readonly byte[] CONNECTED_TO_ADAPTER =	new byte[] { 0x11, 0xD0, 0x00, 0x01, 0x00, 0x00, 0x00, 0xC0, 0x01, 0x09, 0xC1, 0x02, 0x4B, 0x54, 0xC2, 0x02, 0x03, 0x02 };
		private static readonly byte[] EXCHANGE_PDU_PARAMETER =	new byte[] { 0xF0, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0xF0 };
		private static readonly byte[] TPKT =					new byte[] { 0x03, 0x00, 0x00, 0x1F };
		private static readonly byte[] TPKT_PDU =				new byte[] { 0x03, 0x00, 0x00, 0x19 };

		#region constructors
		
		protected Adapter(string ipAddress, bool simulation)
		{
			// can be used for simulation
			this.ipAddress = ipAddress;
			connected = simulation ? simulation : this.Connect();
		}
		
		public Adapter(string ipAddress)
		{
			this.ipAddress = ipAddress;
			connected = this.Connect();
			//TODO: check if connection is established, handle retrys... possibly use a timed loop
		}
		
		~Adapter()
		{
			Dispose(false);
		}
		
		public void Dispose()
		{
			Dispose(true);
		}
		
		bool disposed = false;
		protected void Dispose(bool disposing)
		{
			if (disposed) return;
			
			if (disposing)
			{
				try
				{
					this.Disconnect();
				}
				catch
				{
					
				}
			}

			disposed = true;
		}
		
		private bool Connect()
		{
			try
			{
				Logger.Log("s7Adaptor.Connect >> " + this.ipAddress);
				
				// TODO: add automatic retry

				// open a TCP connection to S7 via port 102
				this.socket = new SimpleSocket(this.ipAddress, 102);

				// write data to socket
				byte[] message = IO.Combine(TPKT, CONNECT_TO_ADAPTER);
				EncodeTPKTSize(ref message);
				DebugWrite("connect message", message);
				byte[] response = socket.Write(message);
				DebugWrite("response", response);
				this.connected = response.SequenceEqual(CONNECTED_TO_ADAPTER);
				
				// TODO: end of automatic retry

				
				// exchange PDU
				message = EncodeTPDU(TPKT_PDU, EXCHANGE_PDU_PARAMETER);
				DebugWrite("ExchangePDU", message);
				response = socket.Write(message);
				DebugWrite("response", response);
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
				return false;
			}
			
			return this.connected;
		}
		
		private void Disconnect()
		{
			try
			{
				if (this.connected) this.connected = false;
				if (this.socket != null) this.socket.Close();
				this.socket = null;
			}
			catch
			{
				
			}
		}
		
		public void Reconnect()
		{
			this.Disconnect();
			GC.Collect();
			Thread.Sleep(5000);
			this.Connect();
		}

		#endregion

		#region Reading from PLC
		public bool ReadBit(string address)
		{
			var data = ReadBytesRaw(address, 1);
			return data[0] > 0;
		}

		public Int16 ReadINT(string address)
		{
			var data = ReadBytesRaw(address, 2);
			return BitConverter.ToInt16(data, 0);
		}

		public Int16[] ReadINT(string address, uint Quantity)
		{
			var data = ReadBytesRaw(address, 2 * Quantity);
			var Result = new Int16[Quantity];

			for (int i = 0; i < Result.Length; i++)
			{
				Result[i] = BitConverter.ToInt16(data, i * 2);
			}

			return Result;
		}

		public Int32 ReadDINT(string address)
		{
			byte[] data = ReadBytesRaw(address, 4);
			return BitConverter.ToInt32(data, 0);
		}
		
		public Int32[] ReadDINT(string address, uint Quantity)
		{
			var data = ReadBytesRaw(address, 4 * Quantity);
			var Result = new Int32[Quantity];

			for (int i = 0; i < Result.Length; i++)
			{
				Result[i] = BitConverter.ToInt32(data, i * 4);
			}

			return Result;
		}

		public Single ReadFLOAT(string address)
		{
			var data = ReadBytesRaw(address, 4);
			return BitConverter.ToSingle(data, 0);
		}

		public Single[] ReadFLOAT(string address, uint Quantity)
		{
			var data = ReadBytesRaw(address, 4 * Quantity);
			var Result = new Single[Quantity];

			for (int i = 0; i < Result.Length; i++)
			{
				Result[i] = BitConverter.ToSingle(data, i * 4);
			}

			return Result;
		}
		
		public string ReadString(string address, ushort length)
		{
			var data = IO.ReverseBytes(ReadBytesRaw(address, length));
			
			string output = "";
			for (var i=2; i<data.Length && i<(data[1]+2); i++)
			{
				output += Convert.ToChar(data[i]);
			}
			
			return output;
		}		
		
		public byte[] ReadBytes(string address, ushort bytes)
		{
			var data = IO.ReverseBytes(ReadBytesRaw(address, bytes));
			return data;
		}

		private const int MAX_BYTES_PER_PAGE = 220;
		private byte[] ReadBytesRaw(string address, uint bytes)
		{
			var s7address = new Address(address);
			s7address.byteLength = (ushort)bytes;
			
			return ReadBytesRaw(s7address);
		}
		
		protected virtual byte[] ReadBytesRaw(Address address)
		{
			var bytes = address.byteLength;
			
			if (bytes > 65535) throw new IndexOutOfRangeException("max bytes = 65535");
			if (bytes < 1) throw new IndexOutOfRangeException("min bytes = 1");
			
			var message = new byte[] {};
			var tpdu = new byte[] {};
			var response = new byte[] {};
			var data = new byte[] {};
			

			
			// maximum page size = 220 bytes
			if (bytes <= MAX_BYTES_PER_PAGE)
			{
				try
				{
					// send read request message
					message = ReadWriteMessage(Action.Read, address);
					DebugWrite("ReadWriteMessage", message);
					
					tpdu = EncodeTPDU(TPKT, message);
					DebugWrite("TPDU", tpdu);
					
					response = socket.Write(tpdu);
					DebugWrite("response", response);
					
					data = ExtractTPDU(response);
					DebugWrite("data", data);
					
					if (data.Length != bytes) throw new ApplicationException("responce data size does not match requested size");
				}
				catch (SocketException ex)
				{
					Logger.Log("s7Adaptor ERROR");
					Logger.Log(" >> " + this.ipAddress);
					Logger.Log(" >> " + address.ToString());
					
					// auto-reconnect
					this.Reconnect();
					
					throw ex;
				}
				catch (Exception ex)
				{
					Logger.Log("s7Adaptor ERROR");
					Logger.Log(" >> " + this.ipAddress);
					Logger.Log(" >> " + address.ToString());
					Logger.Log(StringHelper.ArrayToString("ReadWriteMessage", message));
					Logger.Log(StringHelper.ArrayToString("TPDU", tpdu));
					Logger.Log(StringHelper.ArrayToString("response", response));
					Logger.Log(StringHelper.ArrayToString("data", data));

					throw ex;
				}
				
				return data;
			}
			else
			{
				data = new byte[bytes];
				
				address.byteLength = MAX_BYTES_PER_PAGE;
				Array.Copy(ReadBytesRaw(address), 0, data, bytes-MAX_BYTES_PER_PAGE, MAX_BYTES_PER_PAGE);
				
				address.byteAdddress += MAX_BYTES_PER_PAGE;
				address.startAddress += (MAX_BYTES_PER_PAGE * 8);
				address.byteLength = (ushort)(bytes - MAX_BYTES_PER_PAGE);
				Array.Copy(ReadBytesRaw(address), 0, data, 0, address.byteLength);
				
				return data;
			}
		}
		#endregion

		#region Writing to PLC
		public void WriteFloats(string address, float[] data)
		{
			var returnByte = new byte[data.Length * 4];
			Buffer.BlockCopy(data, 0, returnByte, 0, returnByte.Length);
			WriteBytes(address, returnByte);

		}
		public void WriteDints(string address, Int32[] data)
		{
			var returnByte = new byte[] { };
			var tmpByte = new byte[4] { 0, 0,0,0 };
			Array.Resize(ref returnByte, (data.Length) * 4);
			int cnt = 0;
			foreach (var _data in data)
			{
				tmpByte = BitConverter.GetBytes(_data);
				returnByte[cnt + 3] = tmpByte[0];
				returnByte[cnt + 2] = tmpByte[1];
				returnByte[cnt + 1] = tmpByte[2];
				returnByte[cnt]     = tmpByte[3];
				cnt = cnt + 4;
			}
			WriteBytes(address, returnByte);
		}
		
		public void WriteInts(string address, Int16[] data)
		{
			var returnByte = new byte[]{};
			var tmpByte = new byte[2]{0,0};
			Array.Resize(ref returnByte, (data.Length)*2);
			int cnt = 0;
			foreach (var _data in data)
			{
				tmpByte= BitConverter.GetBytes(_data);
				returnByte[cnt + 1] = tmpByte[0];
				returnByte[cnt]     = tmpByte[1];
				cnt =cnt+2;
			}
			
			WriteBytes(address, returnByte);
		}
		
		public void WriteBytes(string address, byte[] data)
		{
			WriteBytesRaw(address, data);
		}
		
		protected void WriteBytesRaw(string address, byte[] data)
		{
			var s7address = new Address(address);
			s7address.byteLength = (ushort)data.Length;
			
			WriteBytesRaw(s7address, data);
		}
		
		protected virtual void WriteBytesRaw(Address address, byte[] data)
		{
			var bytes = address.byteLength;
			
			if (bytes > 65535) throw new IndexOutOfRangeException("max bytes = 65535");
			if (bytes < 1) throw new IndexOutOfRangeException("min bytes = 1");
			
			if (bytes != data.Length) throw new ArgumentException("data size does not match address");

			
			// send read request message
			var message = ReadWriteMessage(Action.Write, address);
			DebugWrite("ReadWriteMessage", message);
			
			message = EncodeTPDU(TPKT, message, data);
			DebugWrite("TPDU", message);
			
			var response = socket.Write(message);
			DebugWrite("response", response);
		}
		
		#endregion
		
		private byte[] ReadWriteMessage(Action action, Address address)
		{
			return ReadWriteMessage(action, address.area, address.dataBlock, address.startAddress, address.byteLength, address.transportType);
		}
		
		private byte[] ReadWriteMessage(Action action, Areas addressArea, ushort dataBlock, uint startAddress, ushort length, TransportType transportType)
		{
			var message = new byte[] {(byte)action, 0x1};
			
			// item helder
			message = IO.Combine(message, new byte[] { 0x12, 0x0A, 0x10 });
			
			// transport type
			message = IO.Combine(message, new byte[] { (byte)transportType });

			// length (bytes)
			message = IO.Combine(message, IO.Split(length));

			// DB number
			if (addressArea != Areas.DB) dataBlock = 0;
			message = IO.Combine(message, IO.Split(dataBlock));

			// address area
			message = IO.Combine(message, new byte[] { (byte)addressArea });

			// start address
			message = IO.Combine(message, IO.SubSet(IO.Split(startAddress), 1, 3));

			return message;
		}
		
		private byte[] EncodeTPDU(byte[] header, byte[] parameterCode)
		{
			return 	EncodeTPDU(header, parameterCode, new byte[] {});
		}
		
		private byte[] EncodeTPDU(byte[] header, byte[] parameterCode, byte[] parameterValue)
		{
			if (parameterCode.Length < 8) throw new InvalidDataException("incorred parameter code size");
			if (parameterValue.Length > 32) throw new InvalidDataException("parameter value too large");

			var action = (Action)parameterCode[0];
			ushort sequenceNumber = 0;
			byte writeSize = 0;
			
			switch (action)
			{
				case Action.ExchangePDU:
					sequenceNumber = 0x0D;
					break;
				case Action.Write:
					sequenceNumber = 0x2C6;
					// Data Header
					writeSize = Convert.ToByte(parameterValue.Length);
					parameterValue = IO.Combine(new byte[] { 0x0, 0x4, 0x0, (byte)(writeSize * 8) }, parameterValue);
					break;
				case Action.Read:
					sequenceNumber = 0x3301;
					break;
				default:
					throw new InvalidOperationException("unknown action " + action.ToString());
			}

			byte[] message = header;
			
			// iso 8073
			message = IO.Combine(message, new byte[] { 0x2, 0xF0, 0x80 });
			
			// PDU Header (10 bytes)
			//	constant = 0x32
			//	type = 0x1
			//	unknown = 0x0
			//	unknown = 0x0
			message = IO.Combine(message, new byte[] { 0x32, 0x1, 0x0, 0x0 });
			
			// sequence number (2 bytes)
			message = IO.Combine(message, IO.Split(sequenceNumber));

			// parameter size (2 bytes)
			message = IO.Combine(message, IO.Split(Convert.ToUInt16(parameterCode.Length)));
			
			// parameter value size (2 bytes)
			message = IO.Combine(message, IO.Split(Convert.ToUInt16(parameterValue.Length)));
			
			// parameter
			message = IO.Combine(message, parameterCode);
			message = IO.Combine(message, parameterValue);

			EncodeTPKTSize(ref message);
			
			return message;
		}
		
		private void EncodeTPKTSize(ref byte[] message)
		{
			// 0                   1                   2                   3
			// 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
			// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
			// |      vrsn     |    reserved   |          packet length        |
			// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
			
			if (message.Length > ushort.MaxValue) throw new IndexOutOfRangeException("tpkt size exceeded");
			ushort messageSize = Convert.ToUInt16(message.Length);
			
			message[2] = IO.Split(messageSize)[0];
			message[3] = IO.Split(messageSize)[1];
		}
		
		private static void DebugWrite(string title, byte[] data)
		{
			if (ConsoleHelper.ShowDebug)
			{
				Debug.WriteLine(StringHelper.ArrayToString(title, data));
			}
		}
		


		private byte[] ExtractTPDU(byte[] message)
		{
			// PDU
			byte[] PDU = IO.SubSet(message, 7, 12);
			int paramLength = BitConverter.ToInt16(IO.SubSetReversed(PDU, 6, 2), 0);
			int extractDataLength = BitConverter.ToInt16(IO.SubSetReversed(PDU, 8, 2), 0);
			int errorCode = BitConverter.ToInt16(IO.SubSetReversed(PDU, 10, 2), 0);
			
			// parameters
			byte[] parameters = IO.SubSet(message, 7+12, paramLength);
			var action = (Action)parameters[0];
			int itemsToRead = parameters[1];
			
			// extract data
			byte[] extractData = IO.SubSet(message, 7+12+paramLength, extractDataLength);
			bool dataValid = (extractData[0] == 0xFF);
			int dataTransportSize = extractData[1];
			int lengthInBytes = BitConverter.ToInt16(IO.SubSetReversed(extractData, 2, 2), 0) / 8;
			
			// data
			byte[] data = IO.SubSet(extractData, 4, extractDataLength - 4);
			Array.Reverse(data);
			
			return data;
		}
	}
}
