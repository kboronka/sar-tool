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
using System.Threading;

using sar.Base;
using sar.Tools;

namespace sar.Commands
{
	public class rdp : Command
	{
		public rdp(Base.CommandHub parent) : base(parent, "RDP - Launch Session",
		                     new List<string> { "rdp" },
		                     @"-rdp host username password",
		                     new List<string> { "-rdp 192.168.0.101 admin password" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 4)
			{
				throw new ArgumentException("wrong number of arguments");
			}
			
			string host = args[1];
			string username = args[2];
			string password = args[3];
						
			Progress.Message = "Launching rdp session to " + host;
			ConsoleHelper.Run(Environment.SystemDirectory + @"\cmdkey.exe", "/generic:TERMSRV/" + host + " /user:" + username + " /pass:" + password);
			ConsoleHelper.Start(Environment.SystemDirectory + @"\mstsc.exe", "/v " + host + " /f");
			Thread.Sleep(3000);
			ConsoleHelper.Run(Environment.SystemDirectory + @"\cmdkey.exe", "/delete:TERMSRV/" + host);
			
			ConsoleHelper.WriteLine("RDP Session Launched", ConsoleColor.DarkYellow);
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}