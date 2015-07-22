using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Poker.DbModels;


namespace PokerUnitTest {

	[TestClass]
	public class EvaluatePokerHands_Test {

		/// <summary>
		/// Creates Cards as declared in the Dictionary cards.
		/// </summary>
		/// <param name="cards">Dictionaty of Card.CardValue and the number of those cards</param>
		/// <returns>Cards that contains specified cards.</returns>
		public Cards CreateCardSets(Dictionary<Card.CardValue, int> cards) {
			return AddCards(null, cards);
		}

		/// <summary>
		/// Creates Cards with each card being part of a sequence, as specified by the parameters.
		/// If the cardValueStart is high it will start over and the lowest card value.
		/// if skip is true it can create pseudo sequences where they skip one value, 
		/// which is helpful when creating a hand with sets and you don't want to accidently create an actual sequqence.
		/// </summary>
		/// <param name="cardValueStart">Sequence of cards starts with this card Numeric value.</param>
		/// <param name="numOfCards">Length of sequence of cards generated.</param>
		/// <param name="cardsuit">If not null, specifies which suit all cards will generated will have. Else it will generate each card in the sequence with the next suit.</param>
		/// <param name="skip">If true sequence will skip a card for each iteration. Ex: 2, 4, 6</param>
		/// <returns>Cards, as specified by the parameters.</returns>
		public Cards CreateStraightAscending(Card.CardValue cardValueStart, int numOfCards, int? cardsuit = null, bool skip = false) {
			var numValues = Enum.GetNames(typeof(Card.CardValue)).Length;
			var numSuits = Enum.GetNames(typeof(Card.CardSuit)).Length;

			if (numOfCards > numSuits * numValues || cardsuit != null && numOfCards > numValues )
				throw new ArgumentOutOfRangeException("numOfCards");

			var cards = new Cards();
			if (cardsuit == null)
				for (int suit = 0; suit < numOfCards; suit++, cardValueStart++) {
					var cardValue = (int)cardValueStart % numValues;
					cards.AddCard(new Card((suit % numSuits), cardValue == 0 ? (int)cardValueStart : cardValue));
					if (skip) cardValueStart++;
				}
			else
				for (int i = 0; i < numOfCards; i++, cardValueStart++) {
					var cardValue = (int)cardValueStart % numValues;
					cards.AddCard(new Card((int)(cardsuit % numSuits), cardValue == 0 ? (int)cardValueStart : cardValue));
					if (skip) cardValueStart++;
				}
			return cards;
		}

		/// <summary>
		/// Adds Cards as declared in the Dictionary cards to a specified player.
		/// </summary>
		/// <param name="cards">Player</param>
		/// <param name="cardDict">Dictionaty of Card.CardValue and the number of those cards</param>
		/// <returns>Same player with newly added cards.</returns>
		public Cards AddCards(Cards cards, Dictionary<Card.CardValue, int> cardDict) {
			cards = cards ?? new Cards();
			
			foreach (var card in cardDict)
				cards = AddCards(cards, card.Key, card.Value);
			return cards;
		}

		/// <summary>
		/// Adds multiple Card of same Card.CardValue with multiple suits to Cards collection.
		/// </summary>
		/// <param name="cards">Player which cards will be generated and added to.</param>
		/// <param name="cardValue">Card.CardValue of card to be added</param>
		/// <param name="numOfCards">Number of cards to add with the specified Card.CardValue, which will increment the Card.CardSuit.</param>
		/// <exception cref="ArgumentOutOfRangeException">numOfCards must be within length of Card.CardSuit</exception>
		/// <returns>Cards collection containing specified cards added.</returns>
		public Cards AddCards(Cards cards, Card.CardValue cardValue, int numOfCards = 1) {
			if (numOfCards > Enum.GetNames(typeof(Card.CardSuit)).Length)
				throw new ArgumentOutOfRangeException("numOfCards");
			
			cards = cards ?? new Cards();

			for (int i = 0; i < numOfCards; i++)
				cards.AddCard(new Card(i, cardValue));
			return cards;
		}

