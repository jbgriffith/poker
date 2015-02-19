using System.Collections.Generic;

namespace Poker {
	public class Cards {

		private readonly List<Card> cards_ = new List<Card>();
		public List<Card> CardCollection {
			get { return this.cards_; }
		}

		//public Collection<Card> cards = new Collection<Card>();

		public void AddCard(Card card) {
			CardCollection.Add(card);
			//CardCollection.Sort();
		}

		public Card ReturnCard()
		{
			var local = CardCollection[0];
			CardCollection.RemoveAt(0);
			return local;
		}
	}
}
