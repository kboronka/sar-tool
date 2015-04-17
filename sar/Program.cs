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

using sar.Tools;

// binary download: https://sar-tool.googlecode.com/svn/trunk/release/sar.exe
// release download: https://sar-tool.googlecode.com/svn/tags/

namespace sar
{
	public class Program
	{
		public static int Main(string[] args)
		{
			try
			{
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
 
				Environment.CurrentDirectory = @"C:\Jobs\12011\repo\LabelPrinter\LabelPrinter v1.0.0.0";
				args = new string[] { "-b.net", "3.5", @"LabelPrinter.sln", "/p:Configuration=Release /p:Platform=\"Any CPU\"" };
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
		
		#region loging functions
		
		// TODO: do we really to Log method or is the base class good enough
		public static void Log(Exception ex)
		{
			try
			{
				sar.Base.Program.Log(ex);
			}
			catch
			{
				// surpress any error
			}
		}
		
		public static void Log(string message)
		{
			try
			{
				sar.Base.Program.Log(message);
			}
			catch
			{
				// surpress any error
			}
		}
		
		#endregion
	}
}