using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace sar.Controls
{
	public partial class BooleanIndicator : UserControl
	{
		private string caption;
		private bool status;
		
		public string Caption
		{
			get { return this.caption; }
			set
			{
				this.caption = value;
				this.UpdateDisplay();
			}
		}
		
		public bool Status
		{
			get { return this.status; }
			set
			{
				this.status = value;
				this.UpdateDisplay();
			}
		}
		
		public BooleanIndicator()
		{
			this.caption = "booleanIndicator";
			this.status = false;
			InitializeComponent();
			this.UpdateDisplay();
		}
		
		private void UpdateDisplay()
		{
			this.textLabel.Text = this.caption;
			this.shapeLabel.BackColor = (this.status ? Color.Lime : Color.Silver);
		}
	}
}
