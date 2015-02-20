using Newtonsoft.Json.Linq;
using Poker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserData {

	public static class UserData {

		public static List<Player> GetUserData(string Url) {
			var Players = new List<Player>();
			DateTime now = DateTime.Today;

			using (var webClient = new System.Net.WebClient()) {
				var jsonString = webClient.DownloadString(Url);

				dynamic json = JValue.Parse(jsonString);

				foreach (dynamic p in json.results) {
					var ageInYears = now.Year - ConvertFromUnixTS((double)p.user.dob).Year;
					var fullName = String.Format("{0} {1}", p.user.name.first, p.user.name.last);

					Players.Add(new Player(fullName, ageInYears));
				}

			}
			return Players;
		}

		public static DateTime ConvertFromUnixTS(double timestamp) {
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return origin.AddSeconds(timestamp);
		}

	}

}
