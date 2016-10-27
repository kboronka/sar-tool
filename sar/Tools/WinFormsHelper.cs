using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace sar.Tools
{
	public static class WinFormsHelper
	{
		// source: http://stackoverflow.com/a/29497681/259712
		public delegate void InvokeIfRequiredDelegate<T>(T obj) where T : ISynchronizeInvoke;

		public static void InvokeIfRequired<T>(this T obj, InvokeIfRequiredDelegate<T> action) where T : ISynchronizeInvoke
		{
			if (obj.InvokeRequired)
			{
				obj.Invoke(action, new object[] { obj });
			}
			else
			{
				action(obj);
			}
		}
	}
}
