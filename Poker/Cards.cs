using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker {
	public class Cards {

		//private readonly List<Card> cards_ = new List<Card>();
		//public List<Card> CardCollection {
		//	get { return this.cards_; }
		//}

		public List<Card> cards = new List<Card>();
		//public virtual List<Card> cards { get; private set; }

		public void AddCard(Card card) {
			cards.Add(card);
		}

		public void AddCards(IEnumerable<Card> addedCards) {
			cards.AddRange(addedCards);
		}

		public IEnumerable<Card> DealCards(int numCards) {
			var result = cards.Shuffle().Take(numCards);
			var removedCards = new HashSet<Card>(result);
			//HashSet "should" speed up the RemoveAll http://stackoverflow.com/a/853551/3042939
			cards.RemoveAll(x => removedCards.Contains(x));
			return removedCards;
		}

		/// <summary>
		/// This Method overrides the Default .ToString() Method which allows for printing of the Deck Object with a user friendly message.
		/// If no cards are in the Deck, it will also include a message for that.
		/// </summary>
		/// <returns>A user friendly representation of the Deck object.</returns>
		public override string ToString() {
			string result = "";
			if (cards.Count > 0)
				foreach (var card in cards)
					result += string.Format("\t{0}{1}", card, Environment.NewLine);
			else
				result = string.Format("No cards.{0}", Environment.NewLine);

			return result;
		}
	}
}
