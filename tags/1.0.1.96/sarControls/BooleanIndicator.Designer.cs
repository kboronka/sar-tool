
namespace sar.Controls
{
	partial class BooleanIndicator
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
			this.textLabel = new System.Windows.Forms.Label();
			this.shapeLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textLabel
			// 
			this.textLabel.AutoSize = true;
			this.textLabel.Location = new System.Drawing.Point(17, 0);
			this.textLabel.Name = "textLabel";
			this.textLabel.Size = new System.Drawing.Size(42, 16);
			this.textLabel.TabIndex = 0;
			this.textLabel.Text = "label1";
			// 
			// shapeLabel
			// 
			this.shapeLabel.BackColor = System.Drawing.Color.Lime;
			this.shapeLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.shapeLabel.Location = new System.Drawing.Point(0, 1);
			this.shapeLabel.Name = "shapeLabel";
			this.shapeLabel.Size = new System.Drawing.Size(12, 12);
			this.shapeLabel.TabIndex = 1;
			// 
			// BooleanIndicator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.shapeLabel);
			this.Controls.Add(this.textLabel);
			this.Font = new System.Drawing.Font("Arial", 9.75F);
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximumSize = new System.Drawing.Size(500, 16);
			this.MinimumSize = new System.Drawing.Size(100, 16);
			this.Name = "BooleanIndicator";
			this.Size = new System.Drawing.Size(100, 16);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label shapeLabel;
		private System.Windows.Forms.Label textLabel;
	}
}
