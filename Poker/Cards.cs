using System.Collections.Generic;
using System.Linq;

namespace Poker {
	public class Cards {

		//private readonly List<Card> cards_ = new List<Card>();
		//public List<Card> CardCollection {
		//	get { return this.cards_; }
		//}

		public List<Card> cards = new List<Card>();

		public void AddCard(Card card) {
			cards.Add(card);
		}

		public void AddCards(IEnumerable<Card> addedCards) {
			cards.AddRange(addedCards);
		}

		public Card DealCard() {
			var local = cards[0];
			cards.RemoveAt(0);
			return local;
		}

		public IEnumerable<Card> DealCards(int numCards) {
			var result = cards.Shuffle().Take(numCards);
			var removedCards = new HashSet<Card>(result);
			//HashSet "should" speed up the RemoveAll http://stackoverflow.com/a/853551/3042939
			cards.RemoveAll(x => removedCards.Contains(x));
			return result;
		}
	}
}
