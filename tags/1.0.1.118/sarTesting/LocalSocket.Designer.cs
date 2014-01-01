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

namespace sar.Testing
{
	partial class LocalSocket
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
			if (disposing)
			{
				if (components != null) {
					components.Dispose();
				}
				
				if (this.server != null)
				{
					this.server.Shutdown();
					server = null;
					this.socketServerControl1.Server = null;
				}
				
				if (this.client1 != null)
				{
					this.client1.Disconnect();
					this.client1 = null;
				}
				
				if (this.client2 != null)
				{
					this.client2.Disconnect();
					this.client2 = null;
				}
				
				if (this.client3 != null)
				{
					this.client3.Disconnect();
					this.client3 = null;
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
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.socketServerControl1 = new sar.Controls.SocketServerControl();
			this.Connect3 = new System.Windows.Forms.Button();
			this.Host = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.Port = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.Set = new System.Windows.Forms.Button();
			this.TestMember = new System.Windows.Forms.TextBox();
			this.Client1Member = new System.Windows.Forms.Label();
			this.Client2Member = new System.Windows.Forms.Label();
			this.Get = new System.Windows.Forms.Button();
			this.Set_C1 = new System.Windows.Forms.Button();
			this.Set_C2 = new System.Windows.Forms.Button();
			this.socketMemCacheList1 = new sar.Controls.SocketMemCacheList();
			this.socketMemCacheList2 = new sar.Controls.SocketMemCacheList();
			this.StopServer = new System.Windows.Forms.Button();
			this.StartServer = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.Port)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(630, 325);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "connnect 1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(630, 348);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 3;
			this.button2.Text = "connnect 2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.Button2Click);
			// 
			// socketServerControl1
			// 
			this.socketServerControl1.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.socketServerControl1.Location = new System.Drawing.Point(12, 12);
			this.socketServerControl1.Name = "socketServerControl1";
			this.socketServerControl1.Server = null;
			this.socketServerControl1.Size = new System.Drawing.Size(381, 306);
			this.socketServerControl1.TabIndex = 4;
			// 
			// Connect3
			// 
			this.Connect3.Location = new System.Drawing.Point(597, 420);
			this.Connect3.Name = "Connect3";
			this.Connect3.Size = new System.Drawing.Size(75, 23);
			this.Connect3.TabIndex = 5;
			this.Connect3.Text = "connnect";
			this.Connect3.UseVisualStyleBackColor = true;
			this.Connect3.Click += new System.EventHandler(this.Connect3Click);
			// 
			// Host
			// 
			this.Host.Location = new System.Drawing.Point(410, 420);
			this.Host.Name = "Host";
			this.Host.Size = new System.Drawing.Size(100, 20);
			this.Host.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(410, 405);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 12);
			this.label2.TabIndex = 7;
			this.label2.Text = "host";
			// 
			// Port
			// 
			this.Port.Location = new System.Drawing.Point(529, 420);
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
			this.Port.TabIndex = 8;
			this.Port.Value = new decimal(new int[] {
			                              	8100,
			                              	0,
			                              	0,
			                              	0});
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(529, 405);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 12);
			this.label3.TabIndex = 9;
			this.label3.Text = "port";
			// 
			// Set
			// 
			this.Set.Location = new System.Drawing.Point(846, 325);
			this.Set.Name = "Set";
			this.Set.Size = new System.Drawing.Size(75, 25);
			this.Set.TabIndex = 10;
			this.Set.Text = "Set All";
			this.Set.UseVisualStyleBackColor = true;
			this.Set.Click += new System.EventHandler(this.SetClick);
			// 
			// TestMember
			// 
			this.TestMember.Location = new System.Drawing.Point(711, 325);
			this.TestMember.Name = "TestMember";
			this.TestMember.Size = new System.Drawing.Size(129, 20);
			this.TestMember.TabIndex = 11;
			// 
			// Client1Member
			// 
			this.Client1Member.Location = new System.Drawing.Point(410, 330);
			this.Client1Member.Name = "Client1Member";
			this.Client1Member.Size = new System.Drawing.Size(149, 16);
			this.Client1Member.TabIndex = 12;
			this.Client1Member.Text = "Client 1: ";
			// 
			// Client2Member
			// 
			this.Client2Member.Location = new System.Drawing.Point(410, 346);
			this.Client2Member.Name = "Client2Member";
			this.Client2Member.Size = new System.Drawing.Size(149, 16);
			this.Client2Member.TabIndex = 13;
			this.Client2Member.Text = "Client 2: ";
			// 
			// Get
			// 
			this.Get.Location = new System.Drawing.Point(410, 365);
			this.Get.Name = "Get";
			this.Get.Size = new System.Drawing.Size(75, 23);
			this.Get.TabIndex = 14;
			this.Get.Text = "Get";
			this.Get.UseVisualStyleBackColor = true;
			this.Get.Click += new System.EventHandler(this.GetClick);
			// 
			// Set_C1
			// 
			this.Set_C1.Location = new System.Drawing.Point(927, 325);
			this.Set_C1.Name = "Set_C1";
			this.Set_C1.Size = new System.Drawing.Size(75, 23);
			this.Set_C1.TabIndex = 15;
			this.Set_C1.Text = "Set Client 1";
			this.Set_C1.UseVisualStyleBackColor = true;
			this.Set_C1.Click += new System.EventHandler(this.Set_C1Click);
			// 
			// Set_C2
			// 
			this.Set_C2.Location = new System.Drawing.Point(927, 353);
			this.Set_C2.Name = "Set_C2";
			this.Set_C2.Size = new System.Drawing.Size(75, 23);
			this.Set_C2.TabIndex = 16;
			this.Set_C2.Text = "Set Client 2";
			this.Set_C2.UseVisualStyleBackColor = true;
			this.Set_C2.Click += new System.EventHandler(this.Set_C2Click);
			// 
			// socketMemCacheList1
			// 
			this.socketMemCacheList1.Client = null;
			this.socketMemCacheList1.FullRowSelect = true;
			this.socketMemCacheList1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.socketMemCacheList1.Location = new System.Drawing.Point(410, 13);
			this.socketMemCacheList1.Name = "socketMemCacheList1";
			this.socketMemCacheList1.Server = null;
			this.socketMemCacheList1.Size = new System.Drawing.Size(295, 306);
			this.socketMemCacheList1.TabIndex = 17;
			this.socketMemCacheList1.UseCompatibleStateImageBehavior = false;
			this.socketMemCacheList1.View = System.Windows.Forms.View.Details;
			// 
			// socketMemCacheList2
			// 
			this.socketMemCacheList2.Client = null;
			this.socketMemCacheList2.FullRowSelect = true;
			this.socketMemCacheList2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.socketMemCacheList2.Location = new System.Drawing.Point(711, 13);
			this.socketMemCacheList2.Name = "socketMemCacheList2";
			this.socketMemCacheList2.Server = null;
			this.socketMemCacheList2.Size = new System.Drawing.Size(295, 306);
			this.socketMemCacheList2.TabIndex = 18;
			this.socketMemCacheList2.UseCompatibleStateImageBehavior = false;
			this.socketMemCacheList2.View = System.Windows.Forms.View.Details;
			// 
			// StopServer
			// 
			this.StopServer.Location = new System.Drawing.Point(108, 322);
			this.StopServer.Name = "StopServer";
			this.StopServer.Size = new System.Drawing.Size(75, 23);
			this.StopServer.TabIndex = 19;
			this.StopServer.Text = "Stop";
			this.StopServer.UseVisualStyleBackColor = true;
			this.StopServer.Click += new System.EventHandler(this.StopServerClick);
			// 
			// StartServer
			// 
			this.StartServer.Location = new System.Drawing.Point(12, 322);
			this.StartServer.Name = "StartServer";
			this.StartServer.Size = new System.Drawing.Size(75, 23);
			this.StartServer.TabIndex = 20;
			this.StartServer.Text = "Start";
			this.StartServer.UseVisualStyleBackColor = true;
			this.StartServer.Click += new System.EventHandler(this.StartServerClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1011, 467);
			this.Controls.Add(this.StartServer);
			this.Controls.Add(this.StopServer);
			this.Controls.Add(this.socketMemCacheList2);
			this.Controls.Add(this.socketMemCacheList1);
			this.Controls.Add(this.Set_C2);
			this.Controls.Add(this.Set_C1);
			this.Controls.Add(this.Get);
			this.Controls.Add(this.Client2Member);
			this.Controls.Add(this.Client1Member);
			this.Controls.Add(this.TestMember);
			this.Controls.Add(this.Set);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.Port);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.Host);
			this.Controls.Add(this.Connect3);
			this.Controls.Add(this.socketServerControl1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Name = "MainForm";
			this.Text = "sar.Testing";
			((System.ComponentModel.ISupportInitialize)(this.Port)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button StartServer;
		private System.Windows.Forms.Button StopServer;
		private sar.Controls.SocketMemCacheList socketMemCacheList2;
		private sar.Controls.SocketMemCacheList socketMemCacheList1;
		private System.Windows.Forms.Button Set_C2;
		private System.Windows.Forms.Button Set_C1;
		private System.Windows.Forms.Button Get;
		private System.Windows.Forms.Label Client2Member;
		private System.Windows.Forms.Label Client1Member;
		private System.Windows.Forms.TextBox TestMember;
		private System.Windows.Forms.Button Set;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown Port;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox Host;
		private System.Windows.Forms.Button Connect3;
		private sar.Controls.SocketServerControl socketServerControl1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
	}
}
