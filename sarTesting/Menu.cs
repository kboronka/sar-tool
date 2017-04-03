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
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using S7 = sar.S7Siemens;
using sar.Socket;
using sar.Tools;
using sar.Timing;

namespace sar.Testing
{
	public partial class Menu : Form
	{
		private SocketServer socketServer;
		private Thread loop;
		private bool shutdown = false;
		
		public Menu()
		{
			InitializeComponent();
			
			socketServer = new SocketServer(911);
			
			this.Text = "sar-tool testing:" + Program.port.ToString();
			
			foreach (var file in Program.Server.Cache.Files)
			{
				System.Diagnostics.Debug.WriteLine(file);
			}
			
			try
			{
				throw new ApplicationException("test");
			}
			catch (Exception ex)
			{
				textBox3.Text = ExceptionHelper.GetStackTrace(ex);
			}
			
			
			this.loop = new Thread(this.TestLoop);
			this.loop.IsBackground = true;
			this.loop.Start();
		}
		
		~Menu()
		{
			this.shutdown = true;
			this.loop.Join();
		}
		
		
		private void TestLoop()
		{
			var logTrigger = new Interval(1000, 5000);
			var timeout = new Interval(30000);
			while (!shutdown)
			{
				//if (logTrigger.Ready) Program.Log("log: " + counter++.ToString());
				//if (timeout.Ready) Program.Log("timeout: " + timeout.ElapsedMilliseconds.ToString());
				Thread.Sleep(670);
			}
		}
		
		
		void Button1Click(object sender, EventArgs e)
		{
			try
			{
				throw new Exception("test");
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
				Tools.ExceptionHandler.Display(ex);
			}
		}
		
		void ConnectToSPSClick(object sender, EventArgs e)
		{
			try
			{
				var siemensS7 = new S7.Adapter("10.240.26.54");
				byte[] data = siemensS7.ReadBytes("DB250.DBB164", 86);
				var stringValue = siemensS7.ReadString("DB250.DBB164", 86);
				textBox3.Text = StringHelper.ArrayToString("data", data);
				textBox3.Text += "value: " + stringValue;
				
				siemensS7.Dispose();
			}
			catch (Exception ex)
			{
				sar.Tools.ExceptionHandler.Display(ex);
			}
		}
		
		void Button4Click(object sender, EventArgs e)
		{
			foreach (string resource in EmbeddedResource.GetAllResources())
			{
				System.Diagnostics.Debug.WriteLine(resource);
			}
		}
		
		void Button5Click(object sender, EventArgs e)
		{
			using (var plc = new S7.Adapter("192.168.1.3"))
			{
				plc.WriteBytes("DB300.DBB0", new byte[] { 0x01 });
			}
		}
		
		void Button6Click(object sender, EventArgs e)
		{
			var data = @"\tC:\\test\\ \n";

			data = Regex.Replace(data, @"([^\\]|^)([\\][n])", m => m.Groups[1].Value + "\n");
			data = Regex.Replace(data, @"([^\\]|^)([\\][t])", m => m.Groups[1].Value + "\t");
			data = Regex.Replace(data, @"([^\\]|^)([\\][\\])", m => m.Groups[1].Value + "\\");

			System.Diagnostics.Debug.WriteLine(data);
			
			
			var json = new Dictionary<string, object>();
			json.Add("test", @"c:\test\ 102/103 ""abc efg""");
			
			var jsonString = json.ToJson();
			
			var decodeJsonString = jsonString.JsonGetKeyValue("test", "abc");
			json = new Dictionary<string, object>();
			json.Add("test", decodeJsonString);
			
			var jsonString2 = json.ToJson();
			
			System.Diagnostics.Debug.WriteLine(jsonString);
			System.Diagnostics.Debug.WriteLine(jsonString2);
		}
		
		void MakeSocketClick(object sender, EventArgs e)
		{
			for (var x=0;x<1000; x++)
			{
				for (var i = 0; i<100; i++)
				{
					using(var client = new SocketClient("127.0.0.1", 911))
					{
						client.SetValue("sarTesting", AssemblyInfo.Version, true);
						
						Thread.Sleep(250);
					}
				}
				
				System.GC.Collect();
				Thread.Sleep(1250);
			}
		}
		
		void JogForwardClick(object sender, EventArgs e)
		{
			sar_testing.Http.TestWebSocket.JogForward();
		}
		
		void Button8Click(object sender, EventArgs e)
		{
			//{"command":"manual-entry-submit","param1":[{"id":17,"dataType":1,"description":"Test Var1","value":"34.1","$$hashKey":"00I","passFailId":1},{"id":18,"dataType":1,"description":"Test Var2","value":"33.1","$$hashKey":"00J","passFailId":2},{"id":19,"dataType":1,"description":"Test Var3","value":"3.5","$$hashKey":"00K","passFailId":1}],"param2":-1,"param3":""}
			var json = "{\"command\":\"test\",\"param1\":[{\"id\":17,\"dataType\":1,\"description\":\"Test Var1\",\"value\":\"34.1\",\"$$hashKey\":\"00I\",\"passFailId\":1},{\"id\":18,\"dataType\":1,\"description\":\"Test Var2\",\"value\":\"33.1\",\"$$hashKey\":\"00J\",\"passFailId\":2},{\"id\":19,\"dataType\":1,\"description\":\"Test Var3\",\"value\":\"3.5\",\"$$hashKey\":\"00K\",\"passFailId\":1}],"+
				"\"param2\":-1," + 
				"\"param3\":true," + 
				"\"param4\":\"\"}";
			
			var kvp = json.JsonToKeyValuePairs();
			var param4 = json.JsonGetKeyValue("param4", "na");
			var param3 = json.JsonGetKeyValue("param3", false);
			var param2 = json.JsonGetKeyValue("param2", 1);
			//var param1 = json.JsonGetKeyValue("param1", new List<Dictionary<string, object>>());
		}
	}
}
