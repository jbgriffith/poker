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
			string dbName = string.Format("[{0}]", Regex.Match(connString, @"(?<=catalog=)\w+(?=;)").Value); //added square brackets just in case.
			connString = Regex.Replace(connString, @"(?<=catalog=)\w+(?=;)", "master");

			using (SqlConnection conn = new SqlConnection(connString)) {
				conn.Open();
				string sqlCommandText = string.Format(@"IF db_id({0}) IS NOT NULL
															ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
															USE master DROP DATABASE {0};", dbName);
				SqlCommand sqlCommand = new SqlCommand(sqlCommandText, conn);
				try {
					sqlCommand.ExecuteNonQuery();
				}
				catch (Exception ex) {
					throw ex;
				}
				sqlCommandText = string.Format(@"
					CREATE DATABASE {0};
					ALTER AUTHORIZATION ON DATABASE::{0} TO sa;", dbName);
				sqlCommand = new SqlCommand(sqlCommandText, conn);
				sqlCommand.ExecuteNonQuery();
			}
		}
	}
}
