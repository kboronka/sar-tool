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
			this.booleanIndicator1.Location = new System.Drawing.Point(234, 101);
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
			this.button1.Location = new System.Drawing.Point(12, 182);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(113, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Log Error";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// ConnectToSPS
			// 
			this.ConnectToSPS.Location = new System.Drawing.Point(325, 136);
			this.ConnectToSPS.Name = "ConnectToSPS";
			this.ConnectToSPS.Size = new System.Drawing.Size(113, 23);
			this.ConnectToSPS.TabIndex = 5;
			this.ConnectToSPS.Text = "ConnectSPS";
			this.ConnectToSPS.UseVisualStyleBackColor = true;
			this.ConnectToSPS.Click += new System.EventHandler(this.ConnectToSPSClick);
			// 
			// Menu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(507, 235);
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
		}
		private System.Windows.Forms.Button ConnectToSPS;
		private System.Windows.Forms.Button button1;
		private sar.Controls.BooleanIndicator booleanIndicator1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Button RemoteSocketButton;
		private System.Windows.Forms.Button LocalSocketButton;
	}
}
