using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

using sar.Base;
using sar.Databases.MSSQL;
using sar.Tools;

namespace sar.Commands
{
	public class MSSQL_GenerateScripts : Command
	{
		public MSSQL_GenerateScripts(Base.CommandHub parent) : base(parent, "MSSQL - Generate Scripts",
		                               new List<string> { "mssql-gs" },
		                               "-mssql-gs [server] [database] [username] [password] [destination]",
		                               new List<string> { "-mssql-gs 192.168.0.44 TestDB sa root " + @".\databasescripts\".QuoteDouble() })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 6)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			string command = args[0];
			string server = args[1];
			string database = args[2];
			string username = args[3];
			string password = args[4];
			string root = args[5];
			
			root = IO.CheckRoot(root);
			if (!Directory.Exists(root)) Directory.CreateDirectory(root);
			
			var connectionString = new ConnectionString(server, database, username, password);
			int objectCounter = 0;

			using (var connection = new SqlConnection(connectionString.ToString()))
			{
				connection.Open();
					
				Progress.Message = "Generating Scripts";
				foreach (DatabaseObject databaseObject in DatabaseObject.GetDatabaseObjects(connection))
				{
					string filename = databaseObject.Type + "." + databaseObject.Name + ".sql";
					
					Progress.Message = "Saving Script " + filename;
					
					IO.WriteFile(root + filename, databaseObject.GetCreateScript(connection));
					objectCounter++;
				}
				
				connection.Close();
			}
			
			ConsoleHelper.WriteLine("Generated " + objectCounter.ToString() + " Script" + (objectCounter == 1 ? "" : "s"), ConsoleColor.DarkYellow);
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}