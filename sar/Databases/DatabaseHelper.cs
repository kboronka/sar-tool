/* Copyright (C) 2016 Kevin Boronka
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
		
		public static List<string> SplitByGO(string sql)
		{
			var lines = sql.ToLines();
			var result = new List<string>();
			var newSql = "";
			
			foreach (var line in lines)
			{
				if (line.ToUpperInvariant() == "GO")
				{
					result.Add(newSql.TrimWhiteSpace());
					newSql = "";
				}
				else
				{
					newSql += line + Environment.NewLine;
				}
			}
			
			return result;
		}
	}
}
