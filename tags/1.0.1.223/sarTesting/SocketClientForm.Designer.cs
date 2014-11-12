/* Copyright (C) 2014 Kevin Boronka
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

using sar.Socket;
using sar.Controls;

namespace sar.Testing
{
	partial class SocketClientForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
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
			this.socketClientControl1 = new sar.Controls.SocketClientControl();
			this.SuspendLayout();
			// 
			// socketClientControl1
			// 
			this.socketClientControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.socketClientControl1.Client = null;
			this.socketClientControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.socketClientControl1.Location = new System.Drawing.Point(0, 0);
			this.socketClientControl1.Name = "socketClientControl1";
			this.socketClientControl1.Size = new System.Drawing.Size(452, 348);
			this.socketClientControl1.TabIndex = 0;
			// 
			// SocketClientForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(452, 348);
			this.Controls.Add(this.socketClientControl1);
			this.Name = "SocketClientForm";
			this.Text = "SocketClientForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SocketClientFormFormClosing);
			this.ResumeLayout(false);
		}
		private sar.Controls.SocketClientControl socketClientControl1;
	}
}
