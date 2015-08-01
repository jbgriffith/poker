using Poker.NHib.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.DbModels {
    public class Hand : Cards {
        public virtual int Score { get; protected set; } = -1;
        public virtual Player Player { get; set; }
        public virtual Game Game { get; set; }
        public virtual bool HandWonGame { get; set; }

        // If I try to persist, Poker.Nhib.CustomHasManyConvention.Apply throws an exception for trying to save a List<int>..... 
        // Maybe make a ScoreDetails Class which is a List<int> ??????
        [NotPersisted] 
        public virtual IList<int> ScoreDetail { get; set; } = new List<int>();

        public Hand() {
            Player = new Player();
            Game = new Game();
        }
        public Hand(IEnumerable<Card> incomingCards, Player player, Game game) {
            cards = incomingCards.ToList();
            Player = player;
            Game = game;
        }        

        public virtual void CalculateHand() {
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
            foreach (var card in dealtCards)
                cards.Add(card);
        }
    }
}

