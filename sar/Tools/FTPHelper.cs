using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace sar.Tools
{
	public static class FTPHelper
	{
		public static List<string> GetFileList(string ip)
		{
			var downloadFiles = new List<string>();
			
			var reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ip + "/"));
			reqFTP.UseBinary = true;
			reqFTP.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
			reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
			reqFTP.Proxy = null;
			reqFTP.KeepAlive = false;
			reqFTP.UsePassive = false;
			
			var response = reqFTP.GetResponse();
			var reader = new StreamReader(response.GetResponseStream());
			
			string line = reader.ReadLine();
			while (line != null)
			{
				line.Replace("\n", "");
				downloadFiles.Add(line);
				
				line = reader.ReadLine();
			}
			
			return downloadFiles;
		}

		public static byte[] DownloadBytes(string ip, string file)
		{
			var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ip + "/" + file));
			ftpRequest.UseBinary = true;
			ftpRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
			ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
			ftpRequest.Proxy = null;
			ftpRequest.KeepAlive = false;
			ftpRequest.UsePassive = false;
			ftpRequest.Timeout = 30 * 1000;

			var ftpResponce = (FtpWebResponse)ftpRequest.GetResponse();
			var ftpStream = ftpResponce.GetResponseStream();

			

			var buffer = new Byte[2048];
			int bytesread = 0;
			
			using (var ms = new MemoryStream())
			{
				while ((bytesread = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, bytesread);
				}
				
				ftpResponce.Close();
				return ms.ToArray();
			}
		}
		
		public static void DownloadFile(string ip, string file, string root)
		{
			var buffer = DownloadBytes(ip, file);
			
			var writeStream = new FileStream(root + @"\" + file, FileMode.Create);
			writeStream.Write(buffer, 0, buffer.Length);
			writeStream.Close();
		}		
	}
}
