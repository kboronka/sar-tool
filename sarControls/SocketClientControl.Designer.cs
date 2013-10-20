/* Copyright (C) 2013 Kevin Boronka
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
	partial class SocketClientControl
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
			this.connected = new sar.Controls.BooleanIndicator();
			this.History = new System.Windows.Forms.ListBox();
			this.Message = new System.Windows.Forms.TextBox();
			this.Send = new System.Windows.Forms.Button();
			this.DissconnectPB = new System.Windows.Forms.Button();
			this.ConnectPB = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// connected
			// 
			this.connected.Caption = "Connected";
			this.connected.Font = new System.Drawing.Font("Arial", 9.75F);
			this.connected.Location = new System.Drawing.Point(4, 4);
			this.connected.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.connected.MaximumSize = new System.Drawing.Size(500, 16);
			this.connected.MinimumSize = new System.Drawing.Size(100, 16);
			this.connected.Name = "connected";
			this.connected.Size = new System.Drawing.Size(100, 16);
			this.connected.Status = false;
			this.connected.TabIndex = 0;
			// 
			// History
			// 
			this.History.FormattingEnabled = true;
			this.History.Location = new System.Drawing.Point(4, 28);
			this.History.Name = "History";
			this.History.Size = new System.Drawing.Size(435, 277);
			this.History.TabIndex = 1;
			// 
			// Message
			// 
			this.Message.Location = new System.Drawing.Point(4, 312);
			this.Message.Name = "Message";
			this.Message.Size = new System.Drawing.Size(364, 20);
			this.Message.TabIndex = 2;
			// 
			// Send
			// 
			this.Send.Location = new System.Drawing.Point(375, 312);
			this.Send.Name = "Send";
			this.Send.Size = new System.Drawing.Size(64, 23);
			this.Send.TabIndex = 3;
			this.Send.Text = "Send";
			this.Send.UseVisualStyleBackColor = true;
			this.Send.Click += new System.EventHandler(this.SendClick);
			// 
			// DissconnectPB
			// 
			this.DissconnectPB.Location = new System.Drawing.Point(358, 3);
			this.DissconnectPB.Name = "DissconnectPB";
			this.DissconnectPB.Size = new System.Drawing.Size(81, 23);
			this.DissconnectPB.TabIndex = 3;
			this.DissconnectPB.Text = "Dissconnect";
			this.DissconnectPB.UseVisualStyleBackColor = true;
			this.DissconnectPB.Click += new System.EventHandler(this.DissconnectPBClick);
			// 
			// ConnectPB
			// 
			this.ConnectPB.Location = new System.Drawing.Point(271, 3);
			this.ConnectPB.Name = "ConnectPB";
			this.ConnectPB.Size = new System.Drawing.Size(81, 23);
			this.ConnectPB.TabIndex = 4;
			this.ConnectPB.Text = "Connect";
			this.ConnectPB.UseVisualStyleBackColor = true;
			this.ConnectPB.Click += new System.EventHandler(this.ConnectPBClick);
			// 
			// SocketClientControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ConnectPB);
			this.Controls.Add(this.DissconnectPB);
			this.Controls.Add(this.Send);
			this.Controls.Add(this.Message);
			this.Controls.Add(this.History);
			this.Controls.Add(this.connected);
			this.Name = "SocketClientControl";
			this.Size = new System.Drawing.Size(442, 344);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button ConnectPB;
		private System.Windows.Forms.Button DissconnectPB;
		private System.Windows.Forms.Button Send;
		private System.Windows.Forms.TextBox Message;
		private System.Windows.Forms.ListBox History;
		private sar.Controls.BooleanIndicator connected;
	}
}
