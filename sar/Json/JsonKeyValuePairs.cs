/* Copyright (C) 2017 Kevin Boronka
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

using sar.Http;
using sar.Tools;

namespace sar.Json
{
	/// <summary>
	/// Dictionary extension object.
	/// </summary>
	public class JsonKeyValuePairs : Dictionary<string, object>
	{
		public JsonKeyValuePairs() : base()
		{
			
		}
		
		public JsonKeyValuePairs(string json) : base()
		{
			var depth = 0;
			var stringDepth = 0;
			
			var keyStart = -1;
			var keyEnd = -1;
			var key = "";
			
			var valueStart = -1;
			var valueEnd = -1;
			var value = "";
			
			for (var i = 0; i < json.Length; i++)
			{
				var c = json[i];
				
				if (c == '{' && stringDepth == 0)
				{
					depth++;
				}
				else if (c == '[' && depth > 0  && stringDepth == 0)
				{
					depth++;
				}
				else if (c == ']' && depth > 0 && stringDepth == 0)
				{
					depth--;
				}
				else if (c == '"' && depth > 0 && stringDepth == 0)
				{
					stringDepth++;
					
					if (keyStart == -1 && depth == 1)
					{
						// start of key
						keyStart = i;
					}
				}
				else if (c == '"' && depth > 0 && stringDepth == 1)
				{
					stringDepth--;
					
					if (keyEnd == -1 && depth == 1)
					{
						// end of a key
						keyEnd = i;
						key = JsonHelper.TrimJsonString(json.Substring(keyStart, keyEnd - keyStart + 1));
					}
				}
				else if (c == ':' && stringDepth == 0)
				{
					if (valueStart == -1 && depth == 1)
					{
						// start of value
						valueStart = i + 1;
					}
				}
				else if (c == ',' && stringDepth == 0)
				{
					if (valueEnd == -1 && depth == 1)
					{
						// end of value
						valueEnd = i - 1;
						value = json.Substring(valueStart, valueEnd - valueStart + 1);
						this.Add(key, JsonHelper.ValueToObject(value));
						
						// prep for next key
						keyStart = -1;
						keyEnd = -1;
						key = "";
						valueStart = -1;
						valueEnd = -1;
						value = "";
					}
				}
				else if (c == '}' && stringDepth == 0)
				{
					if (depth == 1)
					{
						valueEnd = i - 1;
						value = json.Substring(valueStart, valueEnd - valueStart + 1);
						
						this.Add(key, JsonHelper.ValueToObject(value));
					}
					
					depth--;
				}
			}
			
			if (stringDepth != 0 && depth != 0)
			{
				throw new ApplicationException("Invalid json string");
			}
		}
		
		public bool ValidateStringKey(string key)
		{
			if (!this.ContainsKey(key))
			{
				return false;
			}
			
			return (this[key] is string);
		}
		
		public bool ValidateIntKey(string key)
		{
			if (!this.ContainsKey(key))
			{
				return false;
			}
			
			return (this[key] is int);
		}
		
		public bool ValidateBoolKey(string key)
		{
			if (!this.ContainsKey(key))
			{
				return false;
			}
			
			return (this[key] is bool);
		}

		public bool ValidateArrayKey(string key)
		{
			if (!this.ContainsKey(key))
			{
				return false;
			}
			
			return (this[key] is JsonArray);
		}
		
		public bool ValidateObjectKey(string key)
		{
			if (!this.ContainsKey(key))
			{
				return false;
			}
			
			return (this[key] is JsonKeyValuePairs);
		}
		
		public override string ToString()
		{
			var text = "";
			var delimitor = "";
			
			foreach (var key in this.Keys)
			{
				text += delimitor + key;
				delimitor = ", ";
			}
			
			return "[" + text + "]";
		}
	}
}
