//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Poker.NHib.DataAnnotations;

//namespace Poker.DbModels {
//	public class Cards : ModelBaseGuid, IEnumerable<Card> {
//		[KeepSequence, Inverse]
//		public virtual IList<Card> cards { get; set; }


//		[NotPersisted]
//		public virtual int Count { get { return cards.Count; } protected set { } }

//		public Cards() {
//			cards = new List<Card>();
//		}
//		#region Methods
//		public virtual void AddCard(Card card) { cards.Add(card); }
//		//public virtual void AddCards(IEnumerable<Card> cards) { this.cards.AddRange(cards); }
//		public virtual void Clear() { cards.Clear(); }
//		public virtual void QuickShuffle() { cards.QuickShuffle(); }
//		#region IEnumerable
//		public virtual IEnumerator<Card> GetEnumerator() { return cards.GetEnumerator(); }
//		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return cards.GetEnumerator(); }
//		/// <summary>
//		/// Removes specified number of cards from List of Card.
//		/// </summary>
//		/// <param name="numberOfCards">number of cards to return</param>
//		/// <param name="shuffle">if true, then it will shuffle the cards before taking the specified number of cards.</param>
//		/// <returns>IEnumerable of Card</returns>
//		protected IEnumerable<Card> RemoveCards(int numberOfCards, bool shuffle = false) {
//			var result = (shuffle) ? cards.Shuffle().Take(numberOfCards) : cards.Take(numberOfCards);
//			var removedCards = new HashSet<Card>(result);	//HashSet "should" speed up the RemoveAll http://stackoverflow.com/a/853551/3042939
//			cards.RemoveAll(x => removedCards.Contains(x));
//			return removedCards;
//		}
//		#endregion
//		//public virtual bool Equals(Cards other) {
//		//	if (ReferenceEquals(null, other)) return false;
//		//	if (ReferenceEquals(this, other)) return true;
//		//	return other.CardValue == CardValue && other.CardSuit == CardSuit;
//		//}

//		//public override int GetHashCode() {
//		//	return (this.CardValue.GetHashCode() << 33) ^ this.CardSuit.GetHashCode();
//		//}

//		/// <summary>
//		/// This Method overrides the Default .ToString() Method which allows for printing of the Deck Object with a user friendly message.
//		/// If no cards are in the Deck, it will also include a message for that.
//		/// </summary>
//		/// <returns>A user friendly representation of the Deck object.</returns>
//		public override string ToString() {
//			string result = string.Format("No cards.{0}", Environment.NewLine); ;
//			if (cards.Count > 0) {
//				result = "";
//				foreach (var card in cards)
//					result += string.Format("\t{0}{1}", card, Environment.NewLine);
//			}
//			return result;
//		}
//		#endregion
//	}
//}
