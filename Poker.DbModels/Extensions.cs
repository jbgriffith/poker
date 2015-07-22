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
