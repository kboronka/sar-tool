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
			this.ClientID = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// connected
			// 
			this.connected.Caption = "Connected";
			this.connected.Location = new System.Drawing.Point(5, 5);
			this.connected.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.connected.MaximumSize = new System.Drawing.Size(583, 20);
			this.connected.MinimumSize = new System.Drawing.Size(117, 20);
			this.connected.Name = "connected";
			this.connected.Size = new System.Drawing.Size(117, 20);
			this.connected.Status = false;
			this.connected.TabIndex = 0;
			// 
			// History
			// 
			this.History.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.History.FormattingEnabled = true;
			this.History.ItemHeight = 16;
			this.History.Location = new System.Drawing.Point(5, 34);
			this.History.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.History.Name = "History";
			this.History.Size = new System.Drawing.Size(507, 340);
			this.History.TabIndex = 1;
			// 
			// Message
			// 
			this.Message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.Message.Location = new System.Drawing.Point(5, 384);
			this.Message.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Message.Name = "Message";
			this.Message.Size = new System.Drawing.Size(424, 22);
			this.Message.TabIndex = 2;
			// 
			// Send
			// 
			this.Send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Send.Location = new System.Drawing.Point(437, 384);
			this.Send.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Send.Name = "Send";
			this.Send.Size = new System.Drawing.Size(75, 28);
			this.Send.TabIndex = 3;
			this.Send.Text = "Send";
			this.Send.UseVisualStyleBackColor = true;
			this.Send.Click += new System.EventHandler(this.SendClick);
			// 
			// DissconnectPB
			// 
			this.DissconnectPB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.DissconnectPB.Location = new System.Drawing.Point(418, 4);
			this.DissconnectPB.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.DissconnectPB.Name = "DissconnectPB";
			this.DissconnectPB.Size = new System.Drawing.Size(94, 28);
			this.DissconnectPB.TabIndex = 3;
			this.DissconnectPB.Text = "Dissconnect";
			this.DissconnectPB.UseVisualStyleBackColor = true;
			this.DissconnectPB.Click += new System.EventHandler(this.DissconnectPBClick);
			// 
			// ConnectPB
			// 
			this.ConnectPB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ConnectPB.Location = new System.Drawing.Point(316, 4);
			this.ConnectPB.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.ConnectPB.Name = "ConnectPB";
			this.ConnectPB.Size = new System.Drawing.Size(94, 28);
			this.ConnectPB.TabIndex = 4;
			this.ConnectPB.Text = "Connect";
			this.ConnectPB.UseVisualStyleBackColor = true;
			this.ConnectPB.Click += new System.EventHandler(this.ConnectPBClick);
			// 
			// ClientID
			// 
			this.ClientID.AutoSize = true;
			this.ClientID.Location = new System.Drawing.Point(128, 5);
			this.ClientID.Name = "ClientID";
			this.ClientID.Size = new System.Drawing.Size(61, 16);
			this.ClientID.TabIndex = 5;
			this.ClientID.Text = "ClientID: ";
			// 
			// SocketClientControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this.Controls.Add(this.ClientID);
			this.Controls.Add(this.ConnectPB);
			this.Controls.Add(this.DissconnectPB);
			this.Controls.Add(this.Send);
			this.Controls.Add(this.Message);
			this.Controls.Add(this.History);
			this.Controls.Add(this.connected);
			this.Font = new System.Drawing.Font("Arial", 9.75F);
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "SocketClientControl";
			this.Size = new System.Drawing.Size(516, 423);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label ClientID;
		private System.Windows.Forms.Button ConnectPB;
		private System.Windows.Forms.Button DissconnectPB;
		private System.Windows.Forms.Button Send;
		private System.Windows.Forms.TextBox Message;
		private System.Windows.Forms.ListBox History;
		private sar.Controls.BooleanIndicator connected;
	}
}
