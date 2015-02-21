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
using System.Collections.Generic;
using System.Data.SqlClient;

namespace sar.Tools
{
	public enum SqlObjectType
	{
		FN,	// SQL scalar function
		IF,	// SQL inline table-valued function
		TF,	// SQL table-valued-function
		P,	// SQL Stored Procedure
		TR,	// SQL DML trigger
		U,	// Table (user-defined)
		TT,	// Table type
		V	// View
	}
	
	public class DatabaseObject
	{
		private const string LIST_ALL_OBJECTS = @"SELECT name, type FROM sys.objects WHERE type in ('FN', 'IF', 'P', 'TF', 'TR', 'TT', 'U', 'V')  AND name NOT LIKE 'sp[_]%' AND name NOT LIKE 'fn[_]%' AND name NOT LIKE 'sysdiagrams' ORDER BY type, name";
		private const string LIST_ALL_OBJECTS_2000 = @"SELECT name, xtype FROM sysobjects WHERE xtype in ('FN', 'IF', 'P', 'TF', 'TR', 'TT', 'U', 'V')  AND name NOT LIKE 'sp[_]%' AND name NOT LIKE 'fn[_]%' AND name NOT LIKE 'sysdiagrams' ORDER BY type, name";
		
		#region static
		
		public static List<DatabaseObject> GetDatabaseObjects(SqlConnection connection)
		{
			var result = new List<DatabaseObject>();
			
			using (var command = new SqlCommand(LIST_ALL_OBJECTS_2000, connection))
			{
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						string name = reader.GetString(0);
						var type = (SqlObjectType)Enum.Parse(typeof(SqlObjectType), reader.GetString(1));

						result.Add(new DatabaseObject(name, type));
					}
				}
			}
			
			return result;
		}
		
		#endregion
		
		#region members
		
		private string name;
		private SqlObjectType type;
		
		#endregion
		
		#region properties
		
		public string Name
		{
			get { return this.name; }
		}
		
		public string Type
		{
			get
			{
				/*
					FN,	// SQL scalar function
					IF,	// SQL inline table-valued function
					TF,	// SQL table-valued-function
					P,	// SQL Stored Procedure
					TR,	// SQL DML trigger
					U,	// Table (user-defined)
					TT,	// Table type
					V	// View
				 */
				
				
				switch (this.type)
				{
					case SqlObjectType.FN:
					case SqlObjectType.IF:
					case SqlObjectType.TF:
						return "Function";
					case SqlObjectType.P:
						return "StoredProcedure";
					case SqlObjectType.TR:
						return "Trigger";
					case SqlObjectType.U:
					case SqlObjectType.TT:
						return "Table";
					case SqlObjectType.V:
						return "View";
				}
				return this.name;
			}
		}

		#endregion

		
		private DatabaseObject(string name, SqlObjectType type)
		{
			this.name = name;
			this.type = type;
		}
		
		public string GetCreateScript(SqlConnection connection)
		{
			switch (this.type)
			{
				case SqlObjectType.TT:
				case SqlObjectType.U:
					// TODO: generate table
					return "CREATE TABLE " + name.QuoteSingle();

				default:
					string result = "";
					
					using (var command = new SqlCommand("sp_helptext " + this.name.QuoteSingle(), connection))
					{
						using (var reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								result += reader.GetString(0);
							}
						}
					}
					
					return result.TrimWhiteSpace();
			}
		}
	}
}
