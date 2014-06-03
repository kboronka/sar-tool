
using System;
using System.Reflection;

namespace sar.Tools
{
	public static class ObjectHelper
	{
		public static string GetName(Object sender)
		{
			Type type = sender.GetType();
			PropertyInfo property = type.GetProperty("Name");
			return (string)property.GetValue(sender, null);
		}
	}
}
