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
using System.IO;

namespace sarControls
{
	public partial class FolderSelect : UserControl
	{
		private IWin32Window owner;
		private string path;
		
		public string Path
		{
			get
			{
				if (!string.IsNullOrEmpty(path) && !path.EndsWith(@"\")) path += @"\";
				return path;
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && Directory.Exists(value))
				{
					this.path = value;
					this.pathTextBox.Text = this.path;
				}
			}
		}
		
		#region constructors

		public FolderSelect()
		{
			InitializeComponent();
		}
		
		public FolderSelect(IWin32Window owner)
		{
			this.owner = owner;
			InitializeComponent();
		}
		
		#endregion

		private void FolderSelectResize(object sender, EventArgs e)
		{
			this.pathTextBox.Width = this.Width - this.button.Width;
			this.button.Left = this.Width - this.button.Width;
		}
		
		private void ButtonClick(object sender, EventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.ShowNewFolderButton = true;
			
			if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
			{
				dialog.SelectedPath = path;
			}
			
			if (this.owner == null)
			{
				dialog.ShowDialog();
			}
			else
			{
				dialog.ShowDialog(owner);
			}
			
			if (this.path != dialog.SelectedPath)
			{
				this.path = dialog.SelectedPath;
				this.pathTextBox.Text = this.path;
				NotifyValueChanged();
			}
		}
		
		private void PathTextBoxValidating(object sender, CancelEventArgs e)
		{
			string pathText = this.pathTextBox.Text;
			if (!string.IsNullOrEmpty(pathText) && !Directory.Exists(pathText))
			{
				this.pathTextBox.ForeColor = Color.White;
				this.pathTextBox.BackColor = Color.Red;
				
				DialogResult dialogResult = MessageBox.Show("Create folder", "folder does not exist", MessageBoxButtons.YesNo);
				if(dialogResult == DialogResult.Yes)
				{
					Directory.CreateDirectory(pathText);
				}
				else
				{
					this.pathTextBox.Text = this.Path;
					e.Cancel = true;
				}
			}
			
			this.pathTextBox.ForeColor = SystemColors.WindowText;
			this.pathTextBox.BackColor = SystemColors.Window;
		}
		
		private void PathTextBoxValidated(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.pathTextBox.Text))
			{
				if (this.path != this.pathTextBox.Text)
				{
					this.path = this.pathTextBox.Text;
					NotifyValueChanged();
				}
			}
		}
		
		#region ValueChanged

		private EventHandler valueChanged = null;
		
		//[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler ValueChanged
		{
			add
			{
				valueChanged = (EventHandler)Delegate.Combine(valueChanged, value);
			}
			remove
			{
				valueChanged = (EventHandler)Delegate.Remove(valueChanged, value);
			}
		}
		
		private void NotifyValueChanged()
		{
			try
			{
				valueChanged(this, new System.EventArgs());
			}
			catch
			{
			}
		}

		#endregion
	}
}
