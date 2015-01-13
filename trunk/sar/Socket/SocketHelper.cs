
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net;

namespace sar.Socket
{
	public static class SocketHelper
	{
		private static int FindAvailablePort(int firstPort, int lastPort)
		{
			IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
			IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
			
			List<int> usedPorts = new List<int>();
			foreach (IPEndPoint endpoint in tcpEndPoints)
			{
				usedPorts.Add(endpoint.Port);
			}

			for (int port = firstPort; port < lastPort; port++)
			{
				if (!usedPorts.Contains(port)) return port;
			}
			
			return -1;
		}
	}
}
