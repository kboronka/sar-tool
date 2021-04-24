/* Copyright (C) 2021 Kevin Boronka
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
using System.IO;

namespace sar.Tools
{
	public static class HtmlToPdfHelper
	{
		// documentation: http://wkhtmltopdf.org/usage/wkhtmltopdf.txt
		private static string pdfWriter;

		private static string PdfWriter
		{
			get
			{
				if (string.IsNullOrEmpty(pdfWriter))
				{
					pdfWriter = IO.FindApplication("wkhtmltopdf.exe", @"wkhtmltopdf\bin");
				}

				return pdfWriter;
			}
		}

		public static byte[] ReadPDF(string url)
		{
			if (string.IsNullOrEmpty(PdfWriter))
				throw new ApplicationException("PDF output is not supported");

			lock (pdfWriter)
			{
				string tempfile = ApplicationInfo.DataDirectory + Guid.NewGuid().ToString() + ".pdf";
				const string options = "--page-size A4 --viewport-size 1280x1024 --margin-top 3 --margin-bottom 3 --margin-left 3 --margin-right 3";
				string output = "";

				int exitCode = ConsoleHelper.Run(PdfWriter, options + " " + url.QuoteDouble() + " " + tempfile.QuoteDouble(), out output);

				var data = File.ReadAllBytes(tempfile);
				File.Delete(tempfile);
				return data;
			}
		}
	}
}
