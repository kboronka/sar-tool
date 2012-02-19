/*
 * User: Kevin Boronka (kboronka@gmail.com)
 * Date: 2/7/2012
 * Time: 11:57 PM
 */
using System;
using System.IO;

using SkylaLib.Tools;

namespace SkylaLib.sar
{
	class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				Console.WriteLine(AssemblyInfo.Name + "  v" + AssemblyInfo.Version + "  " + AssemblyInfo.Copyright);
				
				if (args.Length != 3)
				{
					Usage();
					return;
				}
				else
				{
					string root = Directory.GetCurrentDirectory();
					#if DEBUG
					Console.WriteLine("args[0]=" + args[0]);
					Console.WriteLine("args[1]=" + args[1]);
					Console.WriteLine("args[2]=" + args[2]);
					#endif
					
					foreach (string file in IO.SearchAndReplaceInFiles(root, args[0], args[1], args[2]))
					{
						Console.WriteLine(file.Replace(root, ""));
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			
			#if DEBUG
			Console.ReadKey();
			#endif
		}
		
		public static void Usage()
		{
			Console.WriteLine("Usage: sar <file_search_pattern> <search_text> <replace_text>");
			Console.WriteLine("Example: sar \"*.ie9\" \"bing\" \"google\"");
		}
	}
}