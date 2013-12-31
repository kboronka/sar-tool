
using System;
using System.Drawing;
using System.Windows.Forms;

namespace sar.Testing
{
	public partial class Menu : Form
	{
		public Menu()
		{
			InitializeComponent();
		}
		
		void LocalSocketClick(object sender, EventArgs e)
		{
			LocalSocket frm = new LocalSocket();
			frm.ShowDialog(this);
			frm.Dispose();
			
		}
		
		void RemoteSocketClick(object sender, EventArgs e)
		{
			RemoteSocket frm = new RemoteSocket();
			frm.ShowDialog(this);
		}
	}
}
