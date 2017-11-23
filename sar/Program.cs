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
using System.Linq;

using sar.Tools;

namespace sar
{
	public class Program : Base.Program
	{
		public static int Main(string[] args)
		{
			try
			{
				if (args.ToList().Exists(a => a == "/debug" || a == "/d" ))
				{
					ConsoleHelper.ShowDebug = true;
					ConsoleHelper.DebugWriteLine("Debug Mode Active");
					
					var count = 0;
					foreach (var arg in args)
					{
						ConsoleHelper.DebugWriteLine("arg[" + count++.ToString() + "] = " + arg);
					}
				}
				

				
				#if DEBUG
				
				/*
				args = new string[] { "f.rd", @"C:\Users\kboronka\Documents\Virtual Machines\caches", @"/pause" };
				args = new string[] { "rdp", "192.168.171.208", @"username", "password", @"/pause" };
				args = new string[] { "ip.set", "gigabit", "dhcp", @"/debug", @"/pause", @"/admin" };
				args = new string[] { "ip.set", "gigabit", "192.168.14.111", "255.255.255.0", @"/debug", @"/pause", @"/admin" };
				args = new string[] { "ip.config", @"/pause" };
				args = new string[] { "mssql-gs", "192.168.14.110", "TestDB", "sa", "test123", @"\scripts\", @"/pause" };
				args = new string[] { "f.open", "c:\temp" };
				args = new string[] { "file.open", @"c:\temp", @"/pause" };
				args = new string[] { "-b.net", "3.5", @"LabelPrinter.sln", "/p:Configuration=Release /p:Platform=\"Any CPU\"" };
				args = new string[] { "fanuc.downloadPR", "10.242.217.151", @"test path\TEST-(date)", @"/pause" };
				args = new string[] { "svn.GetAssemblyVersion", @"https://github.com/kboronka/sar-tool/trunk/sar/Properties/AssemblyInfo.cs", @"/pause" };
				args = new string[] { "svn.GetNewAssemblyVersion", @"https://github.com/kboronka/sar-tool/trunk/sar/Properties/AssemblyInfo.cs", @"/pause" };
				args = new string[] { "dotNetVersions", @"/pause" };
				args = new string[] { "r", "test.cs", @"break;\r\n(.*)case", @"break;\n\n\n$1case", @"/pause" };
				args = new string[] { "c.c", @"C:\Users\kboronka\Desktop\Manual", @"/pause" };
				args = new string[] { "c.c", @"C:\Users\kboronka\Documents\SharpDevelop Projects\sar", @"/pause" };
				args = new string[] { "hc", @"C:\Users\kboronka\Desktop\Test\Test", @"C:\Users\kboronka\Desktop\Test\Test\Views", @"/pause" };
				
				 */
				
				
				#endif				
				
				var hub = new CommandHub();
				Progress.Start();
				if (args.Length == 0) ConsoleHelper.ApplicationTitle();
				int exitCode = hub.ProcessCommands(args);
				
				Progress.Stop();
				return exitCode;
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteException(ex);
				Progress.Stop();
				return ConsoleHelper.EXIT_ERROR;
			}
		}
	}
}