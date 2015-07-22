using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.DbModels {
	public class Deck : Cards {
		public Deck() {
			foreach (Card.CardSuit currentSuit in Enum.GetValues(typeof(Card.CardSuit)))
				foreach (Card.CardValue currentNumber in Enum.GetValues(typeof(Card.CardValue)))
					base.AddCard(new Card(currentSuit, currentNumber));
			this.Shuffle();
		}

		public IEnumerable<Card> DealCards(int numberOfCards = 1) {
			return RemoveCards(numberOfCards, shuffle:true);
		}

		public IEnumerable<Card> BurnCards(int numberOfCards = 1) {
			return RemoveCards(numberOfCards);
		}
	}
}
