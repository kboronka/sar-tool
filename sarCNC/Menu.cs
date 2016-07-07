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
using System.Text;
using System.Threading;
using System.Windows.Forms;

using S7 = sar.S7Siemens;
using sar.Socket;
using sar.Tools;
using sar.Timing;
using sar.Http;

namespace sar.CNC
{
	public partial class Menu : Form
	{
		private SocketServer socketServer;
		
		public Menu()
		{
			InitializeComponent();
			
			socketServer = new SocketServer(911, Program.ErrorLog, Program.DebugLog);
			
			this.Text = "server:" + Program.port.ToString();
			
			HttpWebSocket.NewClient += new HttpWebSocket.NewConnectionHandler(this.NewClient);
		}
		
		~Menu()
		{

		}
		
		delegate void AddTextCallback(string text);
		
		private void AddText(string text)
		{
		  // InvokeRequired required compares the thread ID of the
		  // calling thread to the thread ID of the creating thread.
		  // If these threads are different, it returns true.
		  if (this.txtCommLog.InvokeRequired)
		  { 
		    var d = new AddTextCallback(AddText);
		    this.Invoke(d, new object[] { text });
		  }
		  else
		  {
		    this.txtCommLog.Text = text + Environment.NewLine + this.txtCommLog.Text;
		  }
		}

		private void NewClient(HttpWebSocket client)
		{
			AddText("Client Connected");
			client.FrameRecived += new HttpWebSocket.FrameRecivedHandler(this.NewFrame);
		}
		
		private void NewFrame(HttpWebSocketFrame frame)
		{
			AddText(System.Text.Encoding.ASCII.GetString(frame.Payload));
		}
		
		void JogForwardClick(object sender, EventArgs e)
		{
			sar.CNC.Http.TestWebSocket.JogForward();
		}
	}
}
