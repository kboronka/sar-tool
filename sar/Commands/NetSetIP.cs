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

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class NetSetIP : Command
	{
		public NetSetIP(Base.CommandHub parent) : base(parent, "Network - Set IP",
		                                               new List<string> { "net.setip", "ip.set" },
		                                               @"-ip.set adapters ip",
		                                               new List<string> { @"-ip.set LAN dhcp /admin",
		                                               	@"-ip.set LAN 192.168.0.11 255.255.255.0 /admin" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 3 || args.Length > 4)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string adapters = args[1];
			string ipaddress = args[2].ToLower();
			string subnetMask = "";
			
			if (ipaddress != "dhcp") 
			{
				if (args.Length != 4)
				{
					throw new ArgumentException("incorrect number of arguments");
				}
				
				subnetMask = args[3];
			}
			
			if (adapters == "gigabit" && NetHelper.GetLAN_ConnectionName() != null)
			{
				adapters = NetHelper.GetLAN_ConnectionName();
			}
			
			Progress.Message = "Setting IP address of " + adapters;
			string output;
			int exitcode = ConsoleHelper.Run("netsh", "interface ip set address \"" + adapters + "\" " + ((ipaddress != "dhcp") ? "static " : "") + ipaddress + " " + subnetMask, out output);

			if (exitcode != 0)
			{
				ConsoleHelper.DebugWriteLine(output);
				ConsoleHelper.WriteLine("setting ip address of " + adapters + " to " + ipaddress + " has failed", ConsoleColor.DarkYellow);
				return ConsoleHelper.EXIT_ERROR;
			}
			
			ConsoleHelper.WriteLine("ip address has been set to " + ipaddress + " successfully", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
