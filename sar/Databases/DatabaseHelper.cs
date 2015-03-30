using System;

namespace sar.Tools
{
	public static class DatabaseHelper
	{
		public static string ToDBString(this DateTime value)
		{
			return "'" + value.ToString(FileLogger.ISO8601_TIMESTAMP) + "'";
		}
	}
}
