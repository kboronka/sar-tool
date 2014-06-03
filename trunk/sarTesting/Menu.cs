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
	}
}
