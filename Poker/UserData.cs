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
				DateTime? dob = DateTime.Parse((string)ea.user.dob);
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
				catch (Exception ex) {
					Console.WriteLine(ex.Message);
					//throw;
				}
			}
			return result;
		}

	}
}
