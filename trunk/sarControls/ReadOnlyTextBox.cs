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

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using sar.Tools;

namespace sar.Controls
{
	public class ReadOnlyTextBox : TextBox
	{
		#region externals
		const int EM_LINESCROLL = 0x00B6;

		[DllImport("user32.dll")]
		static extern bool HideCaret(IntPtr hWnd);

		[DllImport("user32.dll")]
		static extern int GetScrollPos(IntPtr hWnd, int nBar);

		[DllImport("user32.dll")]
		static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
		
		[DllImport("user32.dll")]
		static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
		
		[DllImport("user32.dll")]
		static extern bool LockWindowUpdate(IntPtr hWndLock);
		
		#endregion
		
		public override string Text
		{
			get { return StringHelper.TrimWhiteSpace(base.Text); }
			set
			{
				try
				{
					LockWindowUpdate(this.Handle);
					int scrollPosition = GetScrollPos(this.Handle, 1);
					base.Text = value;

					SetScrollPos(this.Handle, 1, scrollPosition, true);
					SendMessage(this.Handle, EM_LINESCROLL, 0, scrollPosition);
				}
				finally
				{
					LockWindowUpdate(IntPtr.Zero);
				}
			}
		}
		
		public ReadOnlyTextBox()
		{
			this.ReadOnly = true;
			this.BackColor = Color.White;
			this.GotFocus += TextBoxGotFocus;
			this.Cursor = Cursors.Arrow; // mouse cursor like in other controls
			this.Multiline = true;
			this.Click += delegate { this.SelectionLength = 0; };
			this.MouseLeave += delegate { this.SelectionLength = 0; };
			//this.MouseMove += delegate { this.SelectionLength = 0; };
			this.DoubleClick += delegate { this.SelectionLength = 0; };
		}

		private void TextBoxGotFocus(object sender, EventArgs args)
		{
			HideCaret(this.Handle);
		}
	}
}
