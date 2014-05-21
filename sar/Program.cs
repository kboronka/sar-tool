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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

using sar.Commands;
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
				/*
				#if DEBUG
				args = new string[] { "f.rd", @"C:\Users\kboronka\Documents\Virtual Machines\caches" };
				args = new string[] { "rdp", "192.168.171.208", @"atsna\kboronka", "password", @"/pause" };
				#endif
				*/
				CommandHub hub = new CommandHub();
				ConsoleHelper.Start();
				if (args.Length == 0) ConsoleHelper.ApplicationTitle();
				int exitCode = hub.ProcessCommands(args);
				
				ConsoleHelper.Shutdown();
				return exitCode;
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteException(ex);
				ConsoleHelper.Shutdown();
				return ConsoleHelper.EXIT_ERROR;
			}
		}
		
		#region loging functions
		
		public static void Log(Exception ex)
		{
			try
			{
				sar.Base.Program.Log(ex);
			}
			catch
			{
				
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

			}
		}
		
		#endregion
	}
}