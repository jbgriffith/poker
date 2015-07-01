using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poker;


namespace PokerUnitTest {

	[TestClass]
	public class EvaluatePokerHands_Test {

		/// <summary>
		/// Creates a player for testing, named unit tester with an age of 2000 years old.
		/// </summary>
		/// <returns>Player for unit testing.</returns>
		public Player CreatePlayer() {
			return new Player("unit tester", 2000);
		}

		/// <summary>
		/// Creates "Unit Tester" Player and Sets of Cards as decalred in the Dictionary cards.
		/// </summary>
		/// <param name="cards">Dictionaty of Card.CardValue and the number of those cards</param>
		/// <returns>Unit tester Player with specified cards.</returns>
		public Player CreatePlayerAndCardSets(Dictionary<Card.CardValue, int> cards) {
			return AddCards(CreatePlayer(), cards);
		}

		/// <summary>
		/// Creates "Unit Tester" Player with a sequence of cards, as specified by the parameters.
		/// </summary>
		/// <param name="cardValueStart">Sequence of cards starts with this card value.</param>
		/// <param name="numOfCards">Length of sequence of cards generated.</param>
		/// <param name="cardsuit">If not null, specifies which suit all cards will generated will have. Else it will generate each card in the sequence with the next suit.</param>
		/// <param name="skip">If true sequence will skip a card for each iteration.</param>
		/// <returns>Unit tester Player with a sequence of cards, as specified by the parameter.</returns>
		public Player CreateStraightAscending(Card.CardValue cardValueStart, int numOfCards, int? cardsuit = null, bool skip = false) {
			var numValues = Enum.GetNames(typeof(Card.CardValue)).Length;
			var numSuits = Enum.GetNames(typeof(Card.CardSuit)).Length;
			Player player = CreatePlayer();
			if (cardsuit == null)
				for (int suit = 0; suit < numOfCards; suit++, cardValueStart++) {
					player.AddCard(new Card(suit % numSuits, (int)cardValueStart % numValues));
					if (skip) cardValueStart++;
				}
			else
				for (int i = 0; i < numOfCards; i++, cardValueStart++) {
					player.AddCard(new Card((int)cardsuit % numSuits, (int)cardValueStart % numValues));
					if (skip) cardValueStart++;
				}
			return player;
		}

		/// <summary>
		/// Adds Cards as declared in the Dictionary cards to a specified player.
		/// </summary>
		/// <param name="player">Player</param>
		/// <param name="cards">Dictionaty of Card.CardValue and the number of those cards</param>
		/// <returns>Same player with newly added cards.</returns>
		public Player AddCards(Player player, Dictionary<Card.CardValue, int> cards) {
			foreach (var card in cards)
				player = AddCards(player, card.Key, card.Value);
			return player;
		}

		/// <summary>
		/// Adds multiple Card of same Card.CardValue with multiple suits to Player specified.
		/// </summary>
		/// <param name="player">Player which cards will be generated and added to.</param>
		/// <param name="cardValue">Card.CardValue of card to be added</param>
		/// <param name="numOfCards">Number of cards to add with the specified Card.CardValue, which will increment the Card.CardSuit.</param>
		/// <exception cref="ArgumentOutOfRangeException">numOfCards must be within length of Card.CardSuit</exception>
		/// <returns>Player with specified cards added.</returns>
		public Player AddCards(Player player, Card.CardValue cardValue, int numOfCards = 1) {
			if (numOfCards > Enum.GetNames(typeof(Card.CardSuit)).Length)
				throw new ArgumentOutOfRangeException("Argument Out of Range Exception to Add Cards.");
			for (int i = 0; i < numOfCards; i++)
				player.AddCard(new Card(i, cardValue));
			return player;
		}

		#region Generate Poker Hands
		#region Generate One Pair
		public Player GenerateHand_OnePair_Aces_SixHigh() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 2 }, 
				{ Card.CardValue.Two, 1 }, { Card.CardValue.Four, 1 }, { Card.CardValue.Six, 1 } }); // AA246
		}
		public Player GenerateHand_OnePair_Twos_EightHigh() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Two, 2 }, 
				{ Card.CardValue.Four, 1 }, { Card.CardValue.Six, 1 }, { Card.CardValue.Eight, 1 } }); // 22468
		}
		public Player GenerateHand_OnePair_Kings_TenHigh() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.King, 2 }, 
				{ Card.CardValue.Four, 1 }, { Card.CardValue.Six, 1 }, { Card.CardValue.Ten, 1 } }); // KK46T
		}
		public Player GenerateHand_OnePair_Threes_AceHigh() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Three, 2 }, 
				{ Card.CardValue.Two, 1 }, { Card.CardValue.Nine, 1 }, { Card.CardValue.Ace, 1 } }); // 3329A
		}
		#endregion
		#region Generate Two Pairs
		public Player GenerateHand_TwoPairOfAcesEights_TwoHigh() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 2 }, { Card.CardValue.Eight, 2 }, { Card.CardValue.Two, 1 } }); // AA882
		}
		public Player GenerateHand_TwoPairOfJacksNines_SixHigh() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Jack, 2 }, { Card.CardValue.Nine, 2 }, { Card.CardValue.Six, 1 } }); // AA882
		}
		#endregion
		#region Generate Three of a Kind
		public Player GenerateHand_ThreeOfAKindAces_FourHigh() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 3 }, { Card.CardValue.Two, 1 }, { Card.CardValue.Four, 1 } }); // AAA24
		}
		public Player GenerateHand_ThreeOfAKindTwos_SixHigh() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Two, 3 }, { Card.CardValue.Four, 1 }, { Card.CardValue.Six, 1 } }); // 22246
		}
		public Player GenerateHand_ThreeOfAKindKings_FourHigh() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.King, 3 }, { Card.CardValue.Two, 1 }, { Card.CardValue.Four, 1 } }); // KKK24
		}
		#endregion
		#region Generate Straight
		public Player GenerateHand_Straight_AceLow() { return CreateStraightAscending(Card.CardValue.Ace, 5); } // A2345
		public Player GenerateHand_Straight_AceHigh() { return CreateStraightAscending(Card.CardValue.Ten, 5); } // TJQKA
		public Player GenerateHand_Straight_HighKing() { return CreateStraightAscending(Card.CardValue.Nine, 5); } // 9TJQK 
		public Player GenerateHand_Straight_Fail_AceHigh() { return CreateStraightAscending(Card.CardValue.Queen, 5); } // QKA23 
		#endregion
		#region Generate Full House
		public Player GenerateHand_FullHouse_EightsFullOfAces() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 2 }, { Card.CardValue.Eight, 3 } });  // 888AA
		}
		public Player GenerateHand_FullHouse_AcesFullOfEights() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 3 }, { Card.CardValue.Eight, 2 } }); // AAA88
		}
		#endregion
		#region Generate Four of a Kind
		public Player GenerateHand_FourOfAKindAces_EightHigh() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 4 }, { Card.CardValue.Eight, 1 } }); // AAAA8
		}
		public Player GenerateHand_FourOfAKindKings_NineHigh() {
			return CreatePlayerAndCardSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.King, 4 }, { Card.CardValue.Nine, 1 } }); // KKKK9
		}
		#endregion
		#region Generate Flush
		public Player GenerateHand_Flush_KingHigh() { return CreateStraightAscending(Card.CardValue.Five, 5, 1, true); } //579JK (suited)
		#endregion
		#region Generate Straight Flush
		public Player GenerateHand_StraightFlush_KingHigh() { return CreateStraightAscending(Card.CardValue.Nine, 5, 1); } //9TJQK (suited)
		public Player GenerateHand_StraightFlush_AceLow() { return CreateStraightAscending(Card.CardValue.Ace, 5, 1); } //A2345 (suited)
		#endregion
		#region Generate Royal flush
		public Player GenerateHand_RoyalFlushDiamonds() { return CreateStraightAscending(Card.CardValue.Ten, 5, 0); }
		public Player GenerateHand_RoyalFlushClubs() { return CreateStraightAscending(Card.CardValue.Ten, 5, 1); }
		public Player GenerateHand_RoyalFlushHearts() { return CreateStraightAscending(Card.CardValue.Ten, 5, 2); }
		public Player GenerateHand_RoyalFlushSpades() { return CreateStraightAscending(Card.CardValue.Ten, 5, 4); }
		#endregion
		#endregion

		#region Actual Testing
		#region One Pair
		[TestMethod]
		public void OnePair() {
			Player player = GenerateHand_OnePair_Aces_SixHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckOnePair(player.cards) && player.cards.Count == 5);
		}
		[TestMethod]
		public void Score_OnePair() {
			Player player = GenerateHand_OnePair_Aces_SixHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.OnePair);
		}
		#endregion
		#region Two Pair
		[TestMethod]
		public void TwoPair() {
			var player = GenerateHand_TwoPairOfAcesEights_TwoHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckTwoPair(player.cards) && player.cards.Count == 5);
		}
		[TestMethod]
		public void Score_TwoPair() {
			var player = GenerateHand_TwoPairOfAcesEights_TwoHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.TwoPair);
		}
		#endregion
		#region Three of a Kind
		[TestMethod]
		public void ThreeOfAKind() {
			var player = GenerateHand_ThreeOfAKindAces_FourHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckThreeOfAKind(player.cards) && player.cards.Count == 5);
		}
		[TestMethod]
		public void Score_ThreeOfAKind() {
			var player = GenerateHand_ThreeOfAKindAces_FourHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.ThreeOfAKind);
		}
		#endregion
		#region Straight
		[TestMethod]
		public void StraightLowAce() {
			Player player = GenerateHand_Straight_AceLow();
			Assert.IsTrue(EvaluatePokerHand.CheckStraight(player.cards));
		}
		[TestMethod]
		public void Score_StraightLowAce() {
			Player player = GenerateHand_Straight_AceLow();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.Straight);
		}

		[TestMethod]
		public void Straight_AceHigh() {
			Player player = GenerateHand_Straight_AceHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckStraight(player.cards) && player.cards.Count == 5);
		}
		[TestMethod]
		public void Score_Straight_AceHigh() {
			Player player = GenerateHand_Straight_AceHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.Straight);
		}

		[TestMethod]
		public void Straight_KingHigh() {
			Player player = GenerateHand_Straight_HighKing();
			Assert.IsTrue(EvaluatePokerHand.CheckStraight(player.cards) && player.cards.Count == 5);
		}
		[TestMethod]
		public void Score_Straight_KingHigh() {
			Player player = GenerateHand_Straight_HighKing();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.Straight);
		}

		[TestMethod]
		public void Fail_Straight_AceHigh() {
			Player player = GenerateHand_Straight_Fail_AceHigh();
			Assert.IsTrue(!EvaluatePokerHand.CheckStraight(player.cards) && player.cards.Count == 5);
		}
		[TestMethod]
		public void Score_Fail_Straight_AceHigh() {
			Player player = GenerateHand_Straight_Fail_AceHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.HighCard);
		}
		#endregion
		#region Flush
		public void Flush_KingHigh() {
			Player player = GenerateHand_Flush_KingHigh();
			Assert.IsTrue(EvaluatePokerHand.Check_Flush(player.cards) && player.cards.Count == 5);
		}
		[TestMethod]
		public void Score_Flush_KingHigh() {
			Player player = GenerateHand_StraightFlush_KingHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.StraightFlush);
		}
		#endregion
		#region Straight Flush
		[TestMethod]
		public void StraightFlush() {
			Player player = GenerateHand_StraightFlush_KingHigh();
			Assert.IsTrue(EvaluatePokerHand.Check_StraightFlush(player.cards) && player.cards.Count == 5);
		}
		[TestMethod]
		public void Score_StraightFlush() {
			Player player = GenerateHand_StraightFlush_KingHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.StraightFlush);
		}
		#endregion
		#region Full House
		[TestMethod]
		public void FullHouseEightsFullOfAces() {
			Player player = GenerateHand_FullHouse_EightsFullOfAces();
			Assert.IsTrue(EvaluatePokerHand.Check_FullHouse(player.cards) && player.cards.Count == 5);
		}
		[TestMethod]
		public void Score_FullHouseEightsFullOfAces() {
			Player player = GenerateHand_FullHouse_EightsFullOfAces();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.FullHouse);
		}

		[TestMethod]
		public void Fail_FullHouse() {
			Player player = GenerateHand_FourOfAKindAces_EightHigh();
			Assert.IsFalse(EvaluatePokerHand.Check_FullHouse(player.cards));
		}
		[TestMethod]
		public void Score_Fail_FullHouse() {
			Player player = GenerateHand_FourOfAKindAces_EightHigh();
			Assert.AreNotEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.FullHouse);
		}

		[TestMethod]
		public void Fail_FullHouse2() {
			Player player = new Player("unit tester", 2000);
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Seven));
			player.AddCard(new Card(Card.CardSuit.Diamonds, Card.CardValue.Five));
			player.AddCard(new Card(Card.CardSuit.Hearts, Card.CardValue.Five));
			player.AddCard(new Card(Card.CardSuit.Hearts, Card.CardValue.Two));
			player.AddCard(new Card(Card.CardSuit.Spades, Card.CardValue.Ace));
			Assert.IsFalse(EvaluatePokerHand.Check_FullHouse(player.cards));
		}
		#endregion
		#region Four of a Kind
		[TestMethod]
		public void FourOfAKind() {
			var player = GenerateHand_FourOfAKindAces_EightHigh();
			Assert.IsTrue(EvaluatePokerHand.CheckFourOfAKind(player.cards) && player.cards.Count == 5);
		}
		[TestMethod]
		public void Score_FourOfAKind() {
			var player = GenerateHand_FourOfAKindAces_EightHigh();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.FourOfAKind);
		}
		#endregion
		#region Royal Flush
		[TestMethod]
		public void RoyalFlushAceHigh() {
			Player player = GenerateHand_RoyalFlushClubs();
			Assert.IsTrue(EvaluatePokerHand.Check_Flush(player.cards) && EvaluatePokerHand.Check_Royal(player.cards) && player.cards.Count == 5);
		}
		[TestMethod]
		public void Score_RoyalFlushAceHigh() {
			Player player = GenerateHand_RoyalFlushClubs();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), EvaluatePokerHand.PokerHand.RoyalFlush);
		}
		#endregion
		#region Misc Tests
		[TestMethod]
		public void NumberOfSets() {
			Player player = GenerateHand_FourOfAKindAces_EightHigh();
			Assert.AreEqual(EvaluatePokerHand.NumberOfSets(player.cards, 2), 1);
		}

		[TestMethod]
		public void GetHighCards_StraightFlush_KingHigh() {
			Player player = GenerateHand_StraightFlush_KingHigh();
			var actual = EvaluatePokerHand.GetHighCards(player.cards).ToList();
			var expected = new List<int> { 12, 11, 10, 9, 8 };
			CollectionAssert.AreEqual(actual, expected);
		}

		[TestMethod]
		public void GetHighCards_FourOfAKindAces_EightHigh() {
			Player player = GenerateHand_FourOfAKindAces_EightHigh();
			var actual = EvaluatePokerHand.GetHighCards(player.cards).ToList();
			var expected = new List<int> { 13, 13, 13, 13, 7 };
			CollectionAssert.AreEqual(actual, expected);
		}

		[TestMethod]
		public void GetHighCards_OnePair_Threes_AceHigh() {
			Player player = GenerateHand_OnePair_Threes_AceHigh();
			var actual = EvaluatePokerHand.GetHighCards(player.cards).ToList();
			var expected = new List<int> { 13, 8, 2, 2, 1 };
			CollectionAssert.AreEqual(actual, expected);
		}

		[TestMethod]
		public void GetSetsAndHighCards_OnePair_Threes_AceHigh() {
			Player player = GenerateHand_OnePair_Threes_AceHigh();
			var actual = EvaluatePokerHand.GetSetsAndHighCards(player.cards).ToList();
			var expected = new List<int> { 2, 13, 8, 1 };
			CollectionAssert.AreEqual(actual, expected);
		}

		[TestMethod]
		public void GetSuit_AllSpades() {
			Player player = GenerateHand_RoyalFlushClubs();
			var actual = EvaluatePokerHand.GetSuit(player.cards).ToList();
			var expected = new List<Card.CardSuit> { Card.CardSuit.Clubs };
			CollectionAssert.AreEqual(actual, expected);
		}
		#endregion
		#endregion
	}
}
