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
using System.Data.SqlClient;
using System.IO;
using System.Text;

using sar.Base;
using sar.Databases.MSSQL;
using sar.Tools;

namespace sar.Commands
{
	public class MSSQL_GenerateInsert : Command
	{
		public MSSQL_GenerateInsert(Base.CommandHub parent) : base(parent, "MSSQL - Generate Insert",
		                                                           new List<string> { "mssql-gi" },
		                                                           "-mssql-gi <server> <database> <username> <password> <table> [destination]",
		                                                           new List<string> { "-mssql-gs 192.168.0.44 TestDB sa root TableXYZ" + @".\databasescripts\".QuoteDouble(),
		                                                           	"-mssql-gi 192.168.0.44 TestDB sa root TableXYZ" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length < 6 || args.Length > 7)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			//string command = args[0];
			string server = args[1];
			string database = args[2];
			string username = args[3];
			string password = args[4];
			string tableName = args[5];
			string root = @".\";
			
			if (args.Length == 7) root = args[6];
			
			root = IO.CheckRoot(root);
			if (!Directory.Exists(root)) Directory.CreateDirectory(root);
			
			var connectionString = new ConnectionString(server, database, username, password);

			Progress.Message = "Opening SQL Connection";
			using (var connection = new SqlConnection(connectionString.ToString()))
			{
				connection.Open();
				var table = DatabaseObject.GetDatabaseObject(connection, tableName);
				
				Progress.Message = "Getting Insert Script";
				var insertScript = table.GetInsertScript(connection);
				
				connection.Close();
				
				Progress.Message = "Saving Insert Script to file";
				var filename = "TableInsert." + tableName + ".sql";
				File.WriteAllText(root + filename, insertScript);
			}
			
			ConsoleHelper.WriteLine("Generated Insert Script", ConsoleColor.DarkYellow);
			
			return ConsoleHelper.EXIT_OK;
		}
	}
}