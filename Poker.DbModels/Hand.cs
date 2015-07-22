using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.DbModels {
	public class Hand : Cards {
		public virtual int Score { get; private set; }

		private List<int> _scoreDetails = new List<int>();
		public List<int> ScoreDetails { get { return _scoreDetails; } }

		public void ScoreHand(Hand hand) {
			_scoreDetails = EvaluatePokerHand.GetScoreDetail(hand).ToList();
		}

		public Card DealCard(int index) {
			var result = _cards[index];
			_cards.RemoveAt(index);
			return result;
		}
	}
}

