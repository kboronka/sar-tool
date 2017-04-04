using System;
using System.Collections.Generic;

namespace sar.Json
{
	/// <summary>
	/// Description of JsonArray.
	/// </summary>
	public class JsonArray : List<object>
	{
		public JsonArray() : base()
		{
			
		}
		
		public JsonArray(string json) : base()
		{
			var depth = 0;
			var stringDepth = 0;
			
			var valueStart = -1;
			var valueEnd = -1;
			var value = "";
			
			for (var i = 0; i < json.Length; i++)
			{
				var c = json[i];
				

				if (c == '[' && stringDepth == 0)
				{
					depth++;
					
					if (depth == 1)
					{
						valueStart = i + 1;
					}
				}
				else if (c == ']' && stringDepth == 0)
				{
					if (depth == 1)
					{
						valueEnd = i - 1;
						value = json.Substring(valueStart, valueEnd - valueStart + 1);
						this.Add(JsonHelper.ValueToObject(value));
					}
					
					depth--;
				}
				else if (c == ',' && depth == 1 && stringDepth == 0)
				{
					if (depth == 1)
					{
						valueEnd = i - 1;
						value = json.Substring(valueStart, valueEnd - valueStart + 1);
						this.Add(JsonHelper.ValueToObject(value));
						valueStart = i + 1;
					}
				}
				else if (c == '{' && stringDepth == 0)
				{
					depth++;
				}
				else if (c == '}' && stringDepth == 0)
				{
					depth--;
				}
				else if (c == '"' && depth > 0 && stringDepth == 0)
				{
					stringDepth++;
				}
				else if (c == '"' && depth > 0 && stringDepth == 1)
				{
					stringDepth--;
				}
			}
			
			if (stringDepth != 0 && depth != 0)
			{
				throw new ApplicationException("Invalid json string");
			}
		}
	}
}
