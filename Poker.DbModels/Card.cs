using System;
using System.Collections;

using Poker.NHib.DataAnnotations;

namespace Poker.DbModels {
	public class Card : ModelBaseGuid, IEquatable<Card>, IComparer {
		// Would it be better to just seed the DB with the distinct Cards, then load them so I have a distinct set of cards instead of thousands/millions?

		public virtual CardSuits CardSuit { get; protected set; }
		public virtual CardValues CardValue { get; protected set; }
		public virtual DateTimeOffset CreatedUtc { get; set; }


		protected Card() { } // required for Nhibernate...
		public Card(CardSuits suit, CardValues number) {
			CardSuit = suit;
			CardValue = number;
			CreatedUtc = DateTimeOffset.Now;
		}
		public Card(int suit, CardValues number) : this((CardSuits)suit, (CardValues)number) { }
		public Card(int suit, int number) : this((CardSuits)suit, (CardValues)number) { }
		public Card(CardSuits suit, int number) : this(suit, (CardValues)number) { }

		#region Suit & Number Enums
		/// <summary>
		/// Enumeration for the Card Suits
		/// </summary>
		public enum CardSuits {
			Diamonds = 0,
			Clubs = 1,
			Hearts = 2,
			Spades = 3
		}
		/// <summary>
		/// Enumeration for Card Values
		/// </summary>
		public enum CardValues {
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
			return string.Format("{0} of {1}", CardValue, CardSuit);
		}

		public virtual bool Equals(Card other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.CardValue == CardValue && other.CardSuit == CardSuit;
		}

		public override int GetHashCode() {
			return (this.CardValue.GetHashCode() << 33) ^ this.CardSuit.GetHashCode();
		}

		int IComparer.Compare(object a, object b) {
			Card c1 = (Card)a;
			Card c2 = (Card)b;
			if (c1.CardValue == c2.CardValue)
				return c1.CardSuit.CompareTo(c2.CardSuit);
			return c1.CardValue.CompareTo(c1.CardValue);
		}

		public virtual int CompareTo(Card other) {
			if (this.CardValue < other.CardValue) return 1;
			if (this.CardValue > other.CardValue) return -1;
			if (this.CardSuit < other.CardSuit) return 1;
			if (this.CardSuit > other.CardSuit) return -1;
			return 0;
		}
	}
}
