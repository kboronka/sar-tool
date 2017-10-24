/* Copyright (C) 2017 Kevin Boronka
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
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace sar.Tools
{
	public static class FTPHelper
	{
		public static List<string> GetFileList(string ip)
		{
			var files = new List<string>();
			
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
				files.Add(line);
				
				line = reader.ReadLine();
			}
			
			return files;
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
