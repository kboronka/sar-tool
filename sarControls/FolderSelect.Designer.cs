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

namespace sarControls
{
	partial class FolderSelect
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderSelect));
			this.pathTextBox = new System.Windows.Forms.TextBox();
			this.button = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pathTextBox
			// 
			this.pathTextBox.Location = new System.Drawing.Point(0, 0);
			this.pathTextBox.Name = "pathTextBox";
			this.pathTextBox.Size = new System.Drawing.Size(392, 20);
			this.pathTextBox.TabIndex = 0;
			this.pathTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.PathTextBoxValidating);
			this.pathTextBox.Validated += new System.EventHandler(this.PathTextBoxValidated);
			// 
			// button
			// 
			this.button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button.BackgroundImage")));
			this.button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.button.Location = new System.Drawing.Point(392, 0);
			this.button.Name = "button";
			this.button.Size = new System.Drawing.Size(20, 20);
			this.button.TabIndex = 1;
			this.button.UseVisualStyleBackColor = true;
			this.button.Click += new System.EventHandler(this.ButtonClick);
			// 
			// FolderSelect
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.button);
			this.Controls.Add(this.pathTextBox);
			this.Name = "FolderSelect";
			this.Size = new System.Drawing.Size(414, 21);
			this.Resize += new System.EventHandler(this.FolderSelectResize);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button button;
		private System.Windows.Forms.TextBox pathTextBox;
	}
}
