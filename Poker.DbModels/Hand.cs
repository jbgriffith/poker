using Poker.NHib.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.DbModels {
    public class Hand : ModelBaseGuid, IEnumerable<Card> // : Cards 
	{
		public virtual Player Player { get; set; }			//Associated to a single Player
        public virtual int Score { get; protected set; }
        public virtual bool HandWonGame { get; set; }
		public virtual DateTimeOffset CreatedUtc { get; set; }
	
		public virtual IList<Card> cards { get; set; }

		[NotPersisted]
		public virtual int Count { get { return cards.Count; } protected set { } }

		public virtual void AddCard(Card card) { cards.Add(card); }
		

		#region IEnumerable
		public virtual IEnumerator<Card> GetEnumerator() { return cards.GetEnumerator(); }
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return cards.GetEnumerator(); }
		#endregion

		// If I try to persist, Poker.Nhib.CustomHasManyConvention.Apply throws an exception for trying to save a List<int>..... 
        // Maybe make a ScoreDetails Class which is a List<int> ??????
        [NotPersisted] 
        public virtual IList<int> ScoreDetail { get; set; }

        public Hand() {
			cards = new List<Card>();
			Player = new Player();
            Score = -1;
            ScoreDetail = new List<int>();
			CreatedUtc = DateTimeOffset.UtcNow;
        }

        public Hand(IEnumerable<Card> incomingCards, Player player) {
            cards = incomingCards.ToList();
			Player = player;
            Score = -1;
            ScoreDetail = new List<int>();
			CreatedUtc = DateTimeOffset.UtcNow;
        }        

        public virtual void CalculateScore() {
            Score = (int)EvaluatePokerHand.Score(this);
        }

        public virtual void SetScoreDetails() {
            ScoreDetail = EvaluatePokerHand.GetScoreDetail(this).ToList();
        }

        public virtual Card DealCard(int index) {
			var result = cards[index];
			cards.RemoveAt(index);
			return result;
		}

        public virtual void AddCards(IEnumerable<Card> dealtCards) {
			foreach (var card in dealtCards) {
				cards.Add(card);
				//card.Hand = this;
			}
        }
    }
}

