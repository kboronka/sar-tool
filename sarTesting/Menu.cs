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
			RegistryKey winLoginKey = Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\CLSID", true);
			winLoginKey.DeleteSubKeyTree("{045be078-f152-4080-8d55-e476dfdb96ac}");
			winLoginKey.DeleteSubKeyTree("{10136-535ccc8c-d9a4-4ffd-8176-943499b8aa02}");
			winLoginKey.DeleteSubKeyTree("{10459-d3d485b5-84d3-4f7b-acec-468ca308fdf0}");
			winLoginKey.DeleteSubKeyTree("{10881-321646ab-ff97-4418-870e-3b4b82a87ec6}");
			winLoginKey.DeleteSubKeyTree("{11253-e501b6c1-9d57-423f-8e9a-87212c8709b1}");
			winLoginKey.DeleteSubKeyTree("{11998-4664861f-11e6-4815-ade9-2da066b870f7}");
			winLoginKey.DeleteSubKeyTree("{12790-77a9a692-51a4-4f28-9c47-97abf3fef1e8}");
			winLoginKey.DeleteSubKeyTree("{13604-65433da5-08d7-4b74-9c56-5723fd4c2a59}");
			winLoginKey.DeleteSubKeyTree("{13860-d8791cb1-8718-42a7-bf44-87a05aff0a78}");
			winLoginKey.DeleteSubKeyTree("{14233-98e06a52-919b-43c5-b4b8-93412f600ac5}");
			winLoginKey.DeleteSubKeyTree("{14605-7ac0d307-3cd4-4687-a3a1-bbf94a19a924}");
			winLoginKey.DeleteSubKeyTree("{165dfd0c-0ca3-44b4-8dfa-8292d10309eb}");
			winLoginKey.DeleteSubKeyTree("{1d353968-741c-4aaa-bec4-3c4ec698d01d}");
			winLoginKey.DeleteSubKeyTree("{22abc7d0-592e-491e-9cb8-27b0fa21a8f5}");
			winLoginKey.DeleteSubKeyTree("{68b348b7-ad29-4377-927f-d5f5aa43740e}");
			winLoginKey.DeleteSubKeyTree("{79103161-f752-409c-b189-f3be3b2bea5c}");
			winLoginKey.DeleteSubKeyTree("{795d43c8-d204-4a05-8e78-ccd51e9c1d98}");
			winLoginKey.DeleteSubKeyTree("{801c08d1-cd91-486f-a44d-308cf5a69e3b}");
			winLoginKey.DeleteSubKeyTree("{82ede14c-7611-4be4-b771-2c4dcaccac21}");
			winLoginKey.DeleteSubKeyTree("{9764-50d0022a-2fed-43db-9dcf-b0da976b3687}");
			winLoginKey.DeleteSubKeyTree("{b4e0bfab-d0bf-4c3e-acdb-b7acc709d48a}");
			winLoginKey.DeleteSubKeyTree("{eb3bdbe6-7bc2-471d-915f-e31f7908948c}");

			if (winLoginKey == null) throw new KeyNotFoundException("Winlogin key was not found");
			winLoginKey.Close();

		}
	}
}
