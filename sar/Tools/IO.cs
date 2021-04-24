/* Copyright (C) 2021 Kevin Boronka
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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace sar.Tools
{
	public static class IO
	{
		public static bool IncludeSubFolders = true;

		public static string ProgramFilesx86
		{
			get
			{
				if (IntPtr.Size == 8 || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
				{
					return Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\";
				}

				return Environment.GetEnvironmentVariable("ProgramFiles") + @"\";
			}
		}

		public static string ProgramFiles
		{
			get
			{
				return Environment.GetEnvironmentVariable("ProgramW6432") + @"\";
			}
		}

		public static string Windows
		{
			get
			{
				return Environment.GetEnvironmentVariable("SystemRoot") + @"\";
			}
		}

		public static string System32
		{
			get
			{
				return Environment.SystemDirectory + @"\";
			}
		}

		public static string Temp
		{
			get
			{
				return System.IO.Path.GetTempPath();
			}
		}

		public static bool IsSVN(string path)
		{
			path = path.ToLower();

			if (path.Contains(@"\.svn\"))
				return true;
			if (path.Contains(@"\.cvs\"))
				return true;

			return false;
		}

		public static bool IsFileReadOnly(string path)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException("file " + path + " not found");

			var info = new System.IO.FileInfo(path);
			return (System.IO.FileAttributes.ReadOnly == info.Attributes);
		}

		public static List<string> GetAllDirectories(string root)
		{
			var directories = new List<string>();
			directories.Add(root);

			try
			{
				if (IO.IncludeSubFolders)
				{
					foreach (string dir in System.IO.Directory.GetDirectories(root))
					{
						try
						{
							directories.AddRange(GetAllDirectories(dir));
						}
						catch (Exception ex)
						{
							ConsoleHelper.DebugWriteLine(ex.Message);
						}
					}
				}
			}
			catch (Exception ex)
			{
				ConsoleHelper.DebugWriteLine(ex.Message);
			}

			return directories;
		}

		public static List<string> GetAllFiles(string root)
		{
			if (String.IsNullOrEmpty(root))
			{
				throw new NullReferenceException("root search path was not specified");
			}

			string pattern = "*.*";

			// handle filepaths in root
			if (!Directory.Exists(root) && root.Contains("*"))
			{
				ConsoleHelper.DebugWriteLine("root: " + root);

				pattern = root.Substring(root.LastIndexOf('\\') + 1);
				root = root.Substring(0, root.LastIndexOf('\\'));

				ConsoleHelper.DebugWriteLine("root: " + root);
				ConsoleHelper.DebugWriteLine("pattern: " + pattern);
			}

			return GetAllFiles(root, pattern);
		}

		public static string GetRoot(string filepath)
		{
			string root = filepath.Substring(0, filepath.LastIndexOf('\\'));
			if (root.Substring(root.Length - 1) != "\\")
				root += "\\";
			return root;
		}

		public static string GetFilename(string filepath)
		{
			return filepath.Substring(filepath.LastIndexOf('\\') + 1);
		}

		public static string GetFileDirectory(string filepath)
		{
			return filepath.Substring(0, filepath.LastIndexOf('\\'));
		}

		public static string GetFileExtension(string filepath)
		{
			string filename = GetFilename(filepath);

			return (filename.LastIndexOf(".", StringComparison.CurrentCulture) != -1) ? filename.Substring(filename.LastIndexOf(".", StringComparison.CurrentCulture) + 1) : "";
		}

		public static string CheckRoot(string root)
		{
			if (!root.StartsWith(@"\\", StringComparison.CurrentCulture))
			{
				if (root.StartsWith(@"..\", StringComparison.CurrentCulture))
					root = Directory.GetCurrentDirectory() + @"\" + root;
				if (root.StartsWith(@".\", StringComparison.CurrentCulture))
					root = Directory.GetCurrentDirectory() + @"\" + root;
				if (root.StartsWith(@"\", StringComparison.CurrentCulture))
					root = Directory.GetCurrentDirectory() + root;
			}

			if (!root.EndsWith(@"\", StringComparison.CurrentCulture))
				root += @"\";

			root = root.Replace(@"\.\", @"\");
			return root;
		}

		public static void CheckRootAndPattern(ref string root)
		{
			string pattern = "*.*";
			CheckRootAndPattern(ref root, ref pattern);
		}

		public static string CheckPath(string root, string path)
		{
			if (path.LastIndexOf("\"") != -1)
				path = path.Substring(0, path.Length - 1);
			if (root.LastIndexOf("\"") != -1)
				root = root.Substring(0, root.Length - 1);
			if (path.LastIndexOf(@"\") == -1)
				path = @".\" + path;
			if (path.Substring(path.Length - 1, 1) != @"\")
				path += @"\";
			CheckRootAndPattern(ref root, ref path);

			return root + path;
		}

		public static void CheckRootAndPattern(ref string root, ref string pattern)
		{
			if (String.IsNullOrEmpty(root))
			{
				throw new NullReferenceException("root search path was not specified");
			}

			if (!Directory.Exists(root))
			{
				throw new FileNotFoundException("root search path does not exist [" + root + "]");
			}

			var files = new List<string>();

			if (pattern.LastIndexOf(':') != -1)
			{
				// mapped drive absolute paths
				root = pattern.Substring(0, pattern.LastIndexOf(@"\") + 1);
				pattern = pattern.Substring(pattern.LastIndexOf(@"\") + 1, pattern.Length - pattern.LastIndexOf(@"\") - 1);
			}
			else if (pattern.Substring(0, 2) == @"\\")
			{
				// UNC absolute paths
				root = pattern.Substring(0, pattern.LastIndexOf(@"\"));
				pattern = pattern.Substring(pattern.LastIndexOf(@"\") + 1, pattern.Length - pattern.LastIndexOf(@"\") - 1);
			}
			else if (pattern.LastIndexOf(@"\") != -1)
			{
				// relative paths
				if (pattern.Substring(0, 1) != @"\")
					pattern = @"\" + pattern;
				if (root.Substring(root.Length - 1) == @"\")
					root = root.Substring(0, root.Length - 1);

				root += pattern.Substring(0, pattern.LastIndexOf(@"\"));
				pattern = pattern.Substring(pattern.LastIndexOf(@"\") + 1, pattern.Length - pattern.LastIndexOf(@"\") - 1);
			}

			if (root.Substring(root.Length - 1) != "\\")
				root += "\\";
		}

		public static List<string> GetAllFiles(string root, string pattern)
		{
			CheckRootAndPattern(ref root, ref pattern);

			var files = new List<string>();
			var dirs = GetAllDirectories(root);
			foreach (string dir in dirs)
			{
				try
				{
					files.AddRange(Directory.GetFiles(dir, pattern));
				}
				catch (Exception ex)
				{
					ConsoleHelper.DebugWriteLine(ex.Message);
				}
			}

			return files;
		}

		public static int GetLineNumber(string content, int index)
		{
			int lines = 1;

			for (int i = 0; i <= index - 1 && i <= content.Length - 1; i++)
			{
				if (content[i] == '\n')
				{
					lines++;
				}
			}

			return lines;
		}

		#region searching within files

		public static List<SearchResults> SearchAndReplaceInFiles(string root, string filePattern, string search, string replace)
		{
			var results = new List<SearchResults>();

			IO.CheckRootAndPattern(ref root, ref filePattern);

			var files = IO.GetAllFiles(root, filePattern);
			ConsoleHelper.DebugWriteLine("searching " + files.Count.ToString() + " file" + ((files.Count == 1) ? "" : "s"));

			foreach (string file in files)
			{
				if (!IO.IsSVN(file))
				{
					var fileResults = new SearchResults(file);
					var searchResults = SearchAndReplaceInFile(file, search, replace);
					fileResults.AddResults(searchResults);
					results.Add(fileResults);
				}
			}

			return results;
		}

		public static List<SearchResultMatch> SearchAndReplace(ref string content, string search, string replace, string reason)
		{
			var result = new List<SearchResultMatch>();

			var matches = Regex.Matches(content, search);
			foreach (Match match in matches)
			{
				var lineNumber = GetLineNumber(content, match.Index);
				content = Regex.Replace(content, search, replace);
				result.Add(new SearchResultMatch(match, lineNumber, reason));
			}

			return result;
		}

		public static List<SearchResultMatch> SearchAndReplaceInFile(string path, string search, string replace)
		{
			return SearchAndReplaceInFile(path, search, replace, "");
		}

		public static List<SearchResultMatch> SearchAndReplaceInFile(string path, string search, string replace, string reason)
		{
			string content = ReadFileAsUtf8(path);
			var result = SearchAndReplace(ref content, search, replace, reason);

			if (result.Count > 0)
			{
				// make file not readonly
				File.SetAttributes(path, FileAttributes.Normal);

				// write new content back to file
				using (var writer = new StreamWriter(path, false, Encoding.UTF8))
				{
					writer.Write(content);
				}
			}

			return result;
		}

		public static List<SearchResults> SearchInFiles(string root, string filePattern, string search)
		{
			var results = new List<SearchResults>();

			IO.CheckRootAndPattern(ref root, ref filePattern);

			foreach (string file in IO.GetAllFiles(root, filePattern))
			{
				if (!IO.IsSVN(file))
				{
					SearchResults result = SearchInFile(file, search);
					if (result.Matches.Count > 0)
					{
						results.Add(result);
					}
				}
			}

			return results;
		}

		public static SearchResults SearchInFile(string path, string search)
		{
			var result = new SearchResults(path);
			string content = ReadFileAsUtf8(path);

			MatchCollection matches = Regex.Matches(content, search);

			foreach (Match match in matches)
			{
				var lineNumber = GetLineNumber(content, match.Index);
				result.Matches.Add(new SearchResultMatch(match, lineNumber));
			}

			return result;
		}

		#endregion

		public static Encoding ReadEncoding(string filepath)
		{
			if (String.IsNullOrEmpty(filepath))
			{
				throw new NullReferenceException("filepath was not specified");
			}

			if (!File.Exists(filepath))
			{
				throw new FileNotFoundException("filepath does not exist");
			}

			Encoding encoding;

			using (var reader = new StreamReader(filepath, true))
			{
				encoding = reader.CurrentEncoding;
			}

			ConsoleHelper.DebugWriteLine("current EncodingName: " + encoding.EncodingName.ToString());
			ConsoleHelper.DebugWriteLine("current BodyName: " + encoding.BodyName.ToString());
			ConsoleHelper.DebugWriteLine("current HeaderName: " + encoding.HeaderName.ToString());

			return encoding;
		}

		private static FileStream WaitForFile(string path, FileMode mode, FileAccess access, FileShare share)
		{
			// check if file is locked by another application
			for (int attempts = 0; attempts < 10; attempts++)
			{
				try
				{
					var fs = new FileStream(path, mode, access, share);

					fs.ReadByte();
					fs.Seek(0, SeekOrigin.Begin);

					return fs;
				}
				catch (IOException)
				{
					Thread.Sleep(50);
				}
			}

			return null;
		}

		public static byte[] ReadAllBytes(string path)
		{
			byte[] buffer;

			using (var fs = WaitForFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				buffer = new byte[fs.Length];
				fs.Read(buffer, 0, (int)fs.Length);
			}

			return buffer;
		}

		public static String ReadFileAsUtf8(string fileName)
		{
			Encoding encoding = Encoding.Default;
			String original = String.Empty;

			using (var sr = new StreamReader(fileName, Encoding.Default))
			{
				original = sr.ReadToEnd();
				encoding = sr.CurrentEncoding;
				sr.Close();
			}

			//if (encoding == Encoding.UTF8)
			//	return original;

			byte[] encBytes = encoding.GetBytes(original);
			byte[] utf8Bytes = Encoding.Convert(encoding, Encoding.UTF8, encBytes);
			return Encoding.UTF8.GetString(utf8Bytes);
		}

		public static string ReadFile(string filepath)
		{
			return ReadFileAsUtf8(filepath);
		}

		public static string[] ReadFileLines(string filepath)
		{
			return Regex.Split(ReadFile(filepath), "\r\n|\r|\n");
		}

		public static void WriteFileLines(string filepath, string[] lines)
		{
			var linesList = new List<string>();

			foreach (string line in lines)
			{
				linesList.Add(line);
			}

			WriteFileLines(filepath, linesList);
		}

		public static string LinesToString(string[] lines)
		{
			string newFile = "";
			string linebreak = "";

			foreach (string line in lines)
			{
				newFile += linebreak + line;
				linebreak = System.Environment.NewLine;
			}

			return newFile;
		}

		public static string LinesToString(List<string> lines)
		{
			return LinesToString(lines.ToArray());
		}

		public static void WriteFileLines(string filepath, List<string> lines)
		{
			WriteFile(filepath, LinesToString(lines));
		}

		public static void WriteFileLines(string filepath, List<string> lines, Encoding encoding)
		{
			WriteFile(filepath, LinesToString(lines), encoding);
		}

		public static void WriteFile(string filepath, string text, Encoding encoding)
		{
			if (!File.Exists(filepath) || text != ReadFile(filepath))
			{
				using (var writter = new StreamWriter(filepath, false, encoding))
				{
					writter.Write(text);
				}
			}
		}

		public static void WriteFile(string filepath, string text)
		{
			WriteFile(filepath, text, Encoding.UTF8);
		}

		public static void Encode(string filepath, Encoding encoding)
		{
			if (String.IsNullOrEmpty(filepath))
			{
				throw new NullReferenceException("filepath was not specified");
			}

			if (!File.Exists(filepath))
			{
				throw new FileNotFoundException("filepath does not exist");
			}

			Encoding currentEncoding = ReadEncoding(filepath);
			string content = "";

			// read file
			using (var reader = new StreamReader(filepath, true))
			{
				content = reader.ReadToEnd();
			}

			File.Delete(filepath);

			// write to file
			using (var writer = new StreamWriter(filepath, false, encoding))
			{
				ConsoleHelper.DebugWriteLine("after: " + writer.Encoding.ToString());
				writer.Write(content);
			}
		}

		public static string FindApplication(string exeName)
		{
			return FindApplication(exeName, ".");
		}

		public static string FindApplication(string exeName, string folder)
		{
			// check application name
			if (String.IsNullOrEmpty(exeName))
			{
				throw new NullReferenceException("application filename was not specified");
			}

			if (exeName.Length <= 4)
			{
				throw new InvalidDataException("application filename too short");
			}

			string extension = exeName.Substring(exeName.Length - 4, 4);

			if (extension != ".exe" && extension != ".com" && extension != ".bat")
			{
				throw new InvalidDataException("application filename must end in .exe, .com, or .bat");
			}

			// search in program files folders
			var files = new List<string>();

			if (Directory.Exists(IO.ProgramFilesx86 + folder + @"\"))
				files = IO.GetAllFiles(IO.ProgramFilesx86 + folder + @"\", exeName);
			if (files.Count == 0)
			{
				if (Directory.Exists(IO.ProgramFiles + folder + @"\"))
				{
					files = IO.GetAllFiles(IO.ProgramFiles + folder + @"\", exeName);
				}
			}

			// unable to locate application
			if (files.Count == 0)
			{
				throw new FileNotFoundException("unable to locate " + exeName);
			}

			return files[0];
		}

		public static string FindSystemApplication(string filename)
		{
			// check application name
			if (String.IsNullOrEmpty(filename))
			{
				throw new NullReferenceException("application filename was not specified");
			}

			if (filename.Length <= 4)
			{
				throw new InvalidDataException("application filename too short");
			}

			string extension = filename.Substring(filename.Length - 4, 4);

			if (extension != ".exe" && extension != ".com" && extension != ".bat")
			{
				throw new InvalidDataException("application filename must end in .exe, .com, or .bat");
			}

			// search in program files folders

			var files = IO.GetAllFiles(Environment.SystemDirectory + @"\", filename);

			if (files.Exists(f => f.EndsWith(filename)))
			{
				return files.Find(f => f.EndsWith(filename));
			}
			else
			{
				throw new FileNotFoundException("unable to locate " + filename);
			}
		}

		public static string FindFile(string root, string filepattern)
		{
			// check application name
			if (String.IsNullOrEmpty(filepattern))
			{
				throw new NullReferenceException("filename was not specified");
			}

			// check root
			if (String.IsNullOrEmpty(root))
			{
				throw new NullReferenceException("root was not specified");
			}

			IO.CheckRootAndPattern(ref root, ref filepattern);
			List<string> files = IO.GetAllFiles(root, filepattern);

			// unable to locate application
			if (files.Count == 0)
			{
				throw new FileNotFoundException("unable to locate " + filepattern);
			}

			return files[0];
		}

		public static string FindFile(string filepattern)
		{
			string root = Directory.GetCurrentDirectory();
			return FindFile(root, filepattern);
		}

		public static List<string> GetDotNetVersions()
		{
			// get list of msbuild versions availble
			string msbuildFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + @"\..\Microsoft.NET\Framework";
			var allVersions = new List<string>();

			foreach (string path in Directory.GetDirectories(msbuildFolder))
			{
				string version = path.Remove(0, path.LastIndexOf('\\') + 1).Substring(1, 3);
				string msBuildPath = path + @"\MSBuild.exe";
				string vbcBuildPath = path + @"\vbc.exe";
				string cbcBuildPath = path + @"\cbc.exe";

				if (File.Exists(msBuildPath) || File.Exists(vbcBuildPath) || File.Exists(cbcBuildPath))
				{
					allVersions.Add(version);
				}
			}

			return allVersions;
		}

		public static string FindDotNetFolder(string netVersion)
		{
			// get list of msbuild versions availble
			string msbuildFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + @"\..\Microsoft.NET\Framework";
			var msBuildFolders = new Dictionary<string, string>();

			foreach (string path in Directory.GetDirectories(msbuildFolder))
			{
				string version = path.Remove(0, path.LastIndexOf('\\') + 1).Substring(1, 3);
				string msBuildPath = path + @"\MSBuild.exe";
				string vbcBuildPath = path + @"\vbc.exe";
				string cbcBuildPath = path + @"\cbc.exe";

				if (File.Exists(msBuildPath) || File.Exists(vbcBuildPath) || File.Exists(cbcBuildPath))
				{
					msBuildFolders.Add(version, path);
					ConsoleHelper.DebugWriteLine(version + " = " + path);
				}
			}

			// sanity - .net version installed
			if (!msBuildFolders.ContainsKey(netVersion))
				throw new ArgumentOutOfRangeException(".net version");

			return msBuildFolders[netVersion];
		}

		public static FileInfo GetOldestFile(string directory)
		{
			if (!Directory.Exists(directory))
			{
				throw new DirectoryNotFoundException(directory);
			}

			var parent = new DirectoryInfo(directory);
			var files = parent.GetFiles();

			if (files.Length == 0)
			{
				return null;
			}

			FileInfo oldestFile = files[0];
			foreach (var file in files)
			{
				if (file.CreationTime < oldestFile.CreationTime)
				{
					oldestFile = file;
				}
			}

			return oldestFile;
		}

		public static FileInfo GetNewestFile(string directory)
		{
			if (!Directory.Exists(directory))
			{
				throw new DirectoryNotFoundException(directory);
			}

			var parent = new DirectoryInfo(directory);
			var files = parent.GetFiles();

			if (files.Length == 0)
			{
				return null;
			}

			FileInfo oldestFile = files[0];
			foreach (var file in files)
			{
				if (file.CreationTime > oldestFile.CreationTime)
				{
					oldestFile = file;
				}
			}

			return oldestFile;
		}

		public static void CopyFile(string sourcePath, string destinationPath)
		{
			if (File.Exists(destinationPath))
			{
				var toFile = new FileInfo(destinationPath);
				if (toFile.IsReadOnly)
					toFile.IsReadOnly = false;
			}

			var sourceFile = new FileInfo(sourcePath);
			sourceFile.CopyTo(destinationPath, true);

			var destinationFile = new FileInfo(destinationPath);
			if (destinationFile.IsReadOnly)
			{
				destinationFile.IsReadOnly = false;
				destinationFile.CreationTime = sourceFile.CreationTime;
				destinationFile.LastWriteTime = sourceFile.LastWriteTime;
				destinationFile.LastAccessTime = sourceFile.LastAccessTime;
				destinationFile.IsReadOnly = true;
			}
			else
			{
				destinationFile.CreationTime = sourceFile.CreationTime;
				destinationFile.LastWriteTime = sourceFile.LastWriteTime;
				destinationFile.LastAccessTime = sourceFile.LastAccessTime;
			}
		}

		public static void CopyFile(string path)
		{
			IO.CopyFile(path, -1);
		}

		public static void CopyFile(string path, int bytesPerSecond)
		{
			// check path
			if (String.IsNullOrEmpty(path))
			{
				throw new NullReferenceException("filepath was not specified");
			}

			// original file must exits
			if (!File.Exists(path))
			{
				throw new FileNotFoundException("file not found. \"" + path + "\"");
			}
		}

		public static long FileSize(string file)
		{
			long size;
			using (Stream inStream = File.OpenRead(file))
			{
				size = inStream.Length;
			}

			return size;
		}

		public static long FileSize(List<string> files)
		{
			long size = 0;
			foreach (string file in files)
			{
				size += IO.FileSize(file);
			}

			return size;
		}

		public static void DestroyFile(string path)
		{
			// file must exits
			if (!File.Exists(path))
			{
				throw new FileNotFoundException("file not found. \"" + path + "\"");
			}

			byte[] overwriteData = new byte[512];
			double writes = Math.Ceiling(new FileInfo(path).Length / (double)overwriteData.Length);

			RNGCryptoServiceProvider randomData = new RNGCryptoServiceProvider();

			File.SetAttributes(path, FileAttributes.Normal);
			FileStream stream = new FileStream(path, FileMode.Open);
			stream.Position = 0;

			for (int sectorsWritten = 0; sectorsWritten < writes; sectorsWritten++)
			{
				randomData.GetBytes(overwriteData);
				stream.Write(overwriteData, 0, overwriteData.Length);
			}

			stream.SetLength(0);
			stream.Close();

			DateTime dt = DateTime.Now.AddYears(21);
			File.SetCreationTime(path, dt);
			File.SetLastAccessTime(path, dt);
			File.SetLastWriteTime(path, dt);
			File.Delete(path);
		}

		private static void CopyFileSection(string source, string destination, int offset, int length, byte[] buffer)
		{
			using (Stream inStream = File.OpenRead(source))
			{
				using (Stream outStream = File.OpenWrite(destination))
				{
					inStream.Seek(offset, SeekOrigin.Begin);
					int bufferLength = buffer.Length, bytesRead;

					while (length > bufferLength && (bytesRead = inStream.Read(buffer, 0, bufferLength)) > 0)
					{
						outStream.Write(buffer, 0, bytesRead);
						length -= bytesRead;
					}

					while (length > 0 && (bytesRead = inStream.Read(buffer, 0, length)) > 0)
					{
						outStream.Write(buffer, 0, bytesRead);
						length -= bytesRead;
					}
				}
			}
		}

		public static bool WaitForFileSystem(string root, int timeout, bool expected)
		{
			bool found;
			Stopwatch timer = new Stopwatch();
			timer.Start();

			do
			{
				try
				{
					found = Directory.Exists(root);
				}
				catch
				{
					Thread.Sleep(200);
					found = false;
				}
			} while (found != expected && !(timer.ElapsedMilliseconds > timeout));

			return (found == expected);
		}

		#region byte array helpers

		public static byte[] Combine(byte[] first, byte[] second)
		{
			byte[] result = new byte[first.Length + second.Length];
			Buffer.BlockCopy(first, 0, result, 0, first.Length);
			Buffer.BlockCopy(second, 0, result, first.Length, second.Length);

			return result;
		}

		public static byte[] Combine(byte[] first, byte[] second, byte[] third)
		{
			byte[] result = new byte[first.Length + second.Length + third.Length];
			Buffer.BlockCopy(first, 0, result, 0, first.Length);
			Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
			Buffer.BlockCopy(third, 0, result, first.Length + second.Length, third.Length);
			return result;
		}

		public static byte[] Combine(params byte[][] arrays)
		{
			int size = 0;
			foreach (byte[] data in arrays)
			{
				size += data.Length;
			}

			byte[] result = new byte[size];
			int offset = 0;

			foreach (byte[] data in arrays)
			{
				Buffer.BlockCopy(data, 0, result, offset, data.Length);
				offset += data.Length;
			}

			return result;
		}

		public static byte[] Split(ushort u16)
		{
			byte lower = (byte)(u16 & 0xff);
			byte upper = (byte)(u16 >> 8);
			return new byte[] { upper, lower };
		}

		public static byte[] Split(uint u32)
		{
			ushort lower = (ushort)(u32 & 0xffff);
			ushort upper = (ushort)(u32 >> 16);

			return Combine(Split(upper), Split(lower));
		}

		public static byte[] SubSet(byte[] source, int first, int length)
		{
			byte[] result = new byte[length];
			Buffer.BlockCopy(source, first, result, 0, length);

			return result;
		}

		public static byte[] ReverseBytes(byte[] source)
		{
			Array.Reverse(source);
			return source;
		}

		public static byte[] SubSetReversed(byte[] source, int first, int length)
		{
			byte[] result = SubSet(source, first, length);
			Array.Reverse(result);
			return result;
		}

		#endregion

	}
}
