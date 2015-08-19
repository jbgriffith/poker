using System;
using System.Collections.Generic;
using System.Linq;



namespace Poker.DbModels {
	public class Deck {
		public Guid Id { get; set; }
		public IList<Card> cards { get; set; }
		public DateTimeOffset CreatedUtc { get; set; }

		public Deck() {
			Id = GuidComb.Generate();
			cards = new List<Card>();
			foreach (Card.CardSuits currentSuit in Enum.GetValues(typeof(Card.CardSuits)))
				foreach (Card.CardValues currentNumber in Enum.GetValues(typeof(Card.CardValues)))
					cards.Add(new Card(currentSuit, currentNumber));
			cards.QuickShuffle();
			CreatedUtc = DateTimeOffset.UtcNow;
		}

		public IEnumerable<Card> DealCards(int numberOfCards = 1) {
			return RemoveCards(numberOfCards, shuffle: true);
		}

		public IEnumerable<Card> BurnCards(int numberOfCards = 1) {
			return RemoveCards(numberOfCards);
		}

		#region Methods
		/// <summary>
		/// Removes specified number of cards from List of Card.
		/// </summary>
		/// <param name="numberOfCards">number of cards to return</param>
		/// <param name="shuffle">if true, then it will shuffle the cards before taking the specified number of cards.</param>
		/// <returns>IEnumerable of Card</returns>
		protected IEnumerable<Card> RemoveCards(int numberOfCards, bool shuffle = false) {
			var result = (shuffle) ? cards.Shuffle().Take(numberOfCards) : cards.Take(numberOfCards);
			var removedCards = new HashSet<Card>(result);	//HashSet "should" speed up the RemoveAll http://stackoverflow.com/a/853551/3042939
			cards.RemoveAll(x => removedCards.Contains(x));
			return removedCards;
		}

		public void BurnDeck() {
			cards.Clear();
		}

		/// <summary>
		/// This Method overrides the Default .ToString() Method which allows for printing of the Deck Object with a user friendly message.
		/// If no cards are in the Deck, it will also include a message for that.
		/// </summary>
		/// <returns>A user friendly representation of the Deck object.</returns>
		public override string ToString() {
			string result = string.Format("No cards.{0}", Environment.NewLine); ;
			if (cards.Count > 0) {
				result = "";
				foreach (var card in cards)
					result += string.Format("\t{0}{1}", card, Environment.NewLine);
			}
			return result;
		}
		#endregion
	}
}
