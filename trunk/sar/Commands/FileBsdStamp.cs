/* Copyright (C) 2013 Kevin Boronka
 * 
 * software is distributed under the BSD license
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
using skylib.Tools;
using System.Collections.Generic;
using System.IO;

namespace skylib.sar
{
	public class FileBsdStamp : BaseCommand
	{
		public FileBsdStamp(): base("File: BSD Stamp C# Files",
		                            new List<string> { "file.bsd", "f.bsd" },
		                            "-file.bsd <file_search_pattern>",
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
			
			string filePattern = args[1];
			string copywriter = args[2];
			
			string copyright = "";
			copyright += "/* Copyright (C) " + DateTime.Now.Year.ToString() + " " + copywriter + "\n";
			copyright += " * \n";
			copyright += " * software is distributed under the BSD license\n";
			copyright += " * \n";
			copyright += " * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\"\n";
			copyright += " * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE\n";
			copyright += " * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE\n";
			copyright += " * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE\n";
			copyright += " * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR\n";
			copyright += " * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF\n";
			copyright += " * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS\n";
			copyright += " * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN\n";
			copyright += " * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)\n";
			copyright += " * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE\n";
			copyright += " * POSSIBILITY OF SUCH DAMAGE.\n";
			copyright += " */\n";
			
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			
			List<string> files = IO.GetAllFiles(root, filePattern);
			int updates = 0;
			
			foreach (string file in files)
			{;
				StreamReader reader = new StreamReader(file);
				string code = reader.ReadToEnd();
				reader.Close();
				reader.Dispose();
				
				if (!code.StartsWith(copyright))
				{
					if (code.IndexOf("using System;") != -1)
					{
						int top = code.IndexOf("using ");
						
						if (code.IndexOf("#region") != -1) top = Math.Min(top, code.IndexOf("#region"));
						
						StreamWriter sw = new StreamWriter(file);
						sw.Write(copyright + "\n" + code.Substring(top));
						sw.Close();
						sw.Dispose();
						updates++;
					}
				}
			}
			
			ConsoleHelper.WriteLine("Updates made in " + updates.ToString() + " file" + ((updates != 1) ? "s" : ""), ConsoleColor.DarkYellow);
			
			return Program.EXIT_OK;
		}
	}
}
