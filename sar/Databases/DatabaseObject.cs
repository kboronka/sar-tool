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
using System.Data.SqlClient;
using System.Text;

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
		#region static
		
		public static List<DatabaseObject> GetDatabaseObjects(SqlConnection connection)
		{
			var result = new List<DatabaseObject>();
			var sql = Encoding.ASCII.GetString(EmbeddedResource.Get(@"sar.Databases.MSSQL.GetObjects.sql"));
			using (var command = new SqlCommand(sql, connection))
			{
				command.CommandTimeout = 600; 	// 10 minutes
				
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
		
		public static DatabaseObject GetDatabaseObject(SqlConnection connection, string name)
		{
			var result = new List<DatabaseObject>();
			
			using (var command = new SqlCommand(@"SELECT name, xtype FROM sysobjects WHERE name = " + name.QuoteSingle(), connection))
			{
				command.CommandTimeout = 600;	// 10 minutes
				
				using (var reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						var type = (SqlObjectType)Enum.Parse(typeof(SqlObjectType), reader.GetString(1));
						return new DatabaseObject(reader.GetString(0), type);
					}
					else
					{
						return null;
						//throw new MissingMemberException("object " + name + " not found");
					}
				}
			}
		}
		
		public static List<string> GetColumnNames(SqlConnection connection, string table)
		{
			var result = new List<string>();
			
			using (var command = new SqlCommand(@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N" + table.QuoteSingle(), connection))
			{
				command.CommandTimeout = 600;	// 10 minutes
				
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						result.Add(reader.GetString(0));
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
						return "Table";
					case SqlObjectType.TT:
						return "UserDefinedTableType";
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
			string result = "";
			
			switch (this.type)
			{
				case SqlObjectType.TT:
					var createTableTypeScript = Encoding.ASCII.GetString(EmbeddedResource.Get(@"sar.Databases.MSSQL.CreateTableType.sql"));
					createTableTypeScript = createTableTypeScript.Replace(@"%%TableTypeName%%", this.name);
					
					using (var command = new SqlCommand(createTableTypeScript, connection))
					{
						command.CommandTimeout = 600;	// 10 minutes
						
						using (var reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								result += reader.GetString(0) + Environment.NewLine;
							}
						}
					}
					
					return result.TrimWhiteSpace();
					
				case SqlObjectType.U:
					var createTableScript = EmbeddedResource.Get(@"sar.Databases.MSSQL.CreateTable.sql");
					var script = Encoding.ASCII.GetString(createTableScript);
					script = script.Replace(@"%%TableName%%", this.name);
					
					using (var command = new SqlCommand(script, connection))
					{
						command.CommandTimeout = 600;	// 10 minutes
						
						using (var reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								result += reader.GetString(0) + Environment.NewLine;
							}
						}
					}
					
					return result.TrimWhiteSpace();

				default:
					using (var command = new SqlCommand("sp_helptext " + this.name.QuoteSingle(), connection))
					{
						command.CommandTimeout = 600;	// 10 minutes
						
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
		
		public string GetInsertScript(SqlConnection connection)
		{
			string result = "";
			
			switch (this.type)
			{
				case SqlObjectType.TT:
				case SqlObjectType.U:
					var sproc = Encoding.ASCII.GetString(EmbeddedResource.Get(@"sar.Databases.MSSQL.GenerateInsert.sql"));
					foreach (var sql in DatabaseHelper.SplitByGO(sproc))
					{
						using (var command = new SqlCommand(sql, connection))
						{
							command.CommandTimeout = 600;	// 10 minutes
							command.ExecuteNonQuery();
						}
					}
					
					var script = "";
					script += "EXECUTE ";
					script += " dbo.GenerateInsert @ObjectName = N'" + this.name + "'";
					script += " ,@PrintGeneratedCode=0";
					script += " ,@GenerateProjectInfo=0";
					
					result = @"IF NOT EXISTS (SELECT TOP 1 * FROM [" + this.name + "])" + Environment.NewLine;
					
					using (var command = new SqlCommand(script, connection))
					{
						command.CommandTimeout = 600;	// 10 minutes
						
						using (var reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								result += reader.GetString(0) + Environment.NewLine;
							}
						}
					}
					
					// drop sproc
					using (var command = new SqlCommand(@"DROP PROCEDURE dbo.GenerateInsert;", connection))
					{
						command.CommandTimeout = 600;	// 10 minutes
						command.ExecuteNonQuery();
					}
					
					return result.TrimWhiteSpace();

				default:
					throw new ApplicationException("object is not a table");
			}
		}
	}
}
