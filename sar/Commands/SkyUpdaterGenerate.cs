/* Copyright (C) 2014 Kevin Boronka
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
using System.Reflection;
using System.IO;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class SkyUpdaterGenerate : BaseCommand
	{
		public SkyUpdaterGenerate(CommandHubBase commandHub): base(commandHub, "SkyUpdater - Generate XML file from assembly",
		                                  new List<string> { "sky.generate", "sky.gen" },
		                                  "-sky.generate [xml] [assembly] [url]",
		                                  new List<string> { @"-sky.generate info.xml .\release\sar.exe https://sar-tool.googlecode.com/svn/trunk/release" })
		{

		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 4)
			{
				throw new ArgumentException("incorrect number of arguments");
			}

			Progress.Message = "Searching";
			string filePattern = args[2];
			string root = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref root, ref filePattern);
			List<string> files = IO.GetAllFiles(root, filePattern);
			if (files.Count == 0) throw new FileNotFoundException("File " + filePattern + " not found");
			
			Progress.Message = "Locating XML";
			string xmlFilePattern = args[1];
			string xmlRoot = Directory.GetCurrentDirectory();
			IO.CheckRootAndPattern(ref xmlRoot, ref xmlFilePattern);
			string xmlFile = xmlRoot + xmlFilePattern;
			if (!Directory.Exists(xmlRoot)) throw new DirectoryNotFoundException(xmlRoot + " does not exist");
			
			Progress.Message = "Generating XML";
			SkyUpdater updater = SkyUpdater.Make(AssemblyName.GetAssemblyName(files[0]));
			updater.AddFile(StringHelper.TrimStart(files[0], root.Length), args[3]);
			updater.Save(xmlFile);
			
			ConsoleHelper.WriteLine(IO.GetFilename(xmlFile) + " generated", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
