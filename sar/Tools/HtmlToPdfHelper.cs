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
			lock (pdfWriter)
			{
				if (string.IsNullOrEmpty(PdfWriter)) throw new ApplicationException("PDF output is not supported");
				
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
