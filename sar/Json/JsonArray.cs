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
