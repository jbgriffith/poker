using System.ComponentModel.DataAnnotations;

namespace Poker {
	public class Card// : IComparable<Card>
	{
		[Key]
		public int Id { get; private set; }

		public CardSuit Suit { get; private set; }
		public CardValue Number { get; private set; }

		public Card(CardSuit suit, CardValue number) {
			Suit = suit;
			Number = number;
		}

		public Card(int suit, CardValue number) : this((CardSuit)suit, number) {}

		public Card(CardSuit suit, int number) : this (suit, (CardValue)number) {}

		public Card(int suit, int number) : this((CardSuit)suit, (CardValue)number) {}

		/// <summary>
		/// Enumeration for the Suit
		/// </summary>
		public enum CardSuit {
			Spades = 3,
			Hearts = 2,
			Clubs = 1,
			Diamonds = 0,
		}
		/// <summary>
		/// Enumeration for Card Value
		/// </summary>
		public enum CardValue {
			Ace = 0,
			Two = 1,
			Three = 2,
			Four = 3,
			Five = 4,
			Six = 5,
			Seven = 6,
			Eight = 7,
			Nine = 8,
			Ten = 9,
			Jack = 10,
			Queen = 11,
			King = 12,
		}

		public override string ToString() {
			return string.Format("{0} of {1}", Number, Suit);
		}

		//public int CompareTo(Card other) {
		//	// Alphabetic sort if salary is equal. [A to Z]
		//	if (this.Number == other.Number) {
		//		return this.Suit.CompareTo(other.Suit);
		//	}
		//	// Default to salary sort. [High to low]
		//	return other.Number.CompareTo(this.Number);
		//}

	}
}
