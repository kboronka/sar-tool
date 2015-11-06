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
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using sar.Tools;

namespace sar.Tools
{
	public class SimpleSocket
	{
		private System.Net.Sockets.Socket socket;
		private bool connected;
		private string ipAddress;
		private int port;
		
		public SimpleSocket(string ipAddress, int port)
		{
			this.ipAddress = ipAddress;
			this.port = port;
			
			this.Connect();
		}
		
		private void Connect()
		{
			try
			{
				IPAddress address = IPAddress.Parse(this.ipAddress);
				var remoteEP = new IPEndPoint(address, this.port);

				socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.SendTimeout = 10000;
				
				socket.Connect(remoteEP);

				connected = socket.Connected;
			}
			catch (Exception ex)
			{
				Program.Log(ex);
			}
		}
		
		public void Disconnect()
		{
			try
			{
				socket.Disconnect(false);
				socket = null;
			}
			catch (Exception ex)
			{
				Program.Log(ex);
			}
			
			connected = false;
		}
		
		public byte[] Write(byte[] message)
		{
			if (!this.connected) this.Connect();

			try
			{
				this.socket.Send(message);
				return Read(800);
			}
			catch (Exception ex)
			{
				this.Disconnect();
				throw ex;
			}
		}
		
		private byte[] Read(int timeout)
		{
			try
			{
				this.socket.ReceiveTimeout = timeout;
				var buffer = new byte[this.socket.ReceiveBufferSize];
				int responseSize = this.socket.Receive(buffer);
				return IO.SubSet(buffer, 0, responseSize);
			}
			catch (Exception ex)
			{
				this.Disconnect();
				throw ex;
			}
		}
	}
}
