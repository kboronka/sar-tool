/* Copyright (C) 2013 Kevin Boronka
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
using System.Text;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Threading;
using sar.Tools;


namespace sar.Tools
{
	public class NetHelper
	{
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
				if (found != expected) Thread.Sleep(10);
				
			} while (found != expected && (!(timer.ElapsedMilliseconds > timeout) || timeout == -1));
			
			return (found == expected);
		}
		
		public static List<NetworkAddaptor> Addaptors()
		{
			List<NetworkAddaptor> addaptors = new List<NetworkAddaptor>();
			string ipconfig;
			
			ConsoleHelper.Run("ipconfig", "/all", out ipconfig);
			/*
			info.WorkingCopyRootPath = IO.RegexFindString(result.Output, @"Working Copy Root Path:(.+)\n");
			info.Repository = IO.RegexFindString(result.Output, "URL:(.+)\n");
			info.RepositoryRoot = IO.RegexFindString(result.Output, "Repository Root:(.+)\n");
			info.Revision = int.Parse(IO.RegexFindString(result.Output, "Revision:(.+)\n"));
			 */
			//string result = StringHelper.RegexFindString(ipconfig, "adapter (.+):");
			//ConsoleHelper.DebugWriteLine("result: " +  StringHelper.AddQuotes(result));
			
			//string[] split = Regex.Split(ipconfig, "adapter (.+):");
			
			string name;
			int start = 0;
			int end = ipconfig.Length;
			do
			{
				string searchstring = ipconfig.Substring(start + 1);
				
				name = StringHelper.RegexFindString(searchstring, "adapter (.+):");
				start = ipconfig.IndexOf(name + ":", start + 1);
				name = StringHelper.RegexFindString(searchstring.Substring(2), "adapter (.+):");
				end = ipconfig.IndexOf(name + ":", start + 1);
				if (end < start) end = ipconfig.Length;
				string infostring = searchstring.Substring(0, end - start);
				
				
				string mediaState = StringHelper.RegexFindString(infostring, "Media State . . . . . . . . . . . : (.+)\n");
				string physicalAddress = StringHelper.RegexFindString(infostring, "Physical Address. . . . . . . . . : (.+)\n");
				string DHCP = StringHelper.RegexFindString(infostring, "DHCP Enabled. . . . . . . . . . . :(.+)\n");
				string ip = StringHelper.RegexFindString(infostring, "IPv4 Address. . . . . . . . . . . : (.+)\n");
				string mask = StringHelper.RegexFindString(infostring,"Subnet Mask . . . . . . . . . . . : (.+)\n");
				string gateway = StringHelper.RegexFindString(infostring,"Default Gateway . . . . . . . . . : (.+)\n");
				

				ConsoleHelper.DebugWriteLine("name:\t\t" + StringHelper.AddQuotes(name));
				ConsoleHelper.DebugWriteLine("mediaState:\t" + StringHelper.AddQuotes(mediaState));
				ConsoleHelper.DebugWriteLine("MAC:\t\t" + StringHelper.AddQuotes(physicalAddress));
				ConsoleHelper.DebugWriteLine("DHCP:\t\t" + StringHelper.AddQuotes(DHCP));
				ConsoleHelper.DebugWriteLine("ip:\t\t" + StringHelper.AddQuotes(ip));	// TODO: remove (Preferred)
				ConsoleHelper.DebugWriteLine("mask:\t\t" + StringHelper.AddQuotes(mask));
				ConsoleHelper.DebugWriteLine("gateway:\t" + StringHelper.AddQuotes(gateway));
				ConsoleHelper.DebugWriteLine(ConsoleHelper.HR);

				
			} while (!String.IsNullOrEmpty(name));

			
			return addaptors;
		}
	}
	
	public class NetworkAddaptor
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