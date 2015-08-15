using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Poker.DbModels {
	public class Hand : IEnumerable<Card> {
		//[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Guid Id { get; set; }
		public int Score { get; protected set; }
		public bool HandWonGame { get; set; }
		public DateTimeOffset CreatedUtc { get; set; }
		public virtual Player Player { get; set; }			//Associated to a single Player
		public virtual ICollection<Card> Cards { get; set; }

		// If I try to persist, Poker.Nhib.CustomHasManyConvention.Apply throws an exception for trying to save a List<int>.....
		// Maybe make a ScoreDetails Class which is a List<int> ??????
		public ICollection<int> ScoreDetail { get; set; }

		[NotMapped]
		public int Count { get { return Cards.Count; } protected set { } }

		public void AddCard(Card card) { Cards.Add(card); }


		#region IEnumerable
		public IEnumerator<Card> GetEnumerator() { return Cards.GetEnumerator(); }
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return Cards.GetEnumerator(); }
		#endregion

		public Hand() {
			Id = GuidComb.Generate();
			Cards = new List<Card>();
			//Player = new Player();
			Player = null;
			Score = -1;
			ScoreDetail = new List<int>();
			CreatedUtc = DateTimeOffset.UtcNow;
		}

		public Hand(IEnumerable<Card> incomingCards, Player player) {
			Id = GuidComb.Generate();
			Cards = incomingCards.ToList();
			Player = player;
			Score = -1;
			ScoreDetail = new List<int>();
			CreatedUtc = DateTimeOffset.UtcNow;
		}

		public void CalculateScore() {
			Score = (int)EvaluatePokerHand.Score(this);
		}

		public void SetScoreDetails() {
			ScoreDetail = EvaluatePokerHand.GetScoreDetail(this).ToList();
		}

		//public Card DealCard(int index) {
		//	var result = cards[index];
		//	cards.RemoveAt(index);
		//	return result;
		//}

		public void AddCards(IEnumerable<Card> dealtCards) {
			foreach (var card in dealtCards)
				Cards.Add(card);
		}

		public override string ToString() {
			string result = string.Format("\tEmpty Hand.{0}", Environment.NewLine); ;
			if (Cards.Count > 0) {
				result = "";
				foreach (var card in Cards)
					result += string.Format("\t{0}{1}", card, Environment.NewLine);
			}
			return result;
		}

	}
}

