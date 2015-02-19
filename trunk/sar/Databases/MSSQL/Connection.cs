using System;

namespace sar.Databases.MSSQL
{
	public class ConnectionString
	{
		#region members

		private string server;
		private string database;
		private string username;
		private string password;

		#endregion

		#region constructor

		public ConnectionString(string server, string database, string username, string password)
		{
			this.server = server;
			this.database = database;
			this.username = username;
			this.password = password;
		}

		#endregion

		#region methods

		public override string ToString()
		{
			string connectionString = "";
			connectionString += "Data Source=" + this.server + ";";
			connectionString += "Initial Catalog=" + this.database + ";";
			connectionString += "User Id=" + this.username + ";";
			connectionString += "Password=" + this.password + ";";
			connectionString += "Application Name=" + sar.Tools.AssemblyInfo.Name + ";";
			
			return connectionString;
		}

		#endregion
	}
}
