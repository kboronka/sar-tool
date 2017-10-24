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
using System.Net;
using System.Net.NetworkInformation;

namespace sar.Socket
{
	public static class SocketHelper
	{
		public static int FindAvailablePort(int firstPort, int lastPort)
		{
			IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
			IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
			
			List<int> usedPorts = new List<int>();
			foreach (IPEndPoint endpoint in tcpEndPoints)
			{
				usedPorts.Add(endpoint.Port);
			}

			// skip port 87
			// http://superuser.com/questions/188058/which-ports-are-considered-unsafe-on-chrome
			for (int port = firstPort; port < lastPort; port++)
			{
				if (port == 87) continue;
				if (port == 95) continue;
				if (port >= 101 && port < 1000) port = 1000;
				if (!usedPorts.Contains(port)) return port;
			}
			
			return -1;
		}
	}
}
