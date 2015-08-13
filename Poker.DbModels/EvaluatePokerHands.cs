using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.DbModels {
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

		public static PokerHand Score(IEnumerable<Card> cards, int numberOfCards = 5) {
			if (cards == null || cards.Count() != numberOfCards) throw new ArgumentException();
			switch (NumberOfSets(cards)) {
				case 0: {
						if (CheckFlush(cards)) {
							if (CheckAceHighStraight(cards)) return PokerHand.RoyalFlush;
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
						if (CheckFullHouse(cards)) return PokerHand.FullHouse;
						return 0;	// this should never really ever happen.
					}
				default: return 0;	// this should never really ever happen.
			}
		}

		public static bool CheckFullHouse(IEnumerable<Card> cards) {
			var threeOfAKind = GetSetsSized(cards, 3);
			var remainingCards = cards.Except(threeOfAKind);
			return threeOfAKind.Count() == 3 && GetSetsSized(remainingCards, 2).Count() == 2;
		}

		public static bool CheckStraightFlush(IEnumerable<Card> cards) {
			return (CheckFlush(cards) && CheckStraight(cards));
		}

		public static bool CheckFlush(IEnumerable<Card> cards) {
			var Flush =
				from card in cards
				group card by card.CardSuit into f
				select f.Count() == cards.Count();
			return Flush.First();
		}

		public static bool CheckStraight(IEnumerable<Card> cards) {
			return ComputeStraight(cards) || CheckAceHighStraight(cards);
		}
		public static bool CheckAceHighStraight(IEnumerable<Card> cards) {
			return cards.Where(c => c.CardValue == Card.CardValues.Ace).Count() == 1 && ComputeStraight(cards.Select(AceHighValue));
		}
		private static bool ComputeStraight(IEnumerable<Card> cards, int cardCount = 5) {
			cards = cards.OrderBy(c => c.CardValue);
			return cards.Zip(cards.Skip(1), (a, b) => b.CardValue - a.CardValue).All(x => x == 1) && cards.Count() == cardCount;
		}

		public static bool CheckFourOfAKind(IEnumerable<Card> cards) { return GetSetsSized(cards, 4).Count() == 4; }
		public static bool CheckTwoPair(IEnumerable<Card> cards) { return GetSetsSized(cards, 2).Count() == 4; }
		public static bool CheckThreeOfAKind(IEnumerable<Card> cards) { return GetSetsSized(cards, 3).Count() == 3; }
		public static bool CheckOnePair(IEnumerable<Card> cards) { return GetSetsSized(cards, 2).Count() == 2; }

		public static IEnumerable<Card> GetHighCards(IEnumerable<Card> cards) {
			return cards.Select(AceHighValue).OrderByDescending(i => i.CardValue);
		}

		/// <summary>
		/// Transforms Ace from a low card to a high card value
		/// </summary>
		/// <param name="c">A single card</param>
		/// <returns>A transformed single Card</returns>
		public static Card AceHighValue(Card c) {
			return (c.CardValue == Card.CardValues.Ace) ? new Card(c.CardSuit, (int)Card.CardValues.King + 1) : c;
		}

		public static int AceHighNumericValue(Card c) {
			return (int)AceHighValue(c).CardValue;
		}

		/// <summary>
		/// Returns all cards in a set of a specified size
		/// </summary>
		/// <param name="cards">multiple card objects in an IEnumerable</param>
		/// <param name="numberCardsInSet">the specified size of the set of cards</param>
		/// <returns></returns>
		private static IEnumerable<Card> GetSetsSized(IEnumerable<Card> cards, int numberCardsInSet) {
			var gNum = cards.GroupBy(c => c.CardValue).Where(g => g.Count() == numberCardsInSet).Select(c => c.Key);
			return cards.Where(c => gNum.Contains(c.CardValue));
		}

		/// <summary>
		/// Returns all sets from cards.
		/// </summary>
		/// <param name="cards"></param>
		/// <returns></returns>
		private static IEnumerable<Card> GetSets(IEnumerable<Card> cards) {
			var highCards = cards.Select(AceHighValue);
			var gNum = highCards.GroupBy(c => c.CardValue).Where(g => g.Count() > 1).Select(g => g.Key);
			return highCards.Where(c => gNum.Contains(c.CardValue)).OrderByDescending(c => c.CardValue);
		}

		public static IEnumerable<int> GetScoreDetail(Hand hand) {
			switch ((PokerHand)hand.Score) {
				case PokerHand.OnePair:
				case PokerHand.TwoPair:
				case PokerHand.ThreeOfAKind:
				case PokerHand.FourOfAKind:
					return GetSetsAndHighCards(hand); // 1 value for each set, then highcards

				case PokerHand.HighCard:
				case PokerHand.Straight:
				case PokerHand.Flush:
				case PokerHand.StraightFlush:	// 1 value, if same then split pot
					return GetHighCardValue(hand);

				case PokerHand.RoyalFlush:	// 1 suit, if same then split pot
					return GetSuit(hand);

				case PokerHand.FullHouse:	// 1 value, if same then split pot
					return GetThreeOfAKindValue(hand);

				default: return new List<int>(); // should never hit this.
			}
		}

		public static int NumberOfSets(IEnumerable<Card> cards, int numberCardsInSet = 2) {
			var SetsOfCards =
				from card in cards
				group card by card.CardValue into c
				where c.Count() >= numberCardsInSet
				select c.Key;
			return SetsOfCards.Count();
		}

		private static IEnumerable<int> GetThreeOfAKindValue(IEnumerable<Card> cards) {
			return GetSetsSized(cards, 3).Take(1).Select(AceHighValue).Select(c => (int)c.CardValue);
		}

		public static IEnumerable<int> GetSetsAndHighCards(IEnumerable<Card> cards) {
			var cardSets = GetSets(cards);
			var highCards = GetHighCards(cards.Except(cardSets));
			return cardSets.Union(highCards).Select(AceHighValue).Select(c => (int)c.CardValue).Distinct();
		}

		private static IEnumerable<int> GetHighCardValue(IEnumerable<Card> cards) {
			return GetHighCards(cards).Select(c => (int)c.CardValue).Take(1);
		}

		public static IEnumerable<int> GetSuit(IEnumerable<Card> cards) {
			return cards.Take(1).Select(c => (int)c.CardSuit);
		}
	}
}
