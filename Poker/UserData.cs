using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Poker.DbModels;
using Poker;

using Newtonsoft.Json.Linq;

namespace User {

	public static class UserData {

		public static List<Player> GetUserData(string url) {
			var players = new List<Player>();
			DateTime now = DateTime.Today;
			var jsonString = GetJsonFromUrl(url);
			dynamic jsonObjects = JArray.Parse(jsonString);

			foreach (dynamic ea in jsonObjects) {
				DateTime? dob = DateTime.Parse((string)ea.user.dob); // 1; //now.Year - ea.user.dob.Year;
				var fullName = String.Format("{0} {1}", ea.user.name.first, ea.user.name.last);
				players.Add(new Player(fullName, dob));
			}
			return players;
		}

		public static string GetJsonFromUrl(string url) {
			var result = "";
			using (var webClient = new System.Net.WebClient()) {
				try {
					result = webClient.DownloadString(url);
				}
				catch (Exception) {
					throw;
				}
			}
			return result;
		}

		public static DateTime ConvertFromUnixTS(double timestamp) {
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return origin.AddSeconds(timestamp);
		}

	}

}
