using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using sar.Tools;

namespace sar.SPS
{
	public class SPSSocket
	{
		System.Net.Sockets.Socket socket;
		
		public SPSSocket(string ipAddress, int port)
		{
			IPAddress address = IPAddress.Parse(ipAddress);
			IPEndPoint remoteEP = new IPEndPoint(address, port);

			socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			socket.Connect(remoteEP);
		}
		
		public byte[] Write(byte[] message)
		{
			this.socket.Send(message);
			return Read(800);
		}
		
		private byte[] Read(int timeout)
		{
			this.socket.ReceiveTimeout = timeout;
			byte[] buffer = new byte[this.socket.ReceiveBufferSize];
			int responceSize = this.socket.Receive(buffer);
			return IO.SubSet(buffer, 0, Convert.ToByte(responceSize));
		}
	}
}
