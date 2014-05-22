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
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class NetSetIP : Command
	{
		public NetSetIP(Base.CommandHub parent) : base(parent, "Network - Set IP",
		                                 new List<string> { "net.setip", "ip.set" },
		                                 @"-ip.set addaptor ip",
		                                 new List<string> { @"-ip.set LAN dhcp /admin" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string addaptor = args[1];
			string ipaddress = args[2].ToLower();
			
			string ipconfig;
			
			Progress.Message = "Setting IP address of " + addaptor;
			int exitcode = ConsoleHelper.Run("netsh", "interface ip set address \"" + addaptor + "\" " + ((ipaddress != "dhcp") ? "static " : "") + ipaddress, out ipconfig);

			if (exitcode != 0)
			{
				ConsoleHelper.DebugWriteLine(ipconfig);
				ConsoleHelper.WriteLine("setting ip address of " + addaptor + " to " + ipaddress + " has failed", ConsoleColor.DarkYellow);
				return ConsoleHelper.EXIT_ERROR;
			}
			
			ConsoleHelper.WriteLine("ip address has been set to " + ipaddress + " successfully", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
