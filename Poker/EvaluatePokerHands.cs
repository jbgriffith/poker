using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker {
	public static class EvaluatePokerHand {

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

		public static PokerHand Score(ICollection<Card> cards, int numCards = 5) {
			if (cards.Count() != numCards) throw new ArgumentException();
			switch (NumberOfSets(cards)) {
				case 0: {
						if (Check_Flush(cards)) {
							if (Check_Royal(cards)) return PokerHand.RoyalFlush;
							if (CheckStraight(cards)) return PokerHand.StraightFlush;
							return PokerHand.Flush;
						}
						if (CheckStraight(cards)) return PokerHand.Straight;
						return PokerHand.HighCard;
					}
				case 1: {
						if (CheckOnePair(cards)) return PokerHand.OnePair;
						if (CheckThreeOfAKind(cards)) return PokerHand.ThreeOfAKind;
						if (CheckFourOfAKind(cards)) return PokerHand.FourOfAKind;
						return 0;	// this should never really ever happen.
					}
				case 2: {
						if (CheckTwoPair(cards)) return PokerHand.TwoPair;
						if (Check_FullHouse(cards)) return PokerHand.FullHouse;
						return 0;	// this should never really ever happen.
					}
				default: return 0;	// this should never really ever happen.
			}
		}

		public static IEnumerable<int> GetScoreDetail(int score, ICollection<Card> cards) {
			switch ((PokerHand)score) {
				case PokerHand.OnePair:
				case PokerHand.TwoPair:
				case PokerHand.ThreeOfAKind:
				case PokerHand.FourOfAKind:
					return GetSetsAndHighCards(cards);

				case PokerHand.HighCard:
				case PokerHand.Straight:
				case PokerHand.Flush:
				case PokerHand.StraightFlush:
					// 1 value, if same then split pot
					return GetHighCards(cards).Take(1);

				case PokerHand.RoyalFlush:
					// 1 suit, if same then split pot
					return GetSuit(cards).Take(1).Cast<int>();

				case PokerHand.FullHouse:
					// 1 value, if same then split pot
					Console.WriteLine("Get SetValue(ThreeOfAKind)");
					return GetThreeOfAKindValue(cards);

				default: return new List<int>(); // should never hit this.
			}
		}


		//public static bool Check_RoyalFlush(IEnumerable<Card> cards) {
		//	return CheckAceHighStraight(cards) && Check_Flush(cards);
		//}

		public static bool Check_Royal(IEnumerable<Card> cards) { return CheckAceHighStraight(cards); }

		public static bool Check_FullHouse(IEnumerable<Card> cards) {
			var ThreeOfAKind = GetSets(cards, 3);
			var ThreeOfAKindValue = ThreeOfAKind.FirstOrDefault();
			var subsetCards = cards.Where(x => x.Number != ThreeOfAKindValue);	//would .Except() be better?

			return ThreeOfAKind.Count() == 1 && GetSets(subsetCards, 2).Count() == 1;
		}

		public static bool Check_StraightFlush(IEnumerable<Card> cards) { return (Check_Flush(cards) && CheckStraight(cards)); }

		public static bool Check_Flush(IEnumerable<Card> cards) {
			var Flush =
				from card in cards
				group card by card.Suit into f
				select f.Count() == cards.Count();

			return Flush.First();
		}

		public static bool CheckStraight(IEnumerable<Card> cards) {
			var Straight =
				from card in cards
				orderby card.Number ascending
				select card.Number;
			Straight = Straight.ToList();
			// Returns True if all cards are sequential, else False. Influenced by http://stackoverflow.com/a/6150439/3042939
			return Straight.Zip(Straight.Skip(1), (a, b) => b - a).All(x => x == 1) && Straight.Count() == cards.Count() || CheckAceHighStraight(cards);
		}

		public static bool CheckAceHighStraight(IEnumerable<Card> cards) {
			var AceHighStraight =
				from card in cards
				where card.Number != 0
				orderby card.Number ascending
				select card.Number;
			AceHighStraight = AceHighStraight.ToList();
			// possible issue with card.Count() - 1 depending on the game.
			return AceHighStraight.Count() == cards.Count() - 1 && AceHighStraight.Zip(AceHighStraight.Skip(1), (a, b) => b - a).All(x => x == 1);
		}

		public static bool CheckFourOfAKind(IEnumerable<Card> cards) { return GetSets(cards, 4).Count() == 1; }
		public static bool CheckTwoPair(IEnumerable<Card> cards) { return GetSets(cards, 2).Count() == 2; }
		public static bool CheckThreeOfAKind(IEnumerable<Card> cards) { return GetSets(cards, 3).Count() == 1; }
		public static bool CheckOnePair(IEnumerable<Card> cards) { return GetSets(cards, 2).Count() == 1; }

		public static IEnumerable<int> GetHighCards(IEnumerable<Card> cards) { return cards.ToList().ConvertAll(AceHighValue).OrderByDescending(i => i); }

		public static IEnumerable<Card.CardSuit> GetSuit(IEnumerable<Card> cards) {
			var Flush =
				from card in cards
				group card by card.Suit into f
				select f.Key;

			return Flush;
		}

		public static int AceHighValue(Card.CardValue c) { return (int)c == 0 ? 13 : (int)c; }
		public static int AceHighValue(Card c) { return AceHighValue(c.Number); }

		public static IEnumerable<Card.CardValue> GetSets(IEnumerable<Card> cards, int numberCardsInSet) {
			var SetsOfCards =
				from card in cards
				group card by card.Number into c
				where c.Count() == numberCardsInSet
				select c.Key;

			return SetsOfCards;
		}

		public static IEnumerable<Card.CardValue> GetSetValues(IEnumerable<Card> cards) {
			var SetsOfCards =
				from card in cards
				group card by card.Number into c
				where c.Count() > 1
				select c.Key;

			return SetsOfCards;
		}

		public static IEnumerable<int> GetThreeOfAKindValue(IEnumerable<Card> cards) { return GetSets(cards, 3).Take(1).ToList().ConvertAll(AceHighValue); }

		public static IEnumerable<int> GetSetsAndHighCards(IEnumerable<Card> cards) {
			var orderedSetValues = GetSetValues(cards).Cast<int>();
			var oSVAppendHighCards = GetHighCards(cards).Except(orderedSetValues);
			return orderedSetValues.Union(oSVAppendHighCards);
		}

		public static int NumberOfSets(IEnumerable<Card> cards, int numberCardsInSet = 2) {
			var SetsOfCards =
				from card in cards
				group card by card.Number into c
				where c.Count() >= numberCardsInSet
				select c.Key;

			return SetsOfCards.Count();
		}
	}
}
