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
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Threading;
using System.Windows.Forms;

using sar.Tools;
using sar.Socket;

namespace sar.Testing
{
	public partial class RemoteSocket : Form
	{
		private SocketClient client1;

		public RemoteSocket()
		{
			InitializeComponent();

			client1 = new SocketClient("10.240.14.8", 8111, Program.ErrorLog, Program.DebugLog);
			this.socketMemCacheList1.ErrorLog = Program.ErrorLog;
			this.socketMemCacheList1.DebugLog = Program.DebugLog;
			this.socketMemCacheList1.Client = client1;
			this.socketClientControl1.Client = client1;
			client1.SendData("ping");
		}
		
		void Connect3Click(object sender, EventArgs e)
		{
			
		}
	}
}
