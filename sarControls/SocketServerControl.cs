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
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

using sar.Tools;
using sar.Socket;

namespace sar.Controls
{
	public partial class SocketServerControl : UserControl
	{
		private SocketServer server;

		#region properties
		
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SocketServer Server
		{
			get { return this.server; }
			set
			{
				try
				{
					if (this.server != null)
					{
						this.server.NewClient -= new EventHandler(this.ClientsChanged);
						this.server.ClientLost -= new EventHandler(this.ClientsChanged);
						socketMemCacheList1.Server = null;
					}
					
					this.server = value;
					
					if (this.server != null)
					{
						this.server.NewClient += new EventHandler(this.ClientsChanged);
						this.server.ClientLost += new EventHandler(this.ClientsChanged);
						socketMemCacheList1.Server = this.server;
					}
				}
				catch
				{
					
				}
			}
		}
		
		public SocketServerControl()
		{
			InitializeComponent();
		}

		#endregion

		private void ClientsChanged(object sender, EventArgs e)
		{
			this.Invoke((MethodInvoker) delegate
			            {
			            	this.ActiveConnections.Text = "Active Connections: " + this.server.Clients.ToString();
			            });
		}
	}
}
