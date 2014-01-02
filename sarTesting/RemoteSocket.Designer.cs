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

namespace sar.Testing
{
	partial class RemoteSocket
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
			this.label3 = new System.Windows.Forms.Label();
			this.Port = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.Host = new System.Windows.Forms.TextBox();
			this.Connect3 = new System.Windows.Forms.Button();
			this.socketMemCacheList1 = new sar.Controls.SocketMemCacheList();
			this.socketClientControl1 = new sar.Controls.SocketClientControl();
			((System.ComponentModel.ISupportInitialize)(this.Port)).BeginInit();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(129, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 12);
			this.label3.TabIndex = 14;
			this.label3.Text = "port";
			// 
			// Port
			// 
			this.Port.Location = new System.Drawing.Point(129, 29);
			this.Port.Maximum = new decimal(new int[] {
									8200,
									0,
									0,
									0});
			this.Port.Minimum = new decimal(new int[] {
									8100,
									0,
									0,
									0});
			this.Port.Name = "Port";
			this.Port.Size = new System.Drawing.Size(62, 20);
			this.Port.TabIndex = 13;
			this.Port.Value = new decimal(new int[] {
									8111,
									0,
									0,
									0});
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 12);
			this.label2.TabIndex = 12;
			this.label2.Text = "host";
			// 
			// Host
			// 
			this.Host.Location = new System.Drawing.Point(10, 29);
			this.Host.Name = "Host";
			this.Host.Size = new System.Drawing.Size(100, 20);
			this.Host.TabIndex = 11;
			this.Host.Text = "10.240.14.8";
			// 
			// Connect3
			// 
			this.Connect3.Location = new System.Drawing.Point(197, 29);
			this.Connect3.Name = "Connect3";
			this.Connect3.Size = new System.Drawing.Size(75, 23);
			this.Connect3.TabIndex = 10;
			this.Connect3.Text = "connnect";
			this.Connect3.UseVisualStyleBackColor = true;
			this.Connect3.Click += new System.EventHandler(this.Connect3Click);
			// 
			// socketMemCacheList1
			// 
			this.socketMemCacheList1.Client = null;
			this.socketMemCacheList1.FullRowSelect = true;
			this.socketMemCacheList1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.socketMemCacheList1.Location = new System.Drawing.Point(10, 58);
			this.socketMemCacheList1.Name = "socketMemCacheList1";
			this.socketMemCacheList1.Server = null;
			this.socketMemCacheList1.Size = new System.Drawing.Size(295, 306);
			this.socketMemCacheList1.TabIndex = 18;
			this.socketMemCacheList1.UseCompatibleStateImageBehavior = false;
			this.socketMemCacheList1.View = System.Windows.Forms.View.Details;
			// 
			// socketClientControl1
			// 
			this.socketClientControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this.socketClientControl1.Client = null;
			this.socketClientControl1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.socketClientControl1.Location = new System.Drawing.Point(312, 58);
			this.socketClientControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.socketClientControl1.Name = "socketClientControl1";
			this.socketClientControl1.Size = new System.Drawing.Size(465, 306);
			this.socketClientControl1.TabIndex = 19;
			// 
			// RemoteSocket
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 377);
			this.Controls.Add(this.socketClientControl1);
			this.Controls.Add(this.socketMemCacheList1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.Port);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.Host);
			this.Controls.Add(this.Connect3);
			this.Name = "RemoteSocket";
			this.Text = "RemoteSocketTesting";
			((System.ComponentModel.ISupportInitialize)(this.Port)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private sar.Controls.SocketClientControl socketClientControl1;
		private sar.Controls.SocketMemCacheList socketMemCacheList1;
		private System.Windows.Forms.Button Connect3;
		private System.Windows.Forms.TextBox Host;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown Port;
		private System.Windows.Forms.Label label3;
	}
}
