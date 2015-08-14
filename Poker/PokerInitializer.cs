//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Poker.DbModels;

//namespace Poker {
//	class PokerInitializer : System.Data.Entity.DropCreateDatabaseAlways<PokerContext> {
//		protected override void Seed(PokerContext context) {
//			var AllPlayers = new List<Player>();
//			Console.WriteLine("Downloading & Creating users...");
//			for (int users = 0; users < 1; users++)
//				AllPlayers.AddRange(User.UserData.GetUserData("https://www.mockaroo.com/f67ffca0/download?count=1000&key=e1664fd0"));

//			foreach (var pl in AllPlayers)
//				context.Players.Add(pl);

//			context.SaveChanges();
//		}
//	}
//}
