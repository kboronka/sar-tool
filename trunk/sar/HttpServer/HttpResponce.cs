using System;

using sar.Tools;

namespace sar.HttpServer
{
	public class HttpResponce
	{
		private HttpRequest request;
		private string contentType = "text/html";
		private byte[] content;
		
		public HttpResponce(HttpRequest request)
		{
			this.request = request;
			/*
			// Send responce
			lock (request.socket)
			{
				StreamWriter output = new StreamWriter(new BufferedStream(socket.GetStream()));
				
				output.WriteLine("HTTP/1.0 200 OK");
				output.WriteLine("Content-Type: " + this.contentType);
				output.WriteLine("Content-Length: " + this.content.Length.ToString());
				output.WriteLine("Connection: close");
				output.WriteLine("");
				output.Flush();
				
				stream.Write(contentBytes, 0, contentBytes.Length);
				stream.Flush();
				
				output = null;
				stream = null;

				result += "" + "\n";
				socket.Close();
			}
			*/
		}
		
		/*
		private string Header()
		{
			string header;
			
		}
		*/
	}
}