		#region Generate Poker Hands
		#region Generate One Pair
		public Cards GenerateHand_OnePair_Aces_SixHigh() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 2 }, 
				{ Card.CardValue.Two, 1 }, { Card.CardValue.Four, 1 }, { Card.CardValue.Six, 1 } }); // AA246
		}
		public Cards GenerateHand_OnePair_Twos_EightHigh() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Two, 2 }, 
				{ Card.CardValue.Four, 1 }, { Card.CardValue.Six, 1 }, { Card.CardValue.Eight, 1 } }); // 22468
		}
		public Cards GenerateHand_OnePair_Kings_TenHigh() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.King, 2 }, 
				{ Card.CardValue.Four, 1 }, { Card.CardValue.Six, 1 }, { Card.CardValue.Ten, 1 } }); // KK46T
		}
		public Cards GenerateHand_OnePair_Threes_AceHigh() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Three, 2 }, 
				{ Card.CardValue.Two, 1 }, { Card.CardValue.Nine, 1 }, { Card.CardValue.Ace, 1 } }); // 3329A
		}
		#endregion
		#region Generate Two Pairs
		public Cards GenerateHand_TwoPairOfAcesEights_TwoHigh() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 2 }, { Card.CardValue.Eight, 2 }, { Card.CardValue.Two, 1 } }); // AA882
		}
		public Cards GenerateHand_TwoPairOfJacksNines_SixHigh() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Jack, 2 }, { Card.CardValue.Nine, 2 }, { Card.CardValue.Six, 1 } }); // AA882
		}
		#endregion
		#region Generate Three of a Kind
		public Cards GenerateHand_ThreeOfAKindAces_FourHigh() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 3 }, { Card.CardValue.Two, 1 }, { Card.CardValue.Four, 1 } }); // AAA24
		}
		public Cards GenerateHand_ThreeOfAKindTwos_SixHigh() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Two, 3 }, { Card.CardValue.Four, 1 }, { Card.CardValue.Six, 1 } }); // 22246
		}
		public Cards GenerateHand_ThreeOfAKindKings_FourHigh() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.King, 3 }, { Card.CardValue.Two, 1 }, { Card.CardValue.Four, 1 } }); // KKK24
		}
		#endregion
		#region Generate Straight
		public Cards GenerateHand_Straight_AceLow() { return CreateStraightAscending(Card.CardValue.Ace, 5); } // A2345
		public Cards GenerateHand_Straight_AceHigh() { return CreateStraightAscending(Card.CardValue.Ten, 5); } // TJQKA
		public Cards GenerateHand_Straight_HighKing() { return CreateStraightAscending(Card.CardValue.Nine, 5); } // 9TJQK 
		public Cards GenerateHand_Straight_Fail_AceHigh() { return CreateStraightAscending(Card.CardValue.Queen, 5); } // QKA23 
		public Cards GenerateHand_Straight_Fail_AceLowSkipTwo() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 1 }, 
				{ Card.CardValue.Three, 1 }, { Card.CardValue.Four, 1 }, { Card.CardValue.Five, 1 }, { Card.CardValue.Six, 1 } }); // A3456
		}
		#endregion
		#region Generate Full House
		public Cards GenerateHand_FullHouse_EightsFullOfAces() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 2 }, { Card.CardValue.Eight, 3 } });  // 888AA
		}
		public Cards GenerateHand_FullHouse_AcesFullOfEights() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 3 }, { Card.CardValue.Eight, 2 } }); // AAA88
		}
		#endregion
		#region Generate Four of a Kind
		public Cards GenerateHand_FourOfAKindAces_EightHigh() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 4 }, { Card.CardValue.Eight, 1 } }); // AAAA8
		}
		public Cards GenerateHand_FourOfAKindKings_NineHigh() {
			return CreateCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.King, 4 }, { Card.CardValue.Nine, 1 } }); // KKKK9
		}
		#endregion
		#region Generate Flush
		public Cards GenerateHand_Flush_KingHigh() { return CreateStraightAscending(Card.CardValue.Five, 5, (int)Card.CardSuit.Diamonds, true); } //579JK (suited)
		#endregion
		#region Generate Straight Flush
		public Cards GenerateHand_StraightFlush_KingHigh() { return CreateStraightAscending(Card.CardValue.Nine, 5, (int)Card.CardSuit.Diamonds); } //9TJQK (suited)
		public Cards GenerateHand_StraightFlush_AceLow() { return CreateStraightAscending(Card.CardValue.Ace, 5, (int)Card.CardSuit.Diamonds); } //A2345 (suited)
		#endregion
		#region Generate Royal flush
		public Cards GenerateHand_RoyalFlushDiamonds() { return CreateStraightAscending(Card.CardValue.Ten, 5, (int)Card.CardSuit.Diamonds); }
		public Cards GenerateHand_RoyalFlushClubs() { return CreateStraightAscending(Card.CardValue.Ten, 5, (int)Card.CardSuit.Clubs); }
		public Cards GenerateHand_RoyalFlushHearts() { return CreateStraightAscending(Card.CardValue.Ten, 5, (int)Card.CardSuit.Hearts); }
		public Cards GenerateHand_RoyalFlushSpades() { return CreateStraightAscending(Card.CardValue.Ten, 5, (int)Card.CardSuit.Spades); }
		#endregion
		#endregion

		#region Actual Testing
		#region One Pair
		[TestMethod]
		public void OnePair() {
			var cards = GenerateHand_OnePair_Aces_SixHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckOnePair(cards) && cards.Count == 5);
		}
		[TestMethod]
		public void Score_OnePair() {
			var cards = GenerateHand_OnePair_Aces_SixHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.OnePair);
		}
		#endregion
		#region Two Pair
		[TestMethod]
		public void TwoPair() {
			var cards = GenerateHand_TwoPairOfAcesEights_TwoHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckTwoPair(cards) && cards.Count == 5);
		}
		[TestMethod]
		public void Score_TwoPair() {
			var cards = GenerateHand_TwoPairOfAcesEights_TwoHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.TwoPair);
		}
		[TestMethod]
		public void ScoreDetail_TwoPair_AcesEights_TwoHigh() {
			var cards = GenerateHand_TwoPairOfAcesEights_TwoHigh();
			var actual = EvaluatePokerHand.GetSetsAndHighCards(cards).ToList();
			var expected = new List<int> { 14, 8, 2 };
			CollectionAssert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ScoreDetail_TwoPair_AcesEights_TwoHigh_Unsorted() {
			var cards = new Cards();
			cards.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Eight));
			cards.AddCard(new Card(Card.CardSuit.Diamonds, Card.CardValue.Ace));
			cards.AddCard(new Card(Card.CardSuit.Hearts, Card.CardValue.Eight));
			cards.AddCard(new Card(Card.CardSuit.Hearts, Card.CardValue.Two));
			cards.AddCard(new Card(Card.CardSuit.Spades, Card.CardValue.Ace));
			var actual = EvaluatePokerHand.GetSetsAndHighCards(cards).ToList();
			var expected = new List<int> { 14, 8, 2 };
			CollectionAssert.AreEqual(expected, actual);
		}
		#endregion
		#region Three of a Kind
		[TestMethod]
		public void ThreeOfAKind() {
			var cards = GenerateHand_ThreeOfAKindAces_FourHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckThreeOfAKind(cards) && cards.Count == 5);
		}
		[TestMethod]
		public void Score_ThreeOfAKind() {
			var cards = GenerateHand_ThreeOfAKindAces_FourHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.ThreeOfAKind);
		}
		#endregion
		#region Straight
		[TestMethod]
		public void StraightLowAce() {
			var cards = GenerateHand_Straight_AceLow();
			Assert.IsTrue(EvaluatePokerHand.CheckStraight(cards));
		}
		[TestMethod]
		public void Score_StraightLowAce() {
			var cards = GenerateHand_Straight_AceLow();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.Straight);
		}

		[TestMethod]
		public void Straight_AceHigh() {
			var cards = GenerateHand_Straight_AceHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckStraight(cards) && cards.Count == 5);
		}
		[TestMethod]
		public void Score_Straight_AceHigh() {
			var cards = GenerateHand_Straight_AceHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.Straight);
		}

		[TestMethod]
		public void Straight_KingHigh() {
			var cards = GenerateHand_Straight_HighKing();
			Assert.IsTrue(EvaluatePokerHand.CheckStraight(cards) && cards.Count == 5);
		}
		[TestMethod]
		public void Score_Straight_KingHigh() {
			var cards = GenerateHand_Straight_HighKing();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.Straight);
		}

		[TestMethod]
		public void Fail_Straight_AceHigh() {
			var cards = GenerateHand_Straight_Fail_AceHigh();
			Assert.IsTrue(!EvaluatePokerHand.CheckStraight(cards) && cards.Count == 5);
		}
		[TestMethod]
		public void Score_Fail_Straight_AceHigh() {
			var cards = GenerateHand_Straight_Fail_AceHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.HighCard);
		}
		[TestMethod]
		public void Fail_Straight_AceLow() {
			var cards = GenerateHand_Straight_Fail_AceLowSkipTwo();
			Assert.IsFalse(EvaluatePokerHand.CheckStraight(cards) && cards.Count == 5);
		}
		#endregion
		#region Flush
		public void Flush_KingHigh() {
			var cards = GenerateHand_Flush_KingHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckFlush(cards) && cards.Count == 5);
		}
		[TestMethod]
		public void Score_Flush_KingHigh() {
			var cards = GenerateHand_StraightFlush_KingHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.StraightFlush);
		}
		#endregion
		#region Straight Flush
		[TestMethod]
		public void StraightFlush() {
			var cards = GenerateHand_StraightFlush_KingHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckStraightFlush(cards) && cards.Count == 5);
		}
		[TestMethod]
		public void Score_StraightFlush() {
			var cards = GenerateHand_StraightFlush_KingHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.StraightFlush);
		}
		#endregion
		#region Full House
		[TestMethod]
		public void FullHouseEightsFullOfAces() {
			var cards = GenerateHand_FullHouse_EightsFullOfAces();
			Assert.IsTrue(EvaluatePokerHand.CheckFullHouse(cards) && cards.Count == 5);
		}
		[TestMethod]
		public void Score_FullHouseEightsFullOfAces() {
			var cards = GenerateHand_FullHouse_EightsFullOfAces();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.FullHouse);
		}

		[TestMethod]
		public void Fail_FullHouse() {
			var cards = GenerateHand_FourOfAKindAces_EightHigh();
			Assert.IsFalse(EvaluatePokerHand.CheckFullHouse(cards));
		}
		[TestMethod]
		public void Score_Fail_FullHouse() {
			var cards = GenerateHand_FourOfAKindAces_EightHigh();
			Assert.AreNotEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.FullHouse);
		}

		[TestMethod]
		public void Fail_FullHouse2() {
			var cards = new Cards();
			cards.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Seven));
			cards.AddCard(new Card(Card.CardSuit.Diamonds, Card.CardValue.Five));
			cards.AddCard(new Card(Card.CardSuit.Hearts, Card.CardValue.Five));
			cards.AddCard(new Card(Card.CardSuit.Hearts, Card.CardValue.Two));
			cards.AddCard(new Card(Card.CardSuit.Spades, Card.CardValue.Ace));
			Assert.IsFalse(EvaluatePokerHand.CheckFullHouse(cards));
		}
		#endregion
		#region Four of a Kind
		[TestMethod]
		public void FourOfAKind() {
			var cards = GenerateHand_FourOfAKindAces_EightHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckFourOfAKind(cards) && cards.Count == 5);
		}
		[TestMethod]
		public void Score_FourOfAKind() {
			var cards = GenerateHand_FourOfAKindAces_EightHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.FourOfAKind);
		}
		#endregion
		#region Royal Flush
		[TestMethod]
		public void RoyalFlushAceHigh() {
			var cards = GenerateHand_RoyalFlushClubs();
			Assert.IsTrue(EvaluatePokerHand.CheckFlush(cards) && EvaluatePokerHand.CheckAceHighStraight(cards) && cards.Count == 5);
		}
		[TestMethod]
		public void Score_RoyalFlushAceHigh() {
			var cards = GenerateHand_RoyalFlushClubs();
			Assert.AreEqual(EvaluatePokerHand.Score(cards), EvaluatePokerHand.PokerHand.RoyalFlush);
		}
		#endregion
		#region Misc Tests
		[TestMethod]
		public void NumberOfSets() {
			var cards = GenerateHand_FourOfAKindAces_EightHigh();
			Assert.AreEqual(EvaluatePokerHand.NumberOfSets(cards, 2), 1);
		}

		[TestMethod]
		public void GetHighCards_StraightFlush_KingHigh() {
			var cards = GenerateHand_StraightFlush_KingHigh();
			var actualCards = new List<Card>();
			actualCards.Add(new Card(Card.CardSuit.Diamonds, Card.CardValue.King));
			actualCards.Add(new Card(Card.CardSuit.Diamonds, Card.CardValue.Queen));
			actualCards.Add(new Card(Card.CardSuit.Diamonds, Card.CardValue.Jack));
			actualCards.Add(new Card(Card.CardSuit.Diamonds, Card.CardValue.Ten));
			actualCards.Add(new Card(Card.CardSuit.Diamonds, Card.CardValue.Nine));

			var actual = EvaluatePokerHand.GetHighCards(actualCards).ToList();
			var expected = new List<Card>();
			expected.Add(new Card(Card.CardSuit.Diamonds, Card.CardValue.King));
			expected.Add(new Card(Card.CardSuit.Diamonds, Card.CardValue.Queen));
			expected.Add(new Card(Card.CardSuit.Diamonds, Card.CardValue.Jack));
			expected.Add(new Card(Card.CardSuit.Diamonds, Card.CardValue.Ten));
			expected.Add(new Card(Card.CardSuit.Diamonds, Card.CardValue.Nine));
			var actualHC = new List<int>();
			var expectedHC = new List<int>();
			var equality = new List<bool>();
			
			foreach (var item in actualCards)
				actualHC.Add(item.GetHashCode());
			foreach (var item in expected)
				expectedHC.Add(item.GetHashCode());
			for (int i = 0; i < actualCards.Count; i++)
				equality.Add(actualCards[i].Equals(expected[i]));

			CollectionAssert.AreEqual(expected, actualCards, CardsNumericallyAscendingHelper.SortCardValueAscending());
		}

		//[TestMethod]
		//public void GetHighCards_FourOfAKindAces_EightHigh() {
		//	var cards = GenerateHand_FourOfAKindAces_EightHigh();
		//	var actual = EvaluatePokerHand.GetHighCards(player.cards).ToList();
		//	var expected = new List<int> { 13, 13, 13, 13, 7 };
		//	CollectionAssert.AreEqual(actual, expected);
		//}

		//[TestMethod]
		//public void GetHighCards_OnePair_Threes_AceHigh() {
		//	var cards = GenerateHand_OnePair_Threes_AceHigh();
		//	var actual = EvaluatePokerHand.GetHighCards(player.cards).ToList();
		//	var expected = new List<int> { 13, 8, 2, 2, 1 };
		//	CollectionAssert.AreEqual(actual, expected);
		//}

		[TestMethod]
		public void GetSetsAndHighCards_OnePair_Threes_AceHigh() {
			var cards = GenerateHand_OnePair_Threes_AceHigh();
			var actual = EvaluatePokerHand.GetSetsAndHighCards(cards).ToList();
			var expected = new List<int> { 3, 14, 9, 2 };
			CollectionAssert.AreEqual(actual, expected);
		}

		[TestMethod]
		public void GetSuit_AllSpades() {
			var cards = GenerateHand_RoyalFlushClubs();
			var actual = EvaluatePokerHand.GetSuit(cards).ToList();
			var expected = new List<int> { (int)Card.CardSuit.Clubs };
			CollectionAssert.AreEqual(actual, expected);
		}
		#endregion
		#endregion
	}
}
