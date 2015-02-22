using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker {
	public static class EvaluatePokerHand {

		// Scoring -- Need to add HighCard and need to return HighCard for each hand.

		//Royal Flush		=	9S	split pot
		//Straight flush	=	8HS	highest value of cards, if same then split pot
		//Four of a kind	=	7	highest value of cards
		//Full House		=	6	highest value of the three of a kind
		//Flush				=	5HS	highest value of cards, if same then split pot
		//Straight			=	4HS	highest value of cards, if same then split pot
		//Three of a Kind	=	3	highest value of cards
		//2 Pair			=	2HS	highest value of cards, if same then split pot
		//One Pair			=	1HS	highest value of cards, if same then split pot
		//High Card			=	0HS	highest value of cards, if same then split pot

		public enum PokerHand {
			None = 0,
			HighCard = 1,
			OnePair = 2,
			TwoPair = 3,
			ThreeOfAKind = 4,
			Straight = 5,
			Flush = 6,
			FullHouse = 7,
			FourOfAKind = 8,
			StraightFlush = 9,
			RoyalFlush = 10
		}

		public static int Score(List<Card> cards) {
			switch (EvaluatePokerHand.NumberOfSets(cards)) {
				case 0: {
						if (EvaluatePokerHand.RoyalFlush(cards))
							return (int)EvaluatePokerHand.PokerHand.RoyalFlush;
						if (EvaluatePokerHand.StraightFlush(cards))
							return (int)EvaluatePokerHand.PokerHand.StraightFlush;
						if (EvaluatePokerHand.Straight(cards))
							return (int)EvaluatePokerHand.PokerHand.Straight;
						if (EvaluatePokerHand.Flush(cards))
							return (int)EvaluatePokerHand.PokerHand.Flush;
						return 0;
					}
				case 1: {
						if (EvaluatePokerHand.OnePair(cards))
							return (int)EvaluatePokerHand.PokerHand.OnePair;
						if (EvaluatePokerHand.ThreeOfAKind(cards))
							return (int)EvaluatePokerHand.PokerHand.ThreeOfAKind;
						if (EvaluatePokerHand.FourOfAKind(cards))
							return (int)EvaluatePokerHand.PokerHand.FourOfAKind;
						return 0;
					}
				case 2: {
						if (EvaluatePokerHand.TwoPair(cards))
							return (int)EvaluatePokerHand.PokerHand.TwoPair;
						if (EvaluatePokerHand.FullHouse(cards))
							return (int)EvaluatePokerHand.PokerHand.FullHouse;
						return 0;
					}
				default: {
						return 0;
					}
			}
		}

		public static bool RoyalFlush(IEnumerable<Card> cards) {
			return AceHighStraight(cards) && Flush(cards);
		}

		public static bool FullHouse(IEnumerable<Card> cards) {
			var ThreeOfAKind = ReturnSets(cards, 3);
			var ThreeOfAKindValue = ThreeOfAKind.FirstOrDefault();
			var subsetCards = cards.Where(x => x.Number != ThreeOfAKindValue);

			return ThreeOfAKind.Count() == 1 && ReturnSets(subsetCards, 2).Count() == 1;
		}

		public static bool StraightFlush(IEnumerable<Card> cards) {
			return (Flush(cards) && Straight(cards));
		}

		public static bool Flush(IEnumerable<Card> cards) {
			var Flush =
				from card in cards
				group card by card.Suit into f
				select f.Count() == cards.Count();

			return Flush.First();
		}

		public static bool Straight(IEnumerable<Card> cards) {
			var Straight =
				from card in cards.AsEnumerable()
				orderby card.Number ascending
				select card.Number;

			// Returns True if all cards are sequential, else False
			// Influenced by http://stackoverflow.com/a/6150439/3042939
			return Straight.Zip(Straight.Skip(1), (a, b) => b - a).All(x => x == 1) || AceHighStraight(cards);
		}

		public static bool AceHighStraight(IEnumerable<Card> cards) {
			var AceHighStraight =
				from card in cards.AsEnumerable()
				where card.Number != 0
				orderby card.Number ascending
				select card.Number;
			return AceHighStraight.Count() == cards.Count() - 1 && AceHighStraight.Zip(AceHighStraight.Skip(1), (a, b) => b - a).All(x => x == 1);
		}

		public static bool FourOfAKind(IEnumerable<Card> cards) {
			return ReturnSets(cards, 4).Count() == 1;
		}

		public static bool TwoPair(IEnumerable<Card> cards) {
			return ReturnSets(cards, 2).Count() == 2;
		}

		public static bool ThreeOfAKind(IEnumerable<Card> cards) {
			return ReturnSets(cards, 3).Count() == 1;
		}

		public static bool OnePair(IEnumerable<Card> cards) {
			return ReturnSets(cards, 2).Count() == 1;
		}

		public static bool MultipleSetCheck(IEnumerable<Card> cards, int setSize, int set2) {
			return ReturnSets(cards, setSize).Count() == set2;
		}

		public static int SetCheck(IEnumerable<Card> cards) {
			return GetCardValue(cards).Count();
		}

		public static IEnumerable<Card.CardValue> ReturnSets(IEnumerable<Card> cards, int numberCardsInSet) {
			var SetsOfCards =
				from card in cards
				group card by card.Number into c
				where c.Count() == numberCardsInSet
				select c.Key;

			return SetsOfCards;
		}

		public static int NumberOfSets(IEnumerable<Card> cards, int numberCardsInSet = 2) {
			var SetsOfCards =
				from card in cards
				group card by card.Number into c
				where c.Count() >= numberCardsInSet
				select c.Key;

			return SetsOfCards.Count();
		}

		public static IEnumerable<Card.CardValue> GetCardValue(IEnumerable<Card> cards) {
			var CardValue =
				from card in cards
				group card by card.Number into c
				select c.Key;

			return CardValue;
		}

	}
}
