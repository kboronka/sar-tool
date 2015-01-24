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
using System.Drawing;
using System.Windows.Forms;

using sar.Socket;
using sar.Controls;

namespace sar.Testing
{
	public partial class SocketClientForm : Form
	{
		private SocketClient client;
		public SocketClient Client
		{
			set
			{
				this.socketClientControl1.Client = value;
				this.client = value;
			}
		}
		
		public SocketClientForm()
		{
			InitializeComponent();
		}
		
		void SocketClientFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.client != null && this.client.Connected)
			{
				if (MessageBox.Show("Do you want to close the client?", "sarTesting", MessageBoxButtons.YesNo) == DialogResult.No)
				{
					e.Cancel = true;
				}
				
				this.client.Disconnect();
			}
		}
	}
}
