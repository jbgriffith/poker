using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.DbModels {
	public class Cards : ModelBaseGuid, IEnumerable<Card> {
		protected List<Card> _cards = new List<Card>();

		public int Count { get { return _cards.Count; } }

		#region Methods
		public void AddCard(Card card) { _cards.Add(card); }
		public void AddCards(IEnumerable<Card> cards) { _cards.AddRange(cards); }
		public void Clear() { _cards.Clear(); }
		public void QuickShuffle() { _cards.QuickShuffle(); }
		#region IEnumerable
		public IEnumerator<Card> GetEnumerator() { return _cards.GetEnumerator(); }
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return _cards.GetEnumerator(); }
		/// <summary>
		/// Removes specified number of cards from List of Card.
		/// </summary>
		/// <param name="numberOfCards">number of cards to return</param>
		/// <param name="shuffle">if true, then it will shuffle the cards before taking the specified number of cards.</param>
		/// <returns>IEnumerable of Card</returns>
		protected IEnumerable<Card> RemoveCards(int numberOfCards, bool shuffle = false) {
			var result = (shuffle) ? _cards.Shuffle().Take(numberOfCards) : _cards.Take(numberOfCards);
			var removedCards = new HashSet<Card>(result);	//HashSet "should" speed up the RemoveAll http://stackoverflow.com/a/853551/3042939
			_cards.RemoveAll(x => removedCards.Contains(x));
			return removedCards;
		}
		#endregion

		/// <summary>
		/// This Method overrides the Default .ToString() Method which allows for printing of the Deck Object with a user friendly message.
		/// If no cards are in the Deck, it will also include a message for that.
		/// </summary>
		/// <returns>A user friendly representation of the Deck object.</returns>
		public override string ToString() {
			string result = string.Format("No cards.{0}", Environment.NewLine); ;
			if (_cards.Count > 0) {
				result = "";
				foreach (var card in _cards)
					result += string.Format("\t{0}{1}", card, Environment.NewLine);
			}
			return result;
		}
		#endregion
	}
}
