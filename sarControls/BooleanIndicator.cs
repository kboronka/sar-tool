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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace sar.Controls
{
	public partial class BooleanIndicator : UserControl
	{
		private string caption;
		private bool status;
		
		#region properties
		
		[Category("Extended Options")]
		[Description("Text displayed")]
		public string Caption
		{
			get { return this.caption; }
			set
			{
				this.caption = value;
				this.UpdateDisplay();
			}
		}
		
		[Category("Extended Options")]
		[Description("On/Off state")]		
		public bool Status
		{
			get { return this.status; }
			set
			{
				this.status = value;
				this.UpdateDisplay();
			}
		}
		
		#endregion

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
