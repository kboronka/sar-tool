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
using System.Collections.Generic;
using System.Windows.Forms;

using Base=sar.Base;
using sar.Tools;
using sar.Http;

using sar.CNC.Http;

namespace sar.CNC
{
	internal sealed class Program : Base.Program
	{
		private static int port = 0;
		public static HttpServer Server;
		public static GrblPort Port;
		
		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(LogUnhandledException);
			
			Base.Program.LogInfo();
			
			#if DEBUG
			var server = new HttpServer(ApplicationInfo.CurrentDirectory + @"..\..\Http\Views\");
			#else
			var server = new HttpServer(ApplicationInfo.CurrentDirectory + @"views\");
			#endif
			Server = server;
			
			port = server.Port;
			
			Port = new GrblPort("COM3");
			Port.ResponceRecived += new GrblPort.ResponceRecivedHandler(Program.LogResponce);
			
//			foreach (string resource in EmbeddedResource.GetAllResources())
//			{
//				System.Diagnostics.Debug.WriteLine(resource);
//			}
			
			string root = IO.CheckRoot(@".\");
			root = IO.CheckRoot(@"..\");
			root = IO.CheckRoot(@"..\..\");
			root = IO.CheckRoot(@"\");
				
			HttpWebSocket.ClientConnected += new HttpWebSocket.ConnectedClientHandler(AddClient);
			HttpWebSocket.ClientDisconnected += new HttpWebSocket.ClientDisconnectedHandler(DropClient);
		
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Menu());
			
			
			Server.Stop();
			System.Threading.Thread.Sleep(500);
			//throw new ApplicationException("testing unhandled exception");
			// Program.Log("shutting down");
			// Application.Exit();
		}
		
		private static List<HttpWebSocket> clients = new List<HttpWebSocket>();

		private static void AddClient(HttpWebSocket client)
		{
			Program.Log("Client " + client.ID.ToString() + " Connected");
			clients.Add(client);
		}
		
		private static void DropClient(HttpWebSocket client)
		{
			Program.Log("Client " + client.ID.ToString() + " Disconnected");
			clients.Remove(client);
		}
		
		public static void LogRaw(string raw)
		{
			foreach (var client in clients)
			{
				client.SendString(raw);
			}
		}
		
		public static void LogCommand(string message)
		{
			foreach (var client in clients)
			{
				client.SendString(client.ID.ToString() + "<< " + message);
			}
		}
		
		public static void LogResponce(string message)
		{
			foreach (var client in clients)
			{
				if (message == "ok")
				{
					client.SendString(client.ID.ToString() + ">> " + message);	
				}
				else
				{
					client.SendString(">> " + message);	
				}
			}
		}		
	}
}
