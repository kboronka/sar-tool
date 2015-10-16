﻿/* Copyright (C) 2015 Kevin Boronka
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
using System.Net;
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class FanucFTP : Command
	{
		public FanucFTP(Base.CommandHub parent) : base(parent, "FanucBackup",
		                                                  new List<string> { "fanuc.bk" },
		                                                  @"-fanuc.bk <ip:port> <path>",
		                                                  new List<string> { @"-fanuc.bk 192.168.0.2:21 C:\Temp" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			var ip = args[1];
			var path = args[2];
			
			if (path.EndsWith(@"\")) path = StringHelper.TrimEnd(path, 1);
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
			
			var files = GetFileList(ip);
			for (int i = 0; i < files.Count; i++)
			{
				var file = files[i];
				var progress = (i / (double)files.Count) * 100;
				
				Progress.Message = "Downloading " + progress.ToString("0") + "% [" + file + "]";
				
				try
				{
					DownloadFile(ip, file, path);
				}
				catch (Exception ex)
				{
					ConsoleHelper.WriteLine("Error downloading " + file);
					ConsoleHelper.WriteException(ex);
				}
			}
			
			ConsoleHelper.WriteLine(files.Count.ToString() + " Files" + ((files.Count != 1) ? "s" : "") + " Downloaded", ConsoleColor.DarkYellow);
			
			return ConsoleHelper.EXIT_OK;
		}
		
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