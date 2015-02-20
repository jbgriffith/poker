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
			var Overallsw = Stopwatch.StartNew();
			var sw = Stopwatch.StartNew();

			Console.WriteLine("Downloading & Creating users ...");
			for (int users = 0; users < 1; users++) {
				AllPlayers.AddRange(UserData.UserData.GetUserData("http://api.randomuser.me/?results=500&key=6QGL-Z2YJ-OL4O-MACS"));
			}
			Console.WriteLine("Created {0} users", AllPlayers.Count);

			Console.WriteLine("Time Elapsed: {0}", sw.Elapsed);
			Overallsw.Restart();
			for (int i = 0; i < 30000; i++) {
				sw.Restart();
				using (var db = new PokerContext()) {
					//db.Configuration.AutoDetectChangesEnabled = false;
					//db.Configuration.ValidateOnSaveEnabled = false;

					for (int games = 1; games < 101; games++) {

						var Players = new List<Player>();

						//Random r = new Random();
						//int range = 4;
						//int rInt = r.Next(2, range); //for ints

						int rInt = 5;

						// Add each player to the table
						var firstNPlayers = AllPlayers.Shuffle().Take(rInt);
						var NPlayers = new HashSet<Player>(firstNPlayers);
						//" HashSet should" speed up the RemoveAll http://stackoverflow.com/a/853551/3042939
						AllPlayers.RemoveAll(x => NPlayers.Contains(x));

						//EntityFramework
						//db.Players.AddRange(firstNPlayers);

						// Create New Deck
						//int numberOfDecks = (int)Math.Ceiling((5.0 * (numPLayers + 1)) / 52);
						Deck decks = new Deck();
						Players.ForEach(x => {
							x.AddCards(decks.DealCards(5));
							x.Score = EvaluatePokerHand.Score(x.cards);
						});
						//player.Fold();

						AllPlayers.AddRange(Players);

						if (games % 100 == 0) Console.WriteLine("Game #{0}", games);
					}

					Console.WriteLine("Time Elapsed: {0}", sw.Elapsed);
					sw.Restart();
					Console.WriteLine("Saving Changes");

					//EntityFramework
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
			Console.WriteLine("Time Elapsed: {0}", Overallsw.Elapsed);
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

		public static void Shuffles<T>(this IList<T> list) {
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


	public static class EnumerableExtensions {
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) {
			return source.Shuffle(new Random());
		}

		public static IEnumerable<T> Shuffle<T>(
			this IEnumerable<T> source, Random rng) {
			if (source == null) throw new ArgumentNullException("source");
			if (rng == null) throw new ArgumentNullException("rng");

			return source.ShuffleIterator(rng);
		}

		private static IEnumerable<T> ShuffleIterator<T>(
			this IEnumerable<T> source, Random rng) {
			var buffer = source.ToList();
			for (int i = 0; i < buffer.Count; i++) {
				int j = rng.Next(i, buffer.Count);
				yield return buffer[j];

				buffer[j] = buffer[i];
			}
		}
	}
}
