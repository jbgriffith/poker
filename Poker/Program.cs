using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using UserData;

namespace Poker {
	class Program {
		static void Main(string[] args) {
			List<Player> AllPlayers = new List<Player>();

			var sw = Stopwatch.StartNew();
			for (int users = 0; users < 4; users++) {
				AllPlayers.AddRange(UserData.UserData.GetUserData("http://api.randomuser.me/?results=500&key=6QGL-Z2YJ-OL4O-MACS"));
			}

			Console.WriteLine("Time Elapsed: {0}", sw.Elapsed);
			sw.Restart();

			for (int i = 0; i < 3; i++) {
				using (var db = new PokerContext()) {
					db.Configuration.AutoDetectChangesEnabled = false;
					db.Configuration.ValidateOnSaveEnabled = false;

					for (int games = 1; games < 401; games++) {

						List<Player> Players = new List<Player>();

						//Random r = new Random();
						//int range = 4;
						//int rInt = r.Next(2, range); //for ints

						int rInt = 5;

						// Mix it up a little
						AllPlayers.QuickShuffle();

						// Add each player to the table
						var firstNPlayers = AllPlayers.Take(rInt).ToList();
						firstNPlayers.ForEach(player => AllPlayers.Remove(player));
						Players.AddRange(firstNPlayers);
						db.Players.AddRange(firstNPlayers);

						// Create New Deck
						//int numberOfDecks = (int)Math.Ceiling((5.0 * (numPLayers + 1)) / 52);
						Deck decks = new Deck();

						// Deal out the Cards to the players & Score their hands, then fold.
						foreach (var player in Players) {
							for (int j = 0; j < 5; j++)
								player.AddCard(decks.DealCard());
							player.Score = EvaluatePokerHand.Score(player.CardCollection);
							player.Fold();
							//Players.Remove(player);
							AllPlayers.Add(player);
						}
						// Scoring

						//Royal Flush		=	9S	split pot
						//Straight flush	=	8HS	highest value of cards, if same then split pot
						//Four of a kind	=	7	highest value of cards
						//Full House		=	6	highest value of the three of a kind
						//Flush				=	5HS	highest value of cards, if same then split pot
						//Straight			=	4HS	highest value of cards, if same then split pot
						//Three of a Kind	=	3	highest value of cards
						//2 Pair			=	2HS	highest value of cards, if same then split pot
						//One Pair			=	1HS	highest value of cards, if same then split pot
						//High Card			=	0HS	highest value of cards, if same then split pot

						if (games % 100 == 0)
							Console.WriteLine("Game #{0}", games);

					}
					Console.WriteLine("Time Elapsed: {0}", sw.Elapsed);
					sw.Restart();
					Console.WriteLine("Saving Changes");
					db.SaveChanges();
					Console.WriteLine("All games have been saved");
					Console.WriteLine("Time Elapsed: {0}", sw.Elapsed);
					sw.Stop();
#if DEBUG
				Console.WriteLine("Press any key to close...");
				Console.ReadLine();
#endif
				}
			}
		}
	}


	public static class ThreadSafeRandom {
		[ThreadStatic]
		private static Random Local;

		public static Random ThisThreadsRandom {
			get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
		}
	}

	static class MyExtensions {
		public static void QuickShuffle<T>(this IList<T> list) {
			int n = list.Count;
			while (n-- > 1) {
				int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		/// <summary>
		/// Removes the last element from a List of any Type
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="list">A List of any Type</param>
		/// <returns></returns>
		public static T Pop<T>(this Collection<T> list) {
			if (list == null || list.Count == 0)
				throw new ArgumentNullException("list");

			var local = list[0];
			list.RemoveAt(0);
			return local;
		}

		public static void Shuffle<T>(this IList<T> list) {
			RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
			int n = list.Count;
			while (n > 1) {
				byte[] box = new byte[1];
				do provider.GetBytes(box);
				while (!(box[0] < n * (Byte.MaxValue / n)));
				int k = (box[0] % n);
				n--;
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}

}
