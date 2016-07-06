/* Copyright (C) 2016 Kevin Boronka
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

namespace sar.CNC
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
			this.txtCommLog = new System.Windows.Forms.TextBox();
			this.jogForward = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtCommLog
			// 
			this.txtCommLog.Location = new System.Drawing.Point(12, 24);
			this.txtCommLog.Multiline = true;
			this.txtCommLog.Name = "txtCommLog";
			this.txtCommLog.Size = new System.Drawing.Size(312, 107);
			this.txtCommLog.TabIndex = 19;
			// 
			// jogForward
			// 
			this.jogForward.Location = new System.Drawing.Point(211, 137);
			this.jogForward.Name = "jogForward";
			this.jogForward.Size = new System.Drawing.Size(113, 23);
			this.jogForward.TabIndex = 20;
			this.jogForward.Text = "Jog +";
			this.jogForward.UseVisualStyleBackColor = true;
			this.jogForward.Click += new System.EventHandler(this.JogForwardClick);
			// 
			// Menu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(378, 276);
			this.Controls.Add(this.jogForward);
			this.Controls.Add(this.txtCommLog);
			this.Name = "Menu";
			this.Text = "Menu";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.TextBox txtCommLog;
		private System.Windows.Forms.Button jogForward;
	}
}
