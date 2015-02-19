/* Copyright (C) 2015 Kevin Boronka
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;
using System.Xml;

namespace sar.Tools
{
	public static class StringHelper
	{
		public const string TAB = "\t";
		
		public static string RegexFindString(string input, string pattern)
		{
			Match match = Regex.Match(input, pattern);
			
			if (match.Success)
			{
				if (match.Groups.Count > 1)
				{
					return TrimWhiteSpace(match.Groups[1].Value);
				}
				else
				{
					return TrimWhiteSpace(match.Groups[0].Value);
				}
			}
			else
			{
				return null;
			}
		}
		
		public static byte[] GetBytes(string input)
		{
			byte[] bytes = new byte[input.Length * sizeof(char)];
			System.Buffer.BlockCopy(input.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		public static string GetString(byte[] bytes)
		{
			/*
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
			 */
			return System.Text.Encoding.UTF8.GetString(bytes);
		}
		
		public static string TrimStart(string input)
		{
			return TrimStart(input, 1);
		}
		
		public static string TrimStart(string input, int characters)
		{
			if (String.IsNullOrEmpty(input))
			{
				throw new NullReferenceException("input string is null");
			}
			
			if (characters > input.Length) characters = input.Length;
			
			return input.Substring(characters);
		}
		
		public static string TrimEnd(string input)
		{
			return TrimEnd(input, 1);
		}
		
		public static string TrimEnd(string input, int characters)
		{
			if (String.IsNullOrEmpty(input))
			{
				throw new NullReferenceException("input string is null");
			}
			
			if (characters > input.Length) characters = input.Length;
			
			return input.Substring(0, input.Length - characters);
		}
		
		public static string RemoveEmptyLines(string input)
		{
			// empty lines
			input = Regex.Replace(input, @"\n\r*\s*\n\r*", Environment.NewLine);
			
			// first line empty
			input = Regex.Replace(input, @"^\s*\n\r*", "");
			
			// last line empty
			input = Regex.Replace(input, @"\n\r*$", "");
			
			return input;
		}
		
		public static string Remove(string input, List<string> words)
		{
			string result = input;
			
			foreach (string word in words)
			{
				while (result/*.ToLower()*/.Contains(word/*.ToLower()*/))
				{
					//int i = result.ToLower().IndexOf(word.ToLower())
					
					result = result.Replace(word, "");
				}
			}

			return result;
		}
		
		public static string Remove(string input, string word)
		{
			return Remove(input, new List<string>() { word });
		}
		
		public static string FirstWord(string input)
		{
			if (string.IsNullOrEmpty(input)) return null;
			int characters = 0;
			
			for (int i = 0; i < input.Length; i++)
			{
				switch (input[i])
				{
					case '(':
					case ' ':
					case '\n':
					case '\r':
					case '\t':
						if (characters == 0) continue;
						return input.Substring(0, i);
					default:
						characters++;
						break;
				}
			}
			
			return input;
		}
		
		public static string LastWord(string input)
		{
			if (string.IsNullOrEmpty(input)) return null;
			int characters = 0;
			
			for (int i = input.Length - 1; i > 0; i--)
			{
				switch (input[i])
				{
					case ')':
					case ' ':
					case '\n':
					case '\r':
					case '\t':
						if (characters == 0) continue;
						return input.Substring(i, input.Length - i);
					default:
						characters++;
						break;
				}
			}
			
			return input;
		}

		
		public static bool StartsWith(string input, List<string> words)
		{
			foreach (string word in words)
			{
				if (input.ToLower() == word.ToLower()) return true;
				if (input.ToLower().StartsWith(word.ToLower() + " ")) return true;
			}
			
			return false;
		}
		
		public static bool EndsWith(string input, List<string> words)
		{
			foreach (string word in words)
			{
				if (input.ToLower() == word.ToLower()) return true;
				if (input.ToLower().EndsWith(" " + word.ToLower())) return true;
			}
			
			return false;
		}
		
		public static string[] ParseString(string input, string seperator)
		{
			List<String> array = new List<string>();
			
			string pervious = "";
			foreach (string substring in input.Split(new string[] { seperator }, StringSplitOptions.None))
			{
				if (!String.IsNullOrEmpty(substring))
				{
					if ((substring.Substring(0,1) == "\"" || pervious.Length > 0) && substring.Substring(substring.Length - 1,1) != "\"")
					{
						pervious = pervious + substring + " ";
					}
					else
					{
						string nextsubstring = pervious + substring;
						
						if (nextsubstring.Substring(0, 1) == "\"") nextsubstring = StringHelper.TrimStart(nextsubstring);
						if (nextsubstring.Substring(nextsubstring.Length - 1, 1) == "\"") nextsubstring = StringHelper.TrimEnd(nextsubstring);
						
						array.Add(nextsubstring);
						pervious = "";
					}
				}
			}
			
			return array.ToArray();
		}
		
		public static string AddQuotes(string input)
		{
			return "\"" + input + "\"";
		}
		
		public static string MillisecondsToSecondsString(long ms)
		{
			float sec = (float)ms / 1000f;
			
			if (sec < 10)
			{
				return sec.ToString("0.0") + " s";
			}
			else
			{
				return sec.ToString("0") + " s";
			}
		}
		
		public static string MillisecondsToString(long ms)
		{
			float seconds = (float)ms / 1000f;
			float minutes = (float)seconds / 60f;
			float hours = (float)minutes / 60f;
			
			if (seconds < 90)
			{
				return MillisecondsToSecondsString(ms);
			}
			else if (minutes < 90)
			{
				return minutes.ToString("0") + " minutes";
			}
			else
			{
				return hours.ToString("0.0") + " hours";
			}
		}
		
		public static SecureString MakeSecureString(string text)
		{
			SecureString secure = new SecureString();
			foreach (char c in text)
			{
				secure.AppendChar(c);
			}

			return secure;
		}
		
		public static byte[] CombineByteArrays( params byte[][] arrays )
		{
			int sum = 0;
			int offset = 0;
			
			foreach (byte[] array in arrays)
			{
				sum += array.Length;
			}
			
			byte[] result = new byte[sum];

			
			foreach ( byte[] array in arrays )
			{
				System.Buffer.BlockCopy(array, 0, result, offset, array.Length);
				offset += array.Length;
			}
			
			return result;
		}
		
		public static string[] CsvParser(string csvText)
		{
			List<string> columns = new List<string>();

			int last = -1;
			int current = 0;
			bool inQuotes = false;

			while(current < csvText.Length)
			{
				switch(csvText[current])
				{
					case '"':
						inQuotes = !inQuotes;
						break;
					case ',':
						if (!inQuotes)
						{
							columns.Add(csvText.Substring(last + 1, (current - last)).Trim(' ', ','));
							last = current;
						}
						
						break;
					default:
						break;
				}
				
				current++;
			}

			if (last != csvText.Length - 1)
			{
				columns.Add(csvText.Substring(last+1).Trim());
			}

			return columns.ToArray();
		}
		
		#region string extensions
		
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

			while (result.StartsWith("\n") || result.StartsWith("\r") || result.StartsWith(" ") || result.StartsWith("\t"))
			{
				result = TrimStart(result);
			}
			
			while (result.EndsWith("\n") || result.EndsWith("\r") || result.EndsWith(" ") || result.EndsWith("\t"))
			{
				result = TrimEnd(result);
			}
			
			return result;
		}

		public static int ToInt(this string s)
		{
			int output;
			if (int.TryParse(s, out output))
			{
				return output;
			}
			
			return 0;
		}
		
		public static byte[] ToBytes(this string s)
		{
			return GetBytes(s);
		}
		
		#endregion
		
		#region environment variable helpers
		
		
		public static void AllVariables(EnvironmentVariableTarget target)
		{
			foreach (DictionaryEntry var in Environment.GetEnvironmentVariables(target))
			{
				string key = (string)var.Key;
				string value = (string)var.Value;
				ConsoleHelper.WriteLine(key + ": " + value);
			}
		}
		
		
		#endregion
	}
}