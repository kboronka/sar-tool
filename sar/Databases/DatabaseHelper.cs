using System;

namespace sar.Tools
{
	public static class DatabaseHelper
	{
		public static string ToDBString(this DateTime value)
		{
			if (value == DateTime.MaxValue)
			{
				return "'2990-01-01T00:00:00.000'";
			}
			else if (value == DateTime.MinValue)
			{
				return "'1990-01-01T00:00:00.000'";
			}
			else
			{
				return "'" + value.ToString(FileLogger.ISO8601_TIMESTAMP) + "'";
			}
		}
	}
}
