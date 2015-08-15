using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Poker.DbModels;

namespace Poker {
	class Program {
		static void Main(string[] args) {

			var numGames = 1000;
			var actualNumGames = 0;
			var Overallsw = Stopwatch.StartNew();
			var sw = Stopwatch.StartNew();
			//using (var db = new PokerContext()) {
			//	db.Database.Initialize(false);


			Overallsw.Restart();
			sw.Restart();
			//}

			for (int oG = 1; oG < 5 + 1; oG++) {

				sw.Restart();
				using (var db = new PokerContext()) {

					var allPlayers = db.Players.Take(100).ToList();

					for (int g = 1; g < numGames + 1; g++) {
						actualNumGames++;
						//Random r = new Random();
						//int range = 7;
						//int rInt = r.Next(2, range); //for ints
						int rInt = 7;

						// Add players to current game
						var firstNPlayers = allPlayers.Shuffle().Take(rInt);
						var nPlayers = new HashSet<Player>(firstNPlayers);  // HashSet should speed up the RemoveAll http://stackoverflow.com/a/853551/3042939
						allPlayers.RemoveAll(x => nPlayers.Contains(x));

						var game = new Game(nPlayers);

						foreach (var player in game.Players) {
							player.CurrentHand = new Hand(game.Deck.DealCards(5), player);
							player.CurrentHand.CalculateScore();
						}
						game.Deck.BurnDeck(); // no need to save the remaining cards in the deck.

							var maxScore = game.Players.Max(x => x.CurrentHand.Score);
							var playersWithMaxScore = game.Players.Where(x => x.CurrentHand.Score == maxScore).ToList();

							// need to create a function instead of all of the crap below
							var ScoreDetailsMatrix = new List<List<int>>();
							foreach (var player in playersWithMaxScore) {
								player.CurrentHand.SetScoreDetails();
								ScoreDetailsMatrix.Add(player.CurrentHand.ScoreDetail.ToList());
							}

							if (playersWithMaxScore.Count > 1) {
								var p1 = ScoreDetailsMatrix[0];
								for (int numItem = 0; numItem < p1.Count; numItem++) {
									var nthItems = new List<int>();

									for (int ea = 0; ea < ScoreDetailsMatrix.Count; ea++)
										nthItems.Add(ScoreDetailsMatrix[ea][numItem]);

									ScoreDetailsMatrix = ScoreDetailsMatrix.Where(x => x[numItem] == nthItems.Max()).ToList();

									// if only one Player's ScoreDetails are remaining, then stop.
									if (ScoreDetailsMatrix.Count == 1) break;
								}

								foreach (var player in playersWithMaxScore) {
									if (player.CurrentHand.ScoreDetail.SequenceEqual(ScoreDetailsMatrix[0]))
										player.CurrentHand.HandWonGame = true;
								}
							}
							else
								playersWithMaxScore[0].CurrentHand.HandWonGame = true;

						game.ArchivePlayersHands();

						allPlayers.AddRange(game.Players);


						if (g % 100 == 0) Console.WriteLine("Game #{0}", g);
					}

					Console.WriteLine("Time Elapsed: {0}, {1} milliseconds/game avg.", sw.Elapsed, sw.ElapsedMilliseconds / (decimal)numGames);
					sw.Restart();
					Console.WriteLine("Saving Changes");

					db.SaveChanges();
				}
				Console.WriteLine("All games have been saved");
				Console.WriteLine("Time Elapsed: {0}, {1} milliseconds/game to save", sw.Elapsed, sw.ElapsedMilliseconds / (decimal)numGames);
				sw.Stop();
				//#if DEBUG
				//					Console.WriteLine("Press any key to close...");
				//					Console.ReadLine();
				//#endif
			}

			Console.WriteLine("Entire Program Time Elapsed: {0}, {1} milliseconds/game total avg", Overallsw.Elapsed, Overallsw.ElapsedMilliseconds / (decimal)actualNumGames);

			Console.WriteLine("[key press...]");
			Console.ReadLine();
		}
	}
}
