/* Copyright (C) 2014 Kevin Boronka
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
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

using Outlook = Microsoft.Office.Interop.Outlook;

using sar.SPS.Siemens;

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
		
		void Button1Click(object sender, EventArgs e)
		{
			try
			{
				throw new Exception("test");
			}
			catch (Exception ex)
			{
				Program.Log(ex);
			}
		}
		
		void ConnectToSPSClick(object sender, EventArgs e)
		{
			try
			{
				Adapter SiemensSPS = new Adapter("10.242.35.173");
			}
			catch (Exception ex)
			{
				sar.Tools.ExceptionHandler.Display(ex);
			}
		}
		
		private void SendOutlookEmail(string recipient, string subject, string message)
		{
			Outlook.Application outlook = new Microsoft.Office.Interop.Outlook.Application();
			Outlook.MailItem mailItem = (Outlook.MailItem)outlook.CreateItem(Outlook.OlItemType.olMailItem);
			mailItem.Subject = subject;
			mailItem.To = recipient;
			mailItem.Body = message;
			mailItem.Importance = Outlook.OlImportance.olImportanceHigh;
			mailItem.Display(false);
			mailItem.Send();
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			SendOutlookEmail("kboronka@gmail.com", "Subject", "Body");
		}
	}
}
