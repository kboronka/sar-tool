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
	partial class Menu
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
			this.LocalSocketButton = new System.Windows.Forms.Button();
			this.RemoteSocketButton = new System.Windows.Forms.Button();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.booleanIndicator1 = new sar.Controls.BooleanIndicator();
			this.button1 = new System.Windows.Forms.Button();
			this.ConnectToSPS = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.readOnlyTextBox1 = new sar.Controls.ReadOnlyTextBox();
			this.button3 = new System.Windows.Forms.Button();
			this.folderSelect1 = new sarControls.FolderSelect();
			this.folderChanged = new sar.Controls.BooleanIndicator();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// LocalSocketButton
			// 
			this.LocalSocketButton.Location = new System.Drawing.Point(13, 13);
			this.LocalSocketButton.Name = "LocalSocketButton";
			this.LocalSocketButton.Size = new System.Drawing.Size(113, 23);
			this.LocalSocketButton.TabIndex = 0;
			this.LocalSocketButton.Text = "Local Socket";
			this.LocalSocketButton.UseVisualStyleBackColor = true;
			this.LocalSocketButton.Click += new System.EventHandler(this.LocalSocketClick);
			// 
			// RemoteSocketButton
			// 
			this.RemoteSocketButton.Location = new System.Drawing.Point(13, 42);
			this.RemoteSocketButton.Name = "RemoteSocketButton";
			this.RemoteSocketButton.Size = new System.Drawing.Size(113, 23);
			this.RemoteSocketButton.TabIndex = 1;
			this.RemoteSocketButton.Text = "Remote Socket";
			this.RemoteSocketButton.UseVisualStyleBackColor = true;
			this.RemoteSocketButton.Click += new System.EventHandler(this.RemoteSocketClick);
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(338, 10);
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
			this.numericUpDown1.TabIndex = 2;
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.IntegralHeight = false;
			this.comboBox1.Location = new System.Drawing.Point(234, 36);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(224, 32);
			this.comboBox1.TabIndex = 3;
			// 
			// booleanIndicator1
			// 
			this.booleanIndicator1.Caption = "booleanIndicator";
			this.booleanIndicator1.Font = new System.Drawing.Font("Arial", 9.75F);
			this.booleanIndicator1.Location = new System.Drawing.Point(56, 72);
			this.booleanIndicator1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.booleanIndicator1.MaximumSize = new System.Drawing.Size(500, 16);
			this.booleanIndicator1.MinimumSize = new System.Drawing.Size(100, 16);
			this.booleanIndicator1.Name = "booleanIndicator1";
			this.booleanIndicator1.Size = new System.Drawing.Size(131, 16);
			this.booleanIndicator1.Status = false;
			this.booleanIndicator1.TabIndex = 4;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(8, 184);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(113, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Log Error";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// ConnectToSPS
			// 
			this.ConnectToSPS.Location = new System.Drawing.Point(248, 184);
			this.ConnectToSPS.Name = "ConnectToSPS";
			this.ConnectToSPS.Size = new System.Drawing.Size(113, 24);
			this.ConnectToSPS.TabIndex = 5;
			this.ConnectToSPS.Text = "ConnectSPS";
			this.ConnectToSPS.UseVisualStyleBackColor = true;
			this.ConnectToSPS.Click += new System.EventHandler(this.ConnectToSPSClick);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(128, 184);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(113, 23);
			this.button2.TabIndex = 6;
			this.button2.Text = "Send Email";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.Button2Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(288, 88);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(168, 20);
			this.textBox1.TabIndex = 7;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(288, 112);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(168, 20);
			this.textBox2.TabIndex = 8;
			// 
			// readOnlyTextBox1
			// 
			this.readOnlyTextBox1.BackColor = System.Drawing.Color.Yellow;
			this.readOnlyTextBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.readOnlyTextBox1.Location = new System.Drawing.Point(8, 224);
			this.readOnlyTextBox1.Multiline = true;
			this.readOnlyTextBox1.Name = "readOnlyTextBox1";
			this.readOnlyTextBox1.ReadOnly = true;
			this.readOnlyTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.readOnlyTextBox1.Size = new System.Drawing.Size(144, 120);
			this.readOnlyTextBox1.TabIndex = 9;
			this.readOnlyTextBox1.Text = "line 1\r\nline 2";
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(248, 208);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(113, 23);
			this.button3.TabIndex = 10;
			this.button3.Text = "Write to TextBox";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.Button3Click);
			// 
			// folderSelect1
			// 
			this.folderSelect1.Location = new System.Drawing.Point(40, 152);
			this.folderSelect1.Name = "folderSelect1";
			this.folderSelect1.Path = null;
			this.folderSelect1.Size = new System.Drawing.Size(430, 24);
			this.folderSelect1.TabIndex = 11;
			this.folderSelect1.ValueChanged += new System.EventHandler(this.FolderSelect1ValueChanged);
			// 
			// folderChanged
			// 
			this.folderChanged.Caption = "folderChanged";
			this.folderChanged.Font = new System.Drawing.Font("Arial", 9.75F);
			this.folderChanged.Location = new System.Drawing.Point(40, 136);
			this.folderChanged.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.folderChanged.MaximumSize = new System.Drawing.Size(500, 16);
			this.folderChanged.MinimumSize = new System.Drawing.Size(100, 16);
			this.folderChanged.Name = "folderChanged";
			this.folderChanged.Size = new System.Drawing.Size(131, 16);
			this.folderChanged.Status = false;
			this.folderChanged.TabIndex = 12;
			// 
			// Menu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(482, 382);
			this.Controls.Add(this.folderChanged);
			this.Controls.Add(this.folderSelect1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.readOnlyTextBox1);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.ConnectToSPS);
			this.Controls.Add(this.booleanIndicator1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.RemoteSocketButton);
			this.Controls.Add(this.LocalSocketButton);
			this.Name = "Menu";
			this.Text = "Menu";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private sar.Controls.BooleanIndicator folderChanged;
		private sarControls.FolderSelect folderSelect1;
		private System.Windows.Forms.Button button3;
		private sar.Controls.ReadOnlyTextBox readOnlyTextBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button ConnectToSPS;
		private System.Windows.Forms.Button button1;
		private sar.Controls.BooleanIndicator booleanIndicator1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Button RemoteSocketButton;
		private System.Windows.Forms.Button LocalSocketButton;
	}
}
