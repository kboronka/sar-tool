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
using System.Timers;
using System.Threading;
using sar.Tools;

namespace sar.Tools
{
	public class Progress
	{
		private static bool enabled;
		private static bool started;
		private static int i = 0;
		private static string status = "running";
		
		public static bool Enabled
		{
			get { return enabled; }
			set	{ enabled = value; }
		}
		
		public static String Message
		{
			set { Progress.status = value; }
		}
		
		private static void Update(object sender, ElapsedEventArgs e)
		{
			try
			{

			}
			catch
			{
				
			}
		}
		
		public static void Enable()
		{
			started = true;
			
			while (started && Tools.ApplicationInfo.IsWinVistaOrHigher)
			{
				Thread.Sleep(100);
				
				if (enabled)
				{
					if (++Progress.i >= 6) Progress.i = 0;
					
					ConsoleHelper.WriteProgress("\r" + Progress.status + new String('.', i) + new String(' ', 79 - Progress.status.Length - i) + "\r", ConsoleColor.Cyan);
				}
			}
		}
		
		public static void Disable()
		{
			started = false;
		}
	}
}
