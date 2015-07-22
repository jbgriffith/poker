using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using User;

using Poker.DbModels;

namespace Poker {
	class Program {
		
		static void Main(string[] args) {
			List<Player> AllPlayers = new List<Player>();
			var Overallsw = Stopwatch.StartNew();
			var sw = Stopwatch.StartNew();

			Console.WriteLine("Downloading & Creating users ...");
			for (int users = 0; users < 1; users++)
				AllPlayers.AddRange(User.UserData.GetUserData("http://api.randomuser.me/?results=10&key=6QGL-Z2YJ-OL4O-MACS"));
			
			Console.WriteLine("Created {0} users", AllPlayers.Count);
			Console.WriteLine("Time Elapsed: {0}", sw.Elapsed);

			Overallsw.Restart();
			for (int i = 0; i < 1; i++) {
				sw.Restart();
				//using (var pdb = new PokerContext()) {
				//	//pdb.Configuration.AutoDetectChangesEnabled = false;
				//	//pdb.Configuration.ValidateOnSaveEnabled = false;
				//	for (int g = 1; g < 2; g++) {
				//		Random r = new Random();
				//		//int range = 5;
				//		//int rInt = r.Next(2, range); //for ints
				//		int rInt = 5;

				//		// Add each player to the table
				//		var firstNPlayers = AllPlayers.Shuffle().Take(rInt).ToList();
				//		var nPlayers = new HashSet<Player>(firstNPlayers);
				//		//" HashSet should" speed up the RemoveAll http://stackoverflow.com/a/853551/3042939
				//		AllPlayers.RemoveAll(x => nPlayers.Contains(x));

				//		var game = new Game(nPlayers);

				//		game.Players.ForEach(x => {
				//			x.Games.Add(game);
				//			var dealtCards = game.Deck.DealCards(5);
				//			x.Hand.AddCards(dealtCards);
				//			x.Hand.Score = (int)EvaluatePokerHand.Score(x.Hand);
				//			Console.WriteLine(x.ToString());
				//			//if (x.Score > 0)
				//				Console.WriteLine((Poker.EvaluatePokerHand.PokerHand)x.Hand.Score);
				//		});



				//		var maxScore = game.Players.Max(x => x.Hand.Score);
				//		var playersWithMaxScore = game.Players.Where(x => x.Hand.Score == maxScore).ToList();
				//		var winner = playersWithMaxScore.First();
				//		Console.WriteLine("Winning Hand(s) is a {0}", (EvaluatePokerHand.PokerHand)maxScore);
				//		// playersWithMaxScore.SetScoreDetailsAndGetWinners();
				//		// instead of all of the crap below
				//		//
				//		if (playersWithMaxScore.Count > 1) {
							
				//			var blah = new List<List<int>>();
				//			playersWithMaxScore.ForEach(x => {
				//				x.Hand.ScoreDetails = EvaluatePokerHand.GetScoreDetail(x).ToList();
				//				//blah.Add(x.ScoreDetails);
				//				Console.WriteLine("{0} had {1}.", x.Name, String.Join(" ", x.Hand.ScoreDetails));
				//			});
				//			// find winning hand then reassign winner
				//			//playersWithMaxScore.
				//			//var blah2 = EnumerableExtensions.AllCombinationsOf(blah);
				//			//for (int b  = 0; b < blah2.Count; b++) {
								
				//			//}

				//			//var blah3 = blah[0];	
				//			//var blah4 = blah[1];
				//		}
				//		else {
				//			//Game.Winner.AddRange(playersWithMaxScore);
				//			//Game.WinningHand.AddRange(playersWithMaxScore.Select(p => p.cards));
				//			winner.Hand.ScoreDetails = EvaluatePokerHand.GetScoreDetail(winner).ToList();
				//			Console.WriteLine("{0} is a winner!!!!!", winner.Name);
				//		}

				//			//Entity Framework
				//		pdb.Games.Add(game);
				//		AllPlayers.AddRange(game.Players);
				//		//AllPlayers.AddRange(Players);

				//		if (g % 100 == 0) Console.WriteLine("Game #{0}", g);
				//	}

				//	Console.WriteLine("Time Elapsed: {0}", sw.Elapsed);
				//	sw.Restart();
				//	Console.WriteLine("Saving Changes");

				//	//EntityFramework
				//	pdb.SaveChanges();

				//	Console.WriteLine("All games have been saved");
				//	Console.WriteLine("Time Elapsed: {0}", sw.Elapsed);
				//	sw.Stop();
				//	//#if DEBUG
				//	//					Console.WriteLine("Press any key to close...");
				//	//					Console.ReadLine();
				//	//#endif
				//}
			}
			Console.WriteLine("Time Elapsed: {0}", Overallsw.Elapsed);
		}
	}
}
