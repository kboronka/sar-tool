/* Copyright (C) 2021 Kevin Boronka
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
using System.Threading;

namespace sar.Tools
{
	public static class Progress
	{
		private static int i = 0;
		private static string status = "running";

		public static bool Enabled { get; set; }

		public static String Message
		{
			set
			{
				lock (Progress.status)
				{
					Progress.status = value;
				}
			}
		}

		#region background worker

		private static Thread messageLoopThread;
		private static bool messageLoopShutdown;

		public static void Start()
		{
			messageLoopThread = new Thread(Progress.ProgressMessageLoop);
			messageLoopThread.Name = "Progress Message";
			messageLoopThread.IsBackground = true;
			messageLoopThread.Start();
		}

		public static void Stop()
		{
			messageLoopShutdown = true;

			if (messageLoopThread != null && messageLoopThread.IsAlive)
			{
				messageLoopThread.Join();
				messageLoopThread.Abort();
				messageLoopThread = null;
			}
		}

		public static void ProgressMessageLoop()
		{
			const int TERMINAL_CHARACTER_WIDTH = 80;

			while (!messageLoopShutdown)
			{
				Thread.Sleep(100);

				try
				{
					if (Progress.Enabled)
					{
						if (++Progress.i >= 6)
							Progress.i = 0;

						lock (Progress.status)
						{
							var message = Progress.status;

							if (message.Length > (TERMINAL_CHARACTER_WIDTH - 10))
							{
								message = message.Substring(0, (TERMINAL_CHARACTER_WIDTH - 10));
							}

							ConsoleHelper.WriteProgress("\r" + message + new String('.', i) + new String(' ', TERMINAL_CHARACTER_WIDTH - (i + 1) - message.Length) + "\r", ConsoleColor.Cyan);
						}
					}
				}
				catch
				{
					Thread.Sleep(5000);
				}
			}
		}

		public static void Disable()
		{
			messageLoopShutdown = false;
		}

		#endregion

	}
}
