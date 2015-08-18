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

namespace sar_testing
{
	partial class Form1
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
			this.readOnlyTextBox1 = new sar.Controls.ReadOnlyTextBox();
			this.folderSelect1 = new sarControls.FolderSelect();
			this.SuspendLayout();
			// 
			// readOnlyTextBox1
			// 
			this.readOnlyTextBox1.BackColor = System.Drawing.Color.White;
			this.readOnlyTextBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.readOnlyTextBox1.Location = new System.Drawing.Point(1, 13);
			this.readOnlyTextBox1.Multiline = true;
			this.readOnlyTextBox1.Name = "readOnlyTextBox1";
			this.readOnlyTextBox1.ReadOnly = true;
			this.readOnlyTextBox1.Size = new System.Drawing.Size(250, 20);
			this.readOnlyTextBox1.TabIndex = 0;
			// 
			// folderSelect1
			// 
			this.folderSelect1.Location = new System.Drawing.Point(1, 40);
			this.folderSelect1.Name = "folderSelect1";
			this.folderSelect1.Path = null;
			this.folderSelect1.Size = new System.Drawing.Size(414, 21);
			this.folderSelect1.TabIndex = 1;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(423, 80);
			this.Controls.Add(this.folderSelect1);
			this.Controls.Add(this.readOnlyTextBox1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private sarControls.FolderSelect folderSelect1;
		private sar.Controls.ReadOnlyTextBox readOnlyTextBox1;
	}
}
