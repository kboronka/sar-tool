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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using sar.Tools;

namespace sar.Tools
{
	public class SimpleSocket
	{
		System.Net.Sockets.Socket socket;
		
		public SimpleSocket(string ipAddress, int port)
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
			return IO.SubSet(buffer, 0, responceSize);
		}
	}
}
