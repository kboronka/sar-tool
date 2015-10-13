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
using System.Windows.Forms;
using System.Timers;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

using S7 = sar.S7Siemens;
using sar.Tools;
using sar.Http;

namespace sar.Testing
{
	public partial class Menu : Form
	{
		public Menu()
		{
			InitializeComponent();
			
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
		}
		
		
		void Button1Click(object sender, EventArgs e)
		{
			try
			{
				throw new Exception("test");
			}
			catch (Exception ex)
			{
				Program.Log(ex);
				Tools.ExceptionHandler.Display(ex);
			}
		}
		
		void ConnectToSPSClick(object sender, EventArgs e)
		{
			try
			{
				var siemensS7 = new S7.Adapter("10.242.217.122");
				byte[] data = siemensS7.ReadBytes("MW6600", 220);
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
			
			var jsonString = json.ToJSON();
			
			var decodeJsonString = jsonString.GetJSON("test", "abc");
			json = new Dictionary<string, object>();
			json.Add("test", decodeJsonString);
			
			var jsonString2 = json.ToJSON();
			
			System.Diagnostics.Debug.WriteLine(jsonString);
			System.Diagnostics.Debug.WriteLine(jsonString2);
		}
		
		void Button7Click(object sender, EventArgs e)
		{

		}
	}
}
