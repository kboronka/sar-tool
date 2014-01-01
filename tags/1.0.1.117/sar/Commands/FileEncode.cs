/* Copyright (C) 2013 Kevin Boronka
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
using sar.Tools;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace sar.Tools
{
	public class FileEncode : BaseCommand
	{
		public FileEncode() : base("File - Encode",
		                               new List<string> { "file.encode", "f.e" },
		                               "-file.encode [filepattern] [UTF7 | UTF8 | UTF32 | Unicode | BigEndianUnicode | ASCII]",
		                               new List<string> { "-file.encode \"*.nsis\" utf8" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			Progress.Message = "Searching";
			string filePattern = args[1];
			string encodingRequested = args[2];
			Encoding encoding;
			
			switch (encodingRequested.ToLower())
			{
				case "utf7":
					encoding = Encoding.UTF7;
					break;
				case "utf8":
					encoding = Encoding.UTF8;
					break;
				case "utf32":
					encoding = Encoding.UTF32;
					break;
				case "unicode":
					encoding = Encoding.Unicode;
					break;
				case "ascii":
					encoding = Encoding.ASCII;
					break;
				case "bigendianunicode":
					encoding = Encoding.BigEndianUnicode;
					break;
				default:
					throw new ArgumentException(encodingRequested + " encoding method is currently not supported");
			}
			
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);

			Progress.Message = "Encoding Files";
			foreach (string file in files)
			{
				ConsoleHelper.Write("encode: ", ConsoleColor.Cyan);
				ConsoleHelper.WriteLine(file.Substring(root.Length));
				IO.Encode(file, encoding);
			}

			ConsoleHelper.WriteLine("Files Enocded " + files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : ""), ConsoleColor.DarkYellow);
			
			return Program.EXIT_OK;
		}
	}
}
