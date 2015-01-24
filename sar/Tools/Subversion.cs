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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using sar.Tools;

namespace sar.Tools
{
	
	public class SubversionLog : IComparable<SubversionLog>
	{
		// http://svnbook.red-bean.com/en/1.5/svn.ref.svn.c.log.html
		public int Revision { get; set; }
		public DateTime Date { get; set; }
		public string Message { get; set; }
		public string Authour { get; set; }
		public svnInfo Repository { get; set; }
		
		public SubversionLog(svnInfo repo, string logEntry)
		{
			this.Repository = repo;

			logEntry = StringHelper.TrimWhiteSpace(logEntry);
			if (String.IsNullOrEmpty(logEntry))
			{
				throw new NullReferenceException("unspecified log entry");
			}
			
			string[] fields = Regex.Split(logEntry, @" \| ");
			
			// validate number of fields
			if (fields.Length != 4)
			{
				throw new DataMisalignedException("parsed log entry does not have the expected number of fields");
			}
			
			// validate revision number
			if (!fields[0].StartsWith("r"))
			{
				throw new InvalidDataException("invalid revision");
			}
			
			try
			{
				this.Revision = int.Parse(StringHelper.TrimStart(fields[0]));
			}
			catch
			{
				throw new InvalidDataException("invalid revision");
			}
			

			string datestring = Regex.Split(fields[2], @" \(")[0];
			this.Date = DateTime.Parse(datestring);
			
			this.Authour = fields[1];
			
			string [] messageLines = Regex.Split(fields[3], @" line.\n");
			if (messageLines.Length > 1)
			{
				this.Message = StringHelper.TrimWhiteSpace(messageLines[1]);
			}
			else
			{
				this.Message = "";
			}
		}
		
		public int CompareTo(SubversionLog b)
		{
			return this.Revision.CompareTo(b.Revision);
		}
	}
	
	public class svnInfo
	{
		// http://svnbook.red-bean.com/en/1.6/svn.ref.svn.c.info.html
		
		public bool NodeFound { get; set; }
		public string WorkingCopyPath { get; set; }
		public string WorkingCopyRootPath { get; set; }
		public string Repository { get; set; }
		public string RepositoryRoot { get; set; }
		public int Revision { get; set; }
		
		private svnInfo()
		{
			
		}
		
		public static svnInfo ReadLocal(string sandboxPath)
		{
			ShellResults result = Subversion.Run("info", sandboxPath);
			
			svnInfo info = new svnInfo();
			
			if (result.ExitCode == Shell.EXIT_OK)
			{
				info.NodeFound = true;
				info.WorkingCopyPath = sandboxPath;
				info.WorkingCopyRootPath = StringHelper.RegexFindString(result.Output, @"Working Copy Root Path:(.+)\n");
				info.Repository = StringHelper.RegexFindString(result.Output, "URL:(.+)\n");
				info.RepositoryRoot = StringHelper.RegexFindString(result.Output, "Repository Root:(.+)\n");
				info.Revision = int.Parse(StringHelper.RegexFindString(result.Output, "Revision:(.+)\n"));
			}
			else
			{
				info.NodeFound = false;
			}
			
			return info;
		}
		
		public static svnInfo ReadURL(string RepositoryURL)
		{
			ShellResults result = Subversion.Run("info " + RepositoryURL);
			
			svnInfo info = new svnInfo();

			if (result.ExitCode == Shell.EXIT_OK)
			{
				info.NodeFound = true;
				info.WorkingCopyPath = null;
				info.WorkingCopyRootPath = null;
				info.Repository = StringHelper.RegexFindString(result.Output, "URL:(.+)\n");
				info.RepositoryRoot = StringHelper.RegexFindString(result.Output, "Repository Root:(.+)\n");
				info.Revision = int.Parse(StringHelper.RegexFindString(result.Output, "Revision:(.+)\n"));
			}
			else
			{
				info.NodeFound = false;
			}
			
			return info;
		}
	}
	
	public class Subversion
	{
		private string svnPath;
		private static Subversion singleton = new Subversion();
		
		public static string Path
		{
			get
			{
				return singleton.svnPath;
			}
		}
		
		public Subversion()
		{
			this.svnPath = @"C:\Program Files\TortoiseSVN\bin\svn.exe";
			
			try
			{
				if (!File.Exists(this.svnPath))
				{
					this.svnPath = IO.FindApplication("svn.exe");
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("unable to locate svn.exe, subversion may not be installed on this PC", ex);
			}
		}
		
		
		public static ShellResults Run(string subCommand)
		{
			return Subversion.Run(subCommand, Directory.GetCurrentDirectory());
		}
		
		public static ShellResults Run(string subCommand, string sandboxPath)
		{
			return Shell.Run(singleton.svnPath, subCommand, sandboxPath);
		}
		
		public static svnInfo FindSandbox()
		{
			return FindSandbox(Directory.GetCurrentDirectory());
		}
		
		public static svnInfo FindSandbox(string searchPath)
		{
			string sandboxPath = searchPath;
			string rootPath = Directory.GetParent(sandboxPath).Root.FullName;
			
			
			svnInfo info = svnInfo.ReadURL(sandboxPath);
			
			while (!info.NodeFound && sandboxPath != rootPath)
			{
				sandboxPath = Directory.GetParent(sandboxPath).FullName;
				info = svnInfo.ReadLocal(sandboxPath);
			}
			
			if (!info.NodeFound)
			{
				throw new DirectoryNotFoundException("Sandbox not found in " + searchPath);
			}
			
			return info;
		}
		
		public static List<SubversionLog> GetLog()
		{
			svnInfo repo = FindSandbox(Directory.GetCurrentDirectory());
			return GetLog(repo);
		}
		
		public static List<SubversionLog> GetLog(svnInfo repo)
		{
			List<SubversionLog> list = new List<SubversionLog>();
			
			ShellResults result = Shell.Run(singleton.svnPath, "log " + repo.RepositoryRoot);
			
			foreach (string logEntry in Regex.Split(result.Output, new String('-', 72)))
			{
				string entry = StringHelper.TrimWhiteSpace(logEntry);
				if (!String.IsNullOrEmpty(entry))
				{
					list.Add(new SubversionLog(repo, entry));
				}
			}
			
			list.Sort();
			return list;
		}
		
		public static bool SetProperty(svnInfo repo, string name, int revision, string newValue)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new NullReferenceException("property name not specified");
			}
			
			if (revision <= 0 || revision > repo.Revision)
			{
				throw new ArgumentOutOfRangeException("invalid revision");
			}
			
			ShellResults result = Subversion.Run("propset " + name + " -r " + revision.ToString() + " " + newValue + " " + repo.RepositoryRoot);
			return (result.ExitCode == Shell.EXIT_OK);
		}
	}
}
