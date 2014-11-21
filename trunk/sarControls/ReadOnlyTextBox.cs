using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace sar.Controls
{
	public class ReadOnlyTextBox : TextBox
	{
		[DllImport("user32.dll")]
		static extern bool HideCaret(IntPtr hWnd);

		public ReadOnlyTextBox()
		{
			this.ReadOnly = true;
			this.BackColor = Color.White;
			this.GotFocus += TextBoxGotFocus;
			this.Cursor = Cursors.Arrow; // mouse cursor like in other controls
			this.Multiline = true;
		}

		private void TextBoxGotFocus(object sender, EventArgs args)
		{
			HideCaret(this.Handle);
		}
	}
}
