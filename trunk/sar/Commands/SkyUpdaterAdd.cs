/*
 * Created by SharpDevelop.
 * User: kboronka
 * Date: 2013-05-27
 * Time: 4:29 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using skylib.Tools;

namespace skylib.sar
{
	public class SkyUpdaterAdd : BaseCommand
	{
		public SkyUpdaterAdd(): base("SkyUpdater - Add file",
		                             new List<string> { "sky.add", "sky.add" },
		                             "-sky.add [xml] [file] [url]",
		                             new List<string> { @"-sky.add info.xml .\release\sar.exe https://sar-tool.googlecode.com/svn/trunk/release/sar.exe" })
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
			string xmlFile = IO.FindFile(xmlRoot, xmlFilePattern);
			if (!Directory.Exists(xmlRoot)) throw new DirectoryNotFoundException(xmlRoot + " does not exist");
			if (!File.Exists(xmlFile)) throw new FileNotFoundException(xmlFile + " does not exist");
			
			Progress.Message = "Generating XML";
			SkyUpdater updater = new SkyUpdater(xmlFile);
			foreach (string file in files)
			{
				updater.AddFile(StringHelper.TrimStart(file, root.Length), args[3]);
			}
			
			updater.Save(xmlFile);
			
			ConsoleHelper.WriteLine(root + filePattern + " updated", ConsoleColor.DarkYellow);
			return Program.EXIT_OK;
		}
	}
}