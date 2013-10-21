/* Copyright (C) 2013 Kevin Boronka
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
	public class StringHelper
	{
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
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
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

		public static string TrimWhiteSpace(string input)
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
		
		public static string MillisecondsToString(long ms)
		{
			if (ms < 1500)
			{
				return ms.ToString() + " ms";
			}
			else
			{
				return MillisecondsToSecondsString(ms);
			}
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
		
		public static SecureString MakeSecureString(string text)
		{
			SecureString secure = new SecureString();
			foreach (char c in text)
			{
				secure.AppendChar(c);
			}

			return secure;
		}
		
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
		
		#region XML
		
		public static XmlReaderSettings ReaderSettings()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.CloseInput = true;
			settings.IgnoreComments = true;
			settings.IgnoreProcessingInstructions = true;
			settings.IgnoreWhitespace = true;
			return settings;
		}

		public static XmlWriterSettings WriterSettings()
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.CloseOutput = true;
			settings.Encoding = Encoding.UTF8;
			settings.Indent = true;
			settings.IndentChars = "\t";
			return settings;
		}
		
		#endregion
	}
}