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
using System.Windows.Forms;

using Base=sar.Base;
using sar.Tools;
using sar.Http;

namespace sar.Testing
{
	internal sealed class Program : Base.Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(LogUnhandledException);
			
			Base.Program.LogInfo();

			foreach (string resource in EmbeddedResource.GetAllResources())
			{
				System.Diagnostics.Debug.WriteLine(resource);
			}
			
			string root = IO.CheckRoot(@".\");
			root = IO.CheckRoot(@"..\");
			root = IO.CheckRoot(@"..\..\");
			root = IO.CheckRoot(@"\");
				
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Http.HttpServer server = new Http.HttpServer(83);
			Application.Run(new Menu());
			Program.Log("throwing error");
			//throw new ApplicationException("testing unhandled exception");
			// Program.Log("shutting down");
			// Application.Exit();
		}
	}
}
