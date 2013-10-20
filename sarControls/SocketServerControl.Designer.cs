using sar.Tools;
using sar.Socket;

namespace sar.Controls
{
	partial class SocketServerControl
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
			this.ActiveConnections = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// ActiveConnections
			// 
			this.ActiveConnections.ForeColor = System.Drawing.SystemColors.Control;
			this.ActiveConnections.Location = new System.Drawing.Point(3, 11);
			this.ActiveConnections.Name = "ActiveConnections";
			this.ActiveConnections.Size = new System.Drawing.Size(149, 16);
			this.ActiveConnections.TabIndex = 1;
			this.ActiveConnections.Text = "Active Connections: 0";
			// 
			// SocketServerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.Controls.Add(this.ActiveConnections);
			this.Name = "SocketServerControl";
			this.Size = new System.Drawing.Size(273, 90);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label ActiveConnections;
	}
}
