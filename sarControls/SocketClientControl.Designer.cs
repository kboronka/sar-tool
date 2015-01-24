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
			this.CustomMessage = new System.Windows.Forms.TextBox();
			this.Send = new System.Windows.Forms.Button();
			this.ClientID = new System.Windows.Forms.Label();
			this.PacketsIn = new System.Windows.Forms.Label();
			this.PacketsOut = new System.Windows.Forms.Label();
			this.MessageList = new System.Windows.Forms.ListView();
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
			// CustomMessage
			// 
			this.CustomMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.CustomMessage.Location = new System.Drawing.Point(0, 350);
			this.CustomMessage.Name = "CustomMessage";
			this.CustomMessage.Size = new System.Drawing.Size(395, 20);
			this.CustomMessage.TabIndex = 2;
			// 
			// Send
			// 
			this.Send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Send.Location = new System.Drawing.Point(401, 348);
			this.Send.Name = "Send";
			this.Send.Size = new System.Drawing.Size(64, 23);
			this.Send.TabIndex = 3;
			this.Send.Text = "Send";
			this.Send.UseVisualStyleBackColor = true;
			this.Send.Click += new System.EventHandler(this.SendClick);
			// 
			// ClientID
			// 
			this.ClientID.AutoSize = true;
			this.ClientID.Location = new System.Drawing.Point(4, 20);
			this.ClientID.Name = "ClientID";
			this.ClientID.Size = new System.Drawing.Size(50, 13);
			this.ClientID.TabIndex = 5;
			this.ClientID.Text = "ClientID: ";
			// 
			// PacketsIn
			// 
			this.PacketsIn.AutoSize = true;
			this.PacketsIn.Location = new System.Drawing.Point(118, 6);
			this.PacketsIn.Name = "PacketsIn";
			this.PacketsIn.Size = new System.Drawing.Size(22, 13);
			this.PacketsIn.TabIndex = 6;
			this.PacketsIn.Text = "In: ";
			// 
			// PacketsOut
			// 
			this.PacketsOut.AutoSize = true;
			this.PacketsOut.Location = new System.Drawing.Point(110, 20);
			this.PacketsOut.Name = "PacketsOut";
			this.PacketsOut.Size = new System.Drawing.Size(30, 13);
			this.PacketsOut.TabIndex = 7;
			this.PacketsOut.Text = "Out: ";
			// 
			// MessageList
			// 
			this.MessageList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.MessageList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.MessageList.Location = new System.Drawing.Point(0, 37);
			this.MessageList.Name = "MessageList";
			this.MessageList.ShowGroups = false;
			this.MessageList.Size = new System.Drawing.Size(465, 307);
			this.MessageList.TabIndex = 9;
			this.MessageList.UseCompatibleStateImageBehavior = false;
			this.MessageList.View = System.Windows.Forms.View.Details;
			// 
			// SocketClientControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this.Controls.Add(this.MessageList);
			this.Controls.Add(this.PacketsOut);
			this.Controls.Add(this.PacketsIn);
			this.Controls.Add(this.ClientID);
			this.Controls.Add(this.Send);
			this.Controls.Add(this.CustomMessage);
			this.Controls.Add(this.connected);
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "SocketClientControl";
			this.Size = new System.Drawing.Size(465, 370);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ListView MessageList;
		private System.Windows.Forms.Label PacketsOut;
		private System.Windows.Forms.Label PacketsIn;
		private System.Windows.Forms.Label ClientID;
		private System.Windows.Forms.Button Send;
		private System.Windows.Forms.TextBox CustomMessage;
		private sar.Controls.BooleanIndicator connected;
	}
}
