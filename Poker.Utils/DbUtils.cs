using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Poker.Utils {
	public static class DbUtils {
		public static void DropCreateDB(string connString) {
			string dbName = string.Format("{0}", Regex.Match(connString, @"(?<=Catalog=)\w+(?=;)", RegexOptions.IgnoreCase).Value); //added square brackets just in case.
			connString = Regex.Replace(connString, @"(?<=Catalog=)\w+(?=;)", "master", RegexOptions.IgnoreCase);

			using (SqlConnection conn = new SqlConnection(connString)) {
				conn.Open();
				string sqlCommandText = string.Format(@"ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
														USE master DROP DATABASE {0};", dbName).Replace("\r\n", "").Replace("\t", "");
				SqlCommand sqlCommand = new SqlCommand(sqlCommandText, conn);
				try {
					sqlCommand.ExecuteNonQuery();
				}
				catch (Exception) { }
				sqlCommandText = string.Format(@"
					CREATE DATABASE {0};
					ALTER AUTHORIZATION ON DATABASE::{0} TO sa;", dbName).Replace("\r\n", " ").Replace("\t", "");
				sqlCommand = new SqlCommand(sqlCommandText, conn);
				sqlCommand.ExecuteNonQuery();
			}
		}
	}
}
