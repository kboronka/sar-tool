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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace sar.Tools
{
	public partial class ExceptionHandler : Form
	{
		public ExceptionHandler(Exception ex)
		{
			InitializeComponent();
			this.Text = ex.GetType().ToString();
			this.details.Text = ex.Message;
			this.details.Text += Environment.NewLine;
			this.details.Text += GetStackTrace(ex);
		}
		
		public static void Display(Exception ex)
		{
			// temporary
			ExceptionHandler form = new ExceptionHandler(GetInnerException(ex));
			form.ShowDialog();
		}
		
		public static Exception GetInnerException(Exception ex)
		{
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			
			return ex;
		}
		
		public static string GetStackTrace(Exception ex)
		{
			string result = "";
			
			string stackTrace = ex.StackTrace;
			string regex = @"(\s*)at\s((.?)*)\sin\s((.?)*):line\s([1-9]*)";
			
			
			string[] lines = stackTrace.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
			
			foreach (string line in lines)
			{
				MatchCollection regexResults = Regex.Matches(line, regex);
				if (regexResults.Count == 1 && regexResults[0].Groups.Count == 7)
				{
					if (!String.IsNullOrEmpty(result)) result += Environment.NewLine;
					string method = regexResults[0].Groups[2].Value;
					string filepath = regexResults[0].Groups[4].Value;
					string lineNumber = regexResults[0].Groups[6].Value;

					string filename = IO.GetFilename(filepath);
					
					if (method.Contains("."))
					{
						method = method.Substring(method.LastIndexOf('.') + 1);
					}
					
					if (method.Contains("("))
					{
						method = method.Substring(0, method.LastIndexOf('('));
					}
										
					result += "\t" + method + "() in " + filename + ":line " + lineNumber;
				}
			}
			
			return result;
		}
	}
}