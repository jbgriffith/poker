using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.DbModels {
	public class Deck : Cards {
		public Deck() {
			foreach (Card.CardSuits currentSuit in Enum.GetValues(typeof(Card.CardSuits)))
				foreach (Card.CardValues currentNumber in Enum.GetValues(typeof(Card.CardValues)))
					cards.Add(new Card(currentSuit, currentNumber));
			this.QuickShuffle();
		}

		public virtual IEnumerable<Card> DealCards(int numberOfCards = 1) {
			return RemoveCards(numberOfCards, shuffle:true);
		}

		public virtual IEnumerable<Card> BurnCards(int numberOfCards = 1) {
			return RemoveCards(numberOfCards);
		}
	}
}
