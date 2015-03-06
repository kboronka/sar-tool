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
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class NetListAdapters : Command
	{
		public NetListAdapters(Base.CommandHub parent) : base(parent, "Network - List Adapters",
		                                 new List<string> { "ip.config" },
		                                 @"-ip.config",
		                                 new List<string> { "-ip.config" })
		{
		}
		
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			foreach (NetworkAdapter adapter in NetHelper.Adapters())
			{
				ConsoleHelper.Write(adapter.Name, ConsoleColor.White);
				ConsoleHelper.Write(": ");
				
				if (adapter.DHCP)
				{
					ConsoleHelper.Write("dhcp", ConsoleColor.Yellow);
					ConsoleHelper.Write(": ");
				}
				
				ConsoleHelper.Write(adapter.IPAddress + " " + adapter.SubnetMask);
				ConsoleHelper.WriteLine();
			}
			
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}
