
namespace sar.Testing
{
	partial class SocketClientForm
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
			this.socketClientControl1 = new sar.Testing.Controls.SocketClientControl();
			this.SuspendLayout();
			// 
			// socketClientControl1
			// 
			this.socketClientControl1.Client = null;
			this.socketClientControl1.Location = new System.Drawing.Point(12, 12);
			this.socketClientControl1.Name = "socketClientControl1";
			this.socketClientControl1.Size = new System.Drawing.Size(442, 344);
			this.socketClientControl1.TabIndex = 0;
			// 
			// SocketClientForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(465, 369);
			this.Controls.Add(this.socketClientControl1);
			this.Name = "SocketClientForm";
			this.Text = "SocketClientForm";
			this.ResumeLayout(false);
		}
		private sar.Testing.Controls.SocketClientControl socketClientControl1;
	}
}
