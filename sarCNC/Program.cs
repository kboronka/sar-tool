/* Copyright (C) 2016 Kevin Boronka
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
using System.ServiceProcess;
using System.Threading;

using Base = sar.Base;
using sar.Tools;

namespace sar.CNC
{
	internal sealed class Program : Base.Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(LogUnhandledException);
			
			Logger.LogInfo();
			Logger.LogToConsole = true;
			
			try
			{
				if (!System.Environment.UserInteractive)
				{
					// TODO: run as a service
					ServiceBase.Run(new ServiceBase[] { new Service() });
				}
				else
				{
					try
					{
						var hub = new CommandHub();
						Progress.Start();
						ConsoleHelper.ApplicationShortTitle();
						
						#if DEBUG
						Engine.Start();
						#endif
						
						hub.ProcessCommands(args);
					}
					catch (Exception ex)
					{
						ConsoleHelper.WriteException(ex);
					}
					
					Progress.Stop();
			
					Engine.Stop();
					while (!Engine.Stopped)
					{
						Thread.Sleep(100);
					}
						
					return;
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
			}
		}
	}
}
