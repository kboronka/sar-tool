
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
			get { return path; }
			set
			{
				if (!string.IsNullOrEmpty(value) && Directory.Exists(value))
				{
					path = value;
				}

				if (!string.IsNullOrEmpty(value) && !Directory.Exists(value))
				{

				}

				this.pathTextBox.Text = path;
			}
		}
		
		public FolderSelect()
		{
			InitializeComponent();
		}
		
		public FolderSelect(IWin32Window owner)
		{
			this.owner = owner;
			InitializeComponent();
		}
		
		void FolderSelectResize(object sender, EventArgs e)
		{
			this.pathTextBox.Width = this.Width - this.button.Width;
			this.button.Left = this.Width - this.button.Width;
		}
		
		void ButtonClick(object sender, EventArgs e)
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
			
			this.Path = dialog.SelectedPath;
		}
		
		void PathTextBoxValidating(object sender, CancelEventArgs e)
		{
			string pathText = this.pathTextBox.Text;
			if (!string.IsNullOrEmpty(pathText) && !Directory.Exists(pathText))
			{
				DialogResult dialogResult = MessageBox.Show("Create folder", "folder does not exist", MessageBoxButtons.YesNo);
				if(dialogResult == DialogResult.Yes)
				{
					Directory.CreateDirectory(pathText);
				}
				else
				{
					e.Cancel = true;
				}
			}
		}
		
		void PathTextBoxValidated(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.pathTextBox.Text))
			{
				this.path = this.pathTextBox.Text;
			}
		}
	}
}
