
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

using sar.Tools;

namespace sar.Controls
{
	public static class WindowsSettings
	{
		public static int ScrollBarSize
		{
			set
			{
				int size = value * -15;
				
				RegistryKey windowMetricsKey = Registry.LocalMachine.OpenSubKey(@"HKEY_CURRENT_USER\Control Panel\Desktop\WindowMetrics\", true);

				if (windowMetricsKey == null) throw new KeyNotFoundException("WindowMetrics key was not found");

				windowMetricsKey.SetValue("ScrollHeight", size, RegistryValueKind.String);
				windowMetricsKey.SetValue("ScrollWidth", size, RegistryValueKind.String);
				windowMetricsKey.Close();
			}
		}
	}
}
