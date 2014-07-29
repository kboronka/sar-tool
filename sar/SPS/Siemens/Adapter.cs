/* Copyright (C) 2014 Kevin Boronka
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
using System.Linq;
using System.IO;
using System.Net.Sockets;

using sar.Tools;

// specifications: http://tools.ietf.org/html/rfc905

namespace sar.SPS.Siemens
{
	public enum AddressArea : byte { P = 0x80, I = 0x81, Q = 0x82, M = 0x83, DB = 0x84, DI = 0x85, L = 0x86, VL = 0x87 };
	public enum Action : byte { Read = 0x4, Write = 0x5, ExchangePDU = 0xF0 };
	public enum TransportType : byte { Bit = 0x1, Byte = 0x2, Word = 0x4 };
	
	public class Adapter
	{
		private string ipAddress;
		private TcpClient socket;
		private NetworkStream stream;
		
		private static readonly byte[] CONNECT_TO_ADAPTER = 	new byte[] { 0x11, 0xE0, 0x00, 0x00, 0x00, 0x01, 0x00, 0xC0, 0x01, 0x09, 0xC1, 0x02, 0x4B, 0x54, 0xC2, 0x02, 0x03, 0x02 };
		private static readonly byte[] CONNECTED_TO_ADAPTER =	new byte[] { 0x11, 0xD0, 0x00, 0x01, 0x00, 0x00, 0x00, 0xC0, 0x01, 0x09, 0xC1, 0x02, 0x4B, 0x54, 0xC2, 0x02, 0x03, 0x02 };		
		private static readonly byte[] EXCHANGE_PDU_PARAMETER =	new byte[] { 0xF0, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0xF0 };
		private static readonly byte[] TPKT =					new byte[] { 0x03, 0x00, 0x00, 0x1F };

		public Adapter(string ipAddress)
		{
			this.ipAddress = ipAddress;
			this.Connect();
			//TODO: check if connection is established, handle retrys... possibly use a timed loop
		}
		
		private bool Connect()
		{
			// open a TCP connection to S7 via port 102
			this.socket = new TcpClient(this.ipAddress, 102);
			this.stream = this.socket.GetStream();

			// write data to socket
			byte[] message = IO.Combine(TPKT, CONNECT_TO_ADAPTER);
			EncodeTPKTSize(ref message);
			stream.Write(message, 0, message.Length);

			// wait for responce
			byte[] responce = new byte[22];
			int responceSize = stream.Read(responce, 0, responce.Length);
			
			return responce.SequenceEqual(CONNECTED_TO_ADAPTER);
		}
		
		private byte[] ReadWriteMessage(Action action, AddressArea addressArea, ushort dataBlock, uint startAddress, ushort length)
		{
			byte[] message = new byte[] {(byte)action, 0x1};
			
			// item helder
			message = IO.Combine(message, new byte[] { 0x12, 0x0A, 0x10 });
			
			// transport type
			message = IO.Combine(message, new byte[]  { (byte)TransportType.Byte });
			
			// transport type
			message = IO.Combine(message, new byte[]  { (byte)TransportType.Byte });

			// length (bytes)
			message = IO.Combine(message, IO.Split(length));

			// DB number
			if (addressArea != AddressArea.DB) dataBlock = 0;
			message = IO.Combine(message, IO.Split(dataBlock));

			// start address
			startAddress = startAddress * 8;
			message = IO.Combine(message, IO.SubSet(IO.Split(startAddress), 1, 3));

			return message;
		}
		
		private byte[] EncodeTPDU(byte[] parameterCode, byte[] parameterValue)
		{
			if (parameterCode.Length < 8) throw new InvalidDataException("incorred parameter code size");
			if (parameterValue.Length > 32) throw new InvalidDataException("parameter value too large");

			Action action = (Action)parameterCode[0];
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

			byte[] message = TPKT;
			
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
		
		private int ReadInt()
		{
			return 0;
		}

		/*
Trying to read 16 bytes from FW0.
PDU header:
                            0:0x32,0x01,0x00,0x00,0x00,0x00,0x00,0x0E,0x00,0x00,

plen: 14 dlen: 0
Parameter:
                            0:0x04,0x01,0x12,0x0A,0x10,0x02,0x00,0x10,0x00,0x00,0x83,0x00,0x00,0x00,
_daveExchange PDU number: 65537
IF1 enter _daveExchangeTCP
send packet: :
                            0:0x03,0x00,0x00,0x1F,0x02,0xF0,0x80,0x32,0x01,0x00,0x00,0x00,0x01,0x00,0x0E,0x00,
                            10:0x00,0x04,0x01,0x12,0x0A,0x10,0x02,0x00,0x10,0x00,0x00,0x83,0x00,0x00,0x00,
readISOpacket: 41 bytes read, 41 needed
readISOpacket: packet:
                            0:0x03,0x00,0x00,0x29,0x02,0xF0,0x80,0x32,0x03,0x00,0x00,0x00,0x01,0x00,0x02,0x00,
                            10:0x14,0x00,0x00,0x04,0x01,0xFF,0x04,0x00,0x80,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                            20:0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
IF1 _daveExchangeTCP res from read 41
result of exchange: 0
PDU header:
                            0:0x32,0x03,0x00,0x00,0x00,0x01,0x00,0x02,0x00,0x14,0x00,0x00,
plen: 2 dlen: 20
Parameter:
                            0:0x04,0x01,
Data     :
                            0:0xFF,0x04,0x00,0x80,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                            10:0x00,0x00,0x00,0x00,
Data hdr :
                            0:0xFF,0x04,0x00,0x80,
Data     :
                            0:0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
error: ok
_daveSetupReceivedPDU() returned: 0=ok
_daveTestReadResult() returned: 0=ok
FD0: 0
FD4: 0
FD8: 0
FD12: 0.000000
Finished.
		 *
		 */
	}
}
