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

using sar.Tools;
using sar.Socket;

namespace sar.Controls
{
	partial class SocketServerControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.ActiveConnections = new System.Windows.Forms.Label();
			this.socketMemCacheList1 = new sar.Controls.SocketMemCacheList();
			this.SuspendLayout();
			// 
			// ActiveConnections
			// 
			this.ActiveConnections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.ActiveConnections.ForeColor = System.Drawing.SystemColors.Control;
			this.ActiveConnections.Location = new System.Drawing.Point(0, 0);
			this.ActiveConnections.Name = "ActiveConnections";
			this.ActiveConnections.Size = new System.Drawing.Size(394, 16);
			this.ActiveConnections.TabIndex = 1;
			this.ActiveConnections.Text = "Active Connections: 0";
			// 
			// socketMemCacheList1
			// 
			this.socketMemCacheList1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.socketMemCacheList1.Client = null;
			this.socketMemCacheList1.FullRowSelect = true;
			this.socketMemCacheList1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.socketMemCacheList1.Location = new System.Drawing.Point(0, 19);
			this.socketMemCacheList1.Name = "socketMemCacheList1";
			this.socketMemCacheList1.Server = null;
			this.socketMemCacheList1.Size = new System.Drawing.Size(394, 237);
			this.socketMemCacheList1.TabIndex = 11;
			this.socketMemCacheList1.UseCompatibleStateImageBehavior = false;
			this.socketMemCacheList1.View = System.Windows.Forms.View.Details;
			// 
			// SocketServerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.Controls.Add(this.socketMemCacheList1);
			this.Controls.Add(this.ActiveConnections);
			this.Name = "SocketServerControl";
			this.Size = new System.Drawing.Size(394, 256);
			this.ResumeLayout(false);
		}
		private sar.Controls.SocketMemCacheList socketMemCacheList1;
		private System.Windows.Forms.Label ActiveConnections;
	}
}
