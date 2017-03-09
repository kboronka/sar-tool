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
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace sar.Tools
{
	public static class ExtensionMethods
	{
		public static T GetAttribute<T>(this Enum value) where T : Attribute {
			var type = value.GetType();
			var memberInfo = type.GetMember(value.ToString());
			var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
			return (T)attributes[0];
		}
		
		public static string Description(this Enum value) {
			var attribute = value.GetAttribute<DescriptionAttribute>();
			return attribute == null ? value.ToString() : attribute.Description;
		}
		
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
		
		public static string QuoteDouble(this string content)
		{
			return @"""" + content + @"""";
		}

		public static string QuoteSingle(this string content)
		{
			return @"'" + content + @"'";
		}
		
		public static bool IsNumeric(this char c)
		{
			return c.ToString().IsNumeric();
		}
		
		public static bool IsNumeric(this string s)
		{
			float output;
			return float.TryParse(s, out output);
		}

		public static string ToHTML(this string s)
		{
			return Regex.Replace(s, @"\n\r*", @"<br />");
		}
		
		public static bool IsNotNull(this string s)
		{
			return (!String.IsNullOrEmpty(s));
		}
		
		public static string TrimWhiteSpace(this string input)
		{
			if (input == null)
			{
				throw new NullReferenceException("input string is null");
			}
			
			string result = input;

			while (result.StartsWith("\n", StringComparison.InvariantCulture) ||
			       result.StartsWith("\r", StringComparison.InvariantCulture) ||
			       result.StartsWith(" ", StringComparison.InvariantCulture) ||
			       result.StartsWith("\t", StringComparison.InvariantCulture))
			{
				result = StringHelper.TrimStart(result);
			}
			
			while (result.EndsWith("\n", StringComparison.InvariantCulture) ||
			       result.EndsWith("\r", StringComparison.InvariantCulture) ||
			       result.EndsWith(" ", StringComparison.InvariantCulture) ||
			       result.EndsWith("\t", StringComparison.InvariantCulture))
			{
				result = StringHelper.TrimEnd(result);
			}
			
			return result;
		}

		public static int ToInt(this string s)
		{
			int output;
			return (int.TryParse(s, out output)) ? output : 0;
		}
		
		public static byte[] ToBytes(this string s)
		{
			return StringHelper.GetBytes(s);
		}
		
		public static string[] ToLines(this string s)
		{
			return s.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
			//return s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
		}
		
		public static string AppendPrefixTo(this string prefix, string text)
		{
			//string[] lines =
			string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			
			string result = "";
			
			foreach (var line in lines)
			{
				result += prefix + line + Environment.NewLine;
			}
			
			return result;
		}
		
		public static bool IsNotZero(this byte[] byteArray)
		{
			foreach (var b in byteArray)
			{
				if (b != 0) return true;
			}
			
			return false;
		}
		
		public static string ToCommaSeperatedValues(this int[] numbers)
		{
			try
			{
				var line = "";
				var delimiter = "";
				
				foreach (var number in numbers)
				{
					line += delimiter + number.ToString();
					delimiter = ", ";
				}
				
				return line;
			}
			catch
			{
				return "";
			}
		}
		
		public static string ToCSV(this IEnumerable<string> list)
		{
			var line = "";
			var delimitor = "";
			foreach (var item in list)
			{
				line += delimitor + item;
				delimitor = ", ";
			}

			return line;
		}
		
		
		/// <summary>
		///  Determine whether a socket is still connected
		/// </summary>		
		public static bool IsConnected(this TcpClient socket)
		{
			// solution posted by Carsten
			// http://stackoverflow.com/questions/7650402/how-to-test-for-a-broken-connection-of-tcpclient-after-being-connected
			
			var blockingState = socket.Client.Blocking;
			
			try
			{
				var tmp = new byte[] {};

				socket.Client.Blocking = false;
				socket.Client.Send(tmp, 0, 0);
				return socket.Connected;
			}
			catch (SocketException e)
			{
				const int WSAEWOULDBLOCK = 10035;
				if (e.NativeErrorCode.Equals(WSAEWOULDBLOCK))
				{
					return socket.Connected;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				socket.Client.Blocking = blockingState;
			}
		}
	}
}
