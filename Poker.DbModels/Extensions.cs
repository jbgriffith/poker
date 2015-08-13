using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Poker.DbModels {
	public static class ThreadSafeRandom {
		[ThreadStatic]
		private static Random Local;

		public static Random ThisThreadsRandom {
			get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
		}
	}
	public static class MyExtensions {

		public static void RemoveAll<T>(this IList<T> source, Predicate<T> predicate) {
			// TODO: Argument non-nullity validation

			//// Optimization
			//List<T> list = source as List<T>;
			//if (list != null) {
			//    list.RemoveAll(predicate);
			//    return;
			//}
			// Slow way
			for (int i = source.Count - 1; i >= 0; i--) {
				if (predicate(source[i])) {
					source.RemoveAt(i);
				}
			}
		}

		public static void AddRange<T>(this IList<T> source, IEnumerable<T> ienum) {
			// TODO: Argument non-nullity validation

			// Optimization
			List<T> list = source as List<T>;
			if (list != null)
				list.AddRange(ienum);

			//// Slow way
			//for (int i = source.Count - 1; i >= 0; i++) {
			//        source.Add(source[i]);
			//}

		}

		public static void QuickShuffle<T>(this IList<T> list) {
			int n = list.Count;
			while (n-- > 1) {
				int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}


	public static class EnumerableExtensions {

		//public static IEnumerable<Card> MaxByMany<TSource, TKey>(this Hand source, Func<Card, int> selector) {
		//    return source.MaxByMany(selector, Comparer<int>.Default);
		//}

		//public static IEnumerable<Card> MaxByMany<TSource, TKey>(this Hand source, Func<Card, int> selector, IComparer<int> comparer) {
		//    if (source == null) throw new ArgumentNullException("source");
		//    if (selector == null) throw new ArgumentNullException("selector");
		//    if (comparer == null) throw new ArgumentNullException("comparer");
		//    using (var sourceIterator = source.GetEnumerator()) {
		//        if (!sourceIterator.MoveNext())
		//            throw new InvalidOperationException("Sequence contains no elements");

		//        var current = sourceIterator.Current;
		//        var maxCards = new List<Card>();
		//        maxCards.Add(current);
		//        var maxKey = selector(current);
		//        var maxDict = new Dictionary<int, List<Card>>();
		//        maxDict.Add(maxKey, maxCards);

		//        while (sourceIterator.MoveNext()) {
		//            var candidate = sourceIterator.Current;
		//            var candidateProjected = selector(candidate);
		//            if (comparer.Compare(candidateProjected, maxKey) > 0) {

		//                List<Card> existingCardsList;
		//                if (!maxDict.TryGetValue(candidateProjected, out existingCardsList))  {
		//                    existingCardsList = new List<Card>();
		//                    existingCardsList.Add(candidate);
		//                    maxDict[candidateProjected] = existingCardsList;
		//                }

		//                maxCards = new List<Card>();
		//                maxCards.Add(candidate);
		//                maxKey = candidateProjected;
		//            }
		//        }
		//        return maxCards;
		//    }
		//}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) {
			return source.Shuffle(new Random());
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng) {
			if (source == null) throw new ArgumentNullException("source");
			if (rng == null) throw new ArgumentNullException("rng");
			return source.ShuffleIterator(rng);
		}

		private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> source, Random rng) {
			var buffer = source.ToList();
			for (int i = 0; i < buffer.Count; i++) {
				//int j = rng.Next(i, buffer.Count);
				int j = ThreadSafeRandom.ThisThreadsRandom.Next(i, buffer.Count);
				yield return buffer[j];

				buffer[j] = buffer[i];
			}
		}
	}
}
