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
			// Menu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(507, 235);
			this.Controls.Add(this.RemoteSocketButton);
			this.Controls.Add(this.LocalSocketButton);
			this.Name = "Menu";
			this.Text = "Menu";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button RemoteSocketButton;
		private System.Windows.Forms.Button LocalSocketButton;
	}
}
