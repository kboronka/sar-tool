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
using System.IO;
using Microsoft.Win32;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class WindowsShellReplacement : Command
	{
		public WindowsShellReplacement(Base.CommandHub parent) : base(parent, "Windows - Shell Replacement", new List<string> { "windows.shell", "win.shell" },
		                                                              @"-win.shell <filepath>",
		                                                              new List<string>() { @"-win.shell c:\shell\shell.exe" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string path = args[1];
			
			if (!File.Exists(path)) throw new FileNotFoundException("unable to find file: " + path);
			
			RegistryKey winLoginKey;
			if (ApplicationInfo.IsWow64)
			{
				winLoginKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
				ConsoleHelper.WriteLine("64bit");
			}
			else
			{
				winLoginKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
				ConsoleHelper.WriteLine("32bit");
			}

			if (winLoginKey == null) throw new KeyNotFoundException("Winlogin key was not found");

			winLoginKey.SetValue("Shell", path, RegistryValueKind.String);
			ConsoleHelper.WriteLine((string)winLoginKey.GetValue("Shell", path));
			winLoginKey.Close();

			ConsoleHelper.WriteLine("Shell set to " + path + "", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
