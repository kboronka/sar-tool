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
