﻿/* Copyright (C) 2017 Kevin Boronka
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

using Microsoft.Win32;
using sar.Base;
using sar.Tools;
using System;
using System.Collections.Generic;

namespace sar.Commands
{
	public class WindowsAutoLogin : Command
	{
		public WindowsAutoLogin(Base.CommandHub parent) : base(parent, "Windows - Set AutoLogin", new List<string> { "windows.autologin", "win.autologin" },
															   @"-windows.autologin [domain\username] [password]",
															   new List<string>() { @"-windows.autologin ./Username Password",
																   @"-windows.autologin mydomain\username Password"})
		{

		}

		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 3)
			{
				throw new ArgumentException("incorrect number of arguments");
			}

			string username;
			string password;
			string domain;

			username = args[1];
			password = args[2];
			domain = "";

			//TODO: split username by '/' to extract domain

			RegistryKey localKey;

			if (ApplicationInfo.IsWow64)
			{
				ConsoleHelper.DebugWriteLine("is wow64");
				localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
			}
			else
			{
				ConsoleHelper.DebugWriteLine("is 32bit");
				localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
			}

			RegistryKey winLoginKey = localKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
			if (winLoginKey == null)
				throw new KeyNotFoundException("Winlogin key was not found");

			winLoginKey.SetValue("DefaultUserName", username, RegistryValueKind.String);
			winLoginKey.SetValue("DefaultPassword", password, RegistryValueKind.String);
			if (!String.IsNullOrEmpty(domain))
				winLoginKey.SetValue("DefaultDomainName", domain, RegistryValueKind.String);

			var autoAdminLogon = !String.IsNullOrEmpty(username) ? "1" : "0";
			winLoginKey.SetValue("AutoAdminLogon", autoAdminLogon, RegistryValueKind.String);
			winLoginKey.Close();

			ConsoleHelper.WriteLine("AutoLogin set to " + username + ".", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
