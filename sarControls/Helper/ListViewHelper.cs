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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Timers;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace sar.Controls
{
	/// <summary>
	/// Contains helper methods to change extended styles on ListView, including enabling double buffering.
	/// Based on Giovanni Montrone's article on <see cref="http://www.codeproject.com/KB/list/listviewxp.aspx"/>
	/// </summary>
	public class ListViewHelper
	{
		private enum ListViewExtendedStyles
		{
			GridLines = 0x00000001,
			SubItemImages = 0x00000002,
			CheckBoxes = 0x00000004,
			TrackSelect = 0x00000008,
			HeaderDragDrop = 0x00000010,
			FullRowSelect = 0x00000020,
			OneClickActivate = 0x00000040,
			TwoClickActivate = 0x00000080,
			FlatsB = 0x00000100,
			Regional = 0x00000200,
			InfoTip = 0x00000400,
			UnderlineHot = 0x00000800,
			UnderlineCold = 0x00001000,
			MultilWorkAreas = 0x00002000,
			LabelTip = 0x00004000,
			BorderSelect = 0x00008000,
			DoubleBuffer = 0x00010000,
			HideLabels = 0x00020000,
			SingleRow = 0x00040000,
			SnapToGrid = 0x00080000,
			SimpleSelect = 0x00100000
		}

		private enum ListViewMessages
		{
			First = 0x1000,
			SetExtendedStyle = (First + 54),
			GetExtendedStyle = (First + 55),
		}
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SendMessage(IntPtr handle, int messg, int wparam, int lparam);

		private static void SetExtendedStyle(Control control, ListViewExtendedStyles exStyle)
		{
			ListViewExtendedStyles styles;
			styles = (ListViewExtendedStyles)SendMessage(control.Handle, (int)ListViewMessages.GetExtendedStyle, 0, 0);
			styles |= exStyle;
			SendMessage(control.Handle, (int)ListViewMessages.SetExtendedStyle, 0, (int)styles);
		}

		public static void EnableDoubleBuffer(Control control)
		{
			ListViewExtendedStyles styles;
			styles = (ListViewExtendedStyles)SendMessage(control.Handle, (int)ListViewMessages.GetExtendedStyle, 0, 0);
			styles |= ListViewExtendedStyles.DoubleBuffer | ListViewExtendedStyles.BorderSelect;
			SendMessage(control.Handle, (int)ListViewMessages.SetExtendedStyle, 0, (int)styles);
		}
		public static void DisableDoubleBuffer(Control control)
		{
			ListViewExtendedStyles styles;
			styles = (ListViewExtendedStyles)SendMessage(control.Handle, (int)ListViewMessages.GetExtendedStyle, 0, 0);
			styles -= styles & ListViewExtendedStyles.DoubleBuffer;
			styles -= styles & ListViewExtendedStyles.BorderSelect;
			SendMessage(control.Handle, (int)ListViewMessages.SetExtendedStyle, 0, (int)styles);
		}
	}
}