using System;
using System.Collections;

namespace Poker.DbModels {
	public class Card : ModelBaseGuid, IEquatable<Card>, IComparer {
		public CardSuit Suit { get; private set; }
		public CardValue Number { get; private set; }
		//public virtual Cards Cards { get; set; }

		//[Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		//public DateTime CreatedUtc { get; set; }

		public Card(CardSuit suit, CardValue number) {
			Suit = suit;
			Number = number;
		}
		public Card(int suit, CardValue number) : this((CardSuit)suit, (CardValue)number) { }
		public Card(int suit, int number) : this((CardSuit)suit, (CardValue)number) { }
		public Card(CardSuit suit, int number) : this(suit, (CardValue)number) { }

		#region Suit & Number Enums
		/// <summary>
		/// Enumeration for the Suit
		/// </summary>
		public enum CardSuit {
			Diamonds = 0,
			Clubs = 1,
			Hearts = 2,
			Spades = 3			
		}
		/// <summary>
		/// Enumeration for Card Value
		/// </summary>
		public enum CardValue {
			Ace = 1,
			Two = 2,
			Three = 3,
			Four = 4,
			Five = 5,
			Six = 6,
			Seven = 7,
			Eight = 8,
			Nine = 9,
			Ten = 10,
			Jack = 11,
			Queen = 12,
			King = 13,
		}
		#endregion

		public override string ToString() {
			return string.Format("{0} of {1}", Number, Suit);
		}

		public bool Equals(Card other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Number == Number && other.Suit == Suit;
		}

		public override int GetHashCode() {
			return (this.Number.GetHashCode() << 33) ^ this.Suit.GetHashCode();
		}

		int IComparer.Compare(object a, object b) {
			Card c1 = (Card)a;
			Card c2 = (Card)b;
			if (c1.Number == c2.Number)
				return c1.Suit.CompareTo(c2.Suit);
			return c1.Number.CompareTo(c1.Number);
		}

		public int CompareTo(Card other) {
			if (this.Number < other.Number) return 1;
			if (this.Number > other.Number) return -1;
			if (this.Suit < other.Suit) return 1;
			if (this.Suit > other.Suit) return -1;
			return 0;
		}

		//private class sortCardNumberAscendingHelper : IComparer {
		//	int IComparer.Compare(object a, object b) {
		//		Card c1 = (Card)a;
		//		Card c2 = (Card)b;

		//		if (c1.Number > c2.Number) return 1;
		//		if (c1.Number < c2.Number) return -1;
		//		if (c1.Suit > c2.Suit) return 1;
		//		if (c1.Suit < c2.Suit) return -1;

		//		//Console.WriteLine("This is an Error");
		//		//Console.ReadKey();
		//		return 0;

		//	}
		//}

		//private class sortCardNumberDescendingHelper : IComparer {
		//	int IComparer.Compare(object a, object b) {
		//		Card c1 = (Card)a;
		//		Card c2 = (Card)b;

		//		if (c1.Number < c2.Number) return 1;
		//		if (c1.Number > c2.Number) return -1;
		//		if (c1.Suit < c2.Suit) return 1;
		//		if (c1.Suit > c2.Suit) return -1;

		//		//Console.WriteLine("This is an Error");
		//		//Console.ReadKey();
		//		return 0;
		//	}
		//}

		//private class sortMakeDescendingHelper : IComparer {
		//	int IComparer.Compare(object a, object b) {
		//		Card c1 = (Card)a;
		//		Card c2 = (Card)b;
		//		if (c1.Number == c2.Number)
		//			return c1.Suit.CompareTo(c2.Suit);

		//		return c1.Number.CompareTo(c1.Number);
		//	}
		//}

		//public int CompareTo(Card other) {

		//	if (this.Number < other.Number) return 1;
		//	if (this.Number > other.Number) return -1;
		//	if (this.Suit < other.Suit) return 1;
		//	if (this.Suit > other.Suit) return -1;
		//	return 0;
		//}
		// End of nested classes.

		//public int CompareTo(Card other) {
		//	// Alphabetic sort if salary is equal. [A to Z]
		//	if (this.Number == other.Number) {
		//		return this.Suit.CompareTo(other.Suit);
		//	}
		//	// Default to salary sort. [High to low]
		//	return other.Number.CompareTo(this.Number);
		//}

	}

	public class CardsNumericallyAscendingHelper : IComparer {
		int IComparer.Compare(object a, object b) {
			var c1 = (Card)a;
			var c2 = (Card)b;
			if (c1.Number < c2.Number) return 1;
			if (c1.Number > c2.Number) return -1;
			if (c1.Suit < c2.Suit) return 1;
			if (c1.Suit > c2.Suit) return -1;
			return 0;
		}

		public static IComparer SortCardValueAscending() {
			return (IComparer)new CardsNumericallyAscendingHelper();
		}

	}
}
