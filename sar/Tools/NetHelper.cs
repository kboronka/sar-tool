﻿/* Copyright (C) 2017 Kevin Boronka
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace sar.Tools
{
	public static class NetHelper
	{
		public static bool Ping(string ip, int timeout)
		{
			Ping ping = new Ping();
			PingReply pingReply = ping.Send(ip, timeout);

			return (pingReply.Status == IPStatus.Success);
		}

		public static bool Ping(string ip)
		{
			Ping ping = new Ping();
			PingReply pingReply = ping.Send(ip);

			return (pingReply.Status == IPStatus.Success);
		}

		public static bool WaitForPing(string ip, int timeout, bool expected)
		{
			bool found;
			Stopwatch timer = new Stopwatch();
			timer.Start();

			do
			{
				found = Ping(ip);
				if (found != expected)
					Thread.Sleep(10);
			} while (found != expected && (!(timer.ElapsedMilliseconds > timeout) || timeout == -1));

			return (found == expected);
		}

		public static string GetHostName(string uncPath)
		{
			string hostName = uncPath;

			if (hostName.StartsWith(@"\\"))
				hostName = hostName.Substring(2);
			if (hostName.Contains(@"\"))
				hostName = hostName.Substring(0, hostName.IndexOf(@"\"));

			return hostName;
		}

		public static string GetLAN_ConnectionName()
		{
			NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface adapter in adapters)
			{
				if (adapter.Description.ToLower().Contains("gigabit") || adapter.Description.ToLower().Contains("82579lm"))
				{
					return adapter.Name;
				}
			}

			return null;
		}

		public static int[] GetActiveTcpListenerPorts()
		{
			var usedPorts = new List<int>();
			var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			var listeners = ipGlobalProperties.GetActiveTcpListeners();
			IEnumerator myEnum = listeners.GetEnumerator();

			while (myEnum.MoveNext())
			{
				var ip = (IPEndPoint)myEnum.Current;
				usedPorts.Add(ip.Port);
			}

			return usedPorts.ToArray();
		}

		public static int[] GetActiveTcpConnectionPorts()
		{
			var usedPorts = new List<int>();
			var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			var listeners = ipGlobalProperties.GetActiveTcpConnections();
			IEnumerator myEnum = listeners.GetEnumerator();

			while (myEnum.MoveNext())
			{
				var ip = (IPEndPoint)myEnum.Current;
				usedPorts.Add(ip.Port);
			}

			return usedPorts.ToArray();
		}

		public static List<NetworkAdapter> Adapters()
		{
			List<NetworkAdapter> adapters = new List<NetworkAdapter>();

			// capture output of ipconfig /all command
			string ipconfig;
			ConsoleHelper.Run("ipconfig", "/all", out ipconfig);

			string name;
			int start = 0;
			int end = ipconfig.Length;

			do
			{
				name = StringHelper.RegexFindString(ipconfig.Substring(start + 1), "adapter (.+):");
				start = ipconfig.IndexOf(name + ":", start + 1);

				string next = StringHelper.RegexFindString(ipconfig.Substring(start + 1), "adapter (.+):");
				end = ipconfig.IndexOf(next + ":", start + 1);
				if (end < start)
					end = ipconfig.Length;

				string details = ipconfig.Substring(start, end - start);

				string mediaState = StringHelper.RegexFindString(details, "Media State . . . . . . . . . . . : (.+)\n");
				string physicalAddress = StringHelper.RegexFindString(details, "Physical Address. . . . . . . . . : (.+)\n");
				string DHCP = StringHelper.RegexFindString(details, "DHCP Enabled. . . . . . . . . . . : (.+)\n");
				string ip = StringHelper.RegexFindString(details, "IPv4 Address. . . . . . . . . . . : (.+)\n");
				string mask = StringHelper.RegexFindString(details, "Subnet Mask . . . . . . . . . . . : (.+)\n");
				string gateway = StringHelper.RegexFindString(details, "Default Gateway . . . . . . . . . : (.+)\n");

				if (!string.IsNullOrEmpty(ip) && ip.Contains("("))
				{
					ip = ip.Substring(0, ip.IndexOf('('));
				}

				if (!string.IsNullOrEmpty(name) && !name.StartsWith("isatap") && !name.StartsWith("Teredo Tunneling"))
				{
					NetworkAdapter adapter = new NetworkAdapter();
					adapter.Name = name;
					adapter.IPAddress = ip;
					adapter.SubnetMask = mask;
					adapter.Gateway = gateway;
					adapter.PhysicalAdddress = physicalAddress;
					adapter.Connected = (mediaState == "");
					adapter.DHCP = (DHCP == "Yes");
					adapters.Add(adapter);
				}

				ConsoleHelper.DebugWriteLine("name:\t\t" + StringHelper.AddQuotes(name));
				ConsoleHelper.DebugWriteLine("mediaState:\t" + StringHelper.AddQuotes(mediaState));
				ConsoleHelper.DebugWriteLine("MAC:\t\t" + StringHelper.AddQuotes(physicalAddress));
				ConsoleHelper.DebugWriteLine("DHCP:\t\t" + StringHelper.AddQuotes(DHCP));
				ConsoleHelper.DebugWriteLine("ip:\t\t" + StringHelper.AddQuotes(ip));   // TODO: remove (Preferred)
				ConsoleHelper.DebugWriteLine("mask:\t\t" + StringHelper.AddQuotes(mask));
				ConsoleHelper.DebugWriteLine("gateway:\t" + StringHelper.AddQuotes(gateway));
				ConsoleHelper.DebugWriteLine(ConsoleHelper.HR);
			} while (!String.IsNullOrEmpty(name));

			return adapters;
		}
	}

	public class NetworkAdapter
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string PhysicalAdddress { get; set; }
		public bool Connected { get; set; }
		public bool DHCP { get; set; }
		public string IPAddress { get; set; }
		public string SubnetMask { get; set; }
		public string Gateway { get; set; }
	}
}