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
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class FileBsdHeader : Command
	{
		public FileBsdHeader(Base.CommandHub commandHub): base(commandHub, "File - BSD Stamp C# Files",
		                            new List<string> { "file.bsd", "f.bsd" },
		                            "-file.bsd [file_search_pattern]",
		                            new List<string> { "-file.bsd *.cs" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string copywriter = args[2];
			string copyright = "";
			copyright += "/* Copyright (C) " + DateTime.Now.Year.ToString() + " " + copywriter + "\r\n";
			copyright += " * \r\n";
			copyright += " * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\"\r\n";
			copyright += " * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE\r\n";
			copyright += " * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE\r\n";
			copyright += " * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE\r\n";
			copyright += " * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR\r\n";
			copyright += " * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF\r\n";
			copyright += " * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS\r\n";
			copyright += " * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN\r\n";
			copyright += " * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)\r\n";
			copyright += " * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE\r\n";
			copyright += " * POSSIBILITY OF SUCH DAMAGE.\r\n";
			copyright += " */\r\n";
			
			Progress.Message = "Searching";
			string filePattern = args[1];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);

			int updates = 0;
			
			foreach (string file in files)
			{
				StreamReader reader = new StreamReader(file);
				string code = reader.ReadToEnd();
				reader.Close();
				reader.Dispose();
				
				if (!code.StartsWith(copyright + "\r\n"))
				{
					if (code.IndexOf("namespace ") != -1)
					{
						int top = code.IndexOf("namespace ");
						
						if (code.IndexOf("#region") != -1) top = Math.Min(top, code.IndexOf("#region"));
						if (code.IndexOf("using ") != -1) top = Math.Min(top, code.IndexOf("using "));
						
						StreamWriter sw = new StreamWriter(file);
						sw.Write(copyright + "\r\n" + code.Substring(top));
						sw.Close();
						sw.Dispose();
						updates++;
					}
				}
			}
			
			ConsoleHelper.WriteLine("BSD Header updated on " + updates.ToString() + " file" + ((updates != 1) ? "s" : ""), ConsoleColor.DarkYellow);
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}
