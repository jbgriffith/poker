using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poker;


namespace PokerUnitTest {
	[TestClass]
	public class EvaluatePokerHands_Test {

		public Player CreatePlayer() {
			return new Player("unit tester", 2000);
		}

		public Player CreateSets(Card.CardValue cardValue, int numOfCards) {
			// numberOfCards must be less than or equal to length of Suit enum.
			return AddCards(CreatePlayer(), cardValue, numOfCards);
		}

		public Player CreateSets(Dictionary<Card.CardValue, int> cards) {
			return AddCards(CreatePlayer(), cards);
		}

		public Player CreateStraightAscending(Card.CardValue cardValueStart, int numOfCards, int? cardsuit = null) {
			Player player = CreatePlayer();
			// numberOfCards must be less than or equal to length of Suit enum.
			if (cardsuit == null)
				for (int suit = 0; suit < numOfCards; suit++, cardValueStart++)
					player.AddCard(new Card(suit, cardValueStart));
			else
				for (int i = 0; i < numOfCards; i++, cardValueStart++)
					player.AddCard(new Card((int)cardsuit, cardValueStart));
			return player;
		}

		public Player AddCards(Player player, Card.CardValue cardValue, int numOfCards = 1) {
			// numberOfCards must be less than or equal to length of Suit enum.
			for (int i = 0; i < numOfCards; i++)
				player.AddCard(new Card(i, cardValue));
			return player;
		}
		public Player AddCards(Player player, Dictionary<Card.CardValue, int> cards) {
			// card.Value must be less than or equal to length of Suit enum.
			foreach (var card in cards)
				player = AddCards(player, card.Key, card.Value);
			return player;
		}

		// Poker Hands
		public Player GenerateHand_OnePairAces() { return CreateSets(Card.CardValue.Ace, 2); }
		public Player GenerateHand_OnePairTwos() { return CreateSets(Card.CardValue.Two, 2); }
		public Player GenerateHand_OnePairKings() { return CreateSets(Card.CardValue.King, 2); }

		// Two Pairs
		public Player Hand_TwoPairAcesHighEightLow() {
			return CreateSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 2 }, { Card.CardValue.Eight, 2 } });
		}

		// Three of a Kind
		public Player GenerateHand_ThreeOfAKindAces() { return CreateSets(Card.CardValue.Ace, 3); }
		public Player GenerateHand_ThreeOfAKindTwos() { return CreateSets(Card.CardValue.Two, 3); }
		public Player GenerateHand_ThreeOfAKindKings() { return CreateSets(Card.CardValue.King, 3); }

		// Straight
		public Player GenerateHand_StraightLowAce() {
			Player player = CreateStraightAscending(Card.CardValue.Two, 4);
			AddCards(player, Card.CardValue.Ace);
			return player; //A2345
		}
		public Player GenerateHand_StraightHighAce() {
			Player player = CreateStraightAscending(Card.CardValue.Ten, 4);
			AddCards(player, Card.CardValue.Ace);
			return player; //TJQKA
		}
		public Player GenerateHand_StraightHighKing() {
			Player player = CreateStraightAscending(Card.CardValue.Ten, 4);
			AddCards(player, Card.CardValue.Nine);
			return player; //9TJQK
		}

		// Full House
		public Player GenerateHand_FullHouse_EightsFullOfAces() { return CreateSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 2 }, { Card.CardValue.Eight, 3 } }); }
		public Player GenerateHand_FullHouse_AcesFullOfEights() { return CreateSets(new Dictionary<Card.CardValue, int>() { { Card.CardValue.Ace, 3 }, { Card.CardValue.Eight, 2 } }); }

		// Four of a Kind
		public Player GenerateHand_FourOfAKindAces() { return CreateSets(Card.CardValue.Ace, 4); }
		public Player GenerateHand_FourOfAKindKings() { return CreateSets(Card.CardValue.King, 4); }

		// Straight Flush
		public Player Hand_StraightFlushHighKing() {
			Player player = CreatePlayer();
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.King));
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Queen));
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Jack));
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Ten));
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Nine));
			return player;
		}

		// Royal flush
		public Player Hand_RoyalFlushAceKingClubs() {
			Player player = CreatePlayer();
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Ace));
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.King));
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Queen));
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Jack));
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Ten));
			return player;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Actual Testing ////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////


		// One Pair
		[TestMethod]
		public void OnePair() {
			Player player = GenerateHand_OnePairAces();
			Assert.IsTrue(EvaluatePokerHand.OnePair(player.cards));
		}
		[TestMethod]
		public void Score_OnePair() {
			Player player = GenerateHand_OnePairAces();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), (int)EvaluatePokerHand.PokerHand.OnePair);
		}


		// Two Pair
		[TestMethod]
		public void TwoPair() {
			var player = Hand_TwoPairAcesHighEightLow();
			Assert.IsTrue(EvaluatePokerHand.TwoPair(player.cards));
		}
		[TestMethod]
		public void Score_TwoPair() {
			var player = Hand_TwoPairAcesHighEightLow();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), (int)EvaluatePokerHand.PokerHand.TwoPair);
		}


		// Three of a Kind
		[TestMethod]
		public void ThreeOfAKind() {
			var player = GenerateHand_ThreeOfAKindAces();
			Assert.IsTrue(EvaluatePokerHand.ThreeOfAKind(player.cards));
		}
		[TestMethod]
		public void Score_ThreeOfAKind() {
			var player = GenerateHand_ThreeOfAKindAces();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), (int)EvaluatePokerHand.PokerHand.ThreeOfAKind);
		}


		// Straight
		[TestMethod]
		public void StraightLowAce() {
			Player player = GenerateHand_StraightLowAce();
			Assert.IsTrue(EvaluatePokerHand.Straight(player.cards));
		}
		[TestMethod]
		public void Score_StraightLowAce() {
			Player player = GenerateHand_StraightLowAce();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), (int)EvaluatePokerHand.PokerHand.Straight);
		}

		[TestMethod]
		public void Straight_AceHigh() {
			Player player = GenerateHand_StraightHighAce();
			Assert.IsTrue(EvaluatePokerHand.Straight(player.cards));
		}
		[TestMethod]
		public void Score_Straight_AceHigh() {
			Player player = GenerateHand_StraightHighAce();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), (int)EvaluatePokerHand.PokerHand.Straight);
		}


		// Flush


		// Straight Flush
		[TestMethod]
		public void StraightFlush() {
			Player player = Hand_StraightFlushHighKing();
			Assert.IsTrue(EvaluatePokerHand.StraightFlush(player.cards));
		}
		[TestMethod]
		public void Score_StraightFlush() {
			Player player = Hand_StraightFlushHighKing();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), (int)EvaluatePokerHand.PokerHand.StraightFlush);
		}

		// Full House
		[TestMethod]
		public void FullHouseEightsFullOfAces() {
			Player player = GenerateHand_FullHouse_EightsFullOfAces();
			Assert.IsTrue(EvaluatePokerHand.FullHouse(player.cards));
		}
		[TestMethod]
		public void Score_FullHouseEightsFullOfAces() {
			Player player = GenerateHand_FullHouse_EightsFullOfAces();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), (int)EvaluatePokerHand.PokerHand.FullHouse);
		}

		[TestMethod]
		public void Fail_FullHouse() {
			Player player = GenerateHand_FourOfAKindAces();
			Assert.IsFalse(EvaluatePokerHand.FullHouse(player.cards));
		}
		[TestMethod]
		public void Score_Fail_FullHouse() {
			Player player = GenerateHand_FourOfAKindAces();
			Assert.AreNotEqual(EvaluatePokerHand.Score(player.cards), (int)EvaluatePokerHand.PokerHand.FullHouse);
		}

		[TestMethod]
		public void Fail_FullHouse2() {
			Player player = new Player("unit tester", 2000);
			player.AddCard(new Card(Card.CardSuit.Clubs, Card.CardValue.Seven));
			player.AddCard(new Card(Card.CardSuit.Diamonds, Card.CardValue.Five));
			player.AddCard(new Card(Card.CardSuit.Hearts, Card.CardValue.Five));
			player.AddCard(new Card(Card.CardSuit.Hearts, Card.CardValue.Two));
			player.AddCard(new Card(Card.CardSuit.Spades, Card.CardValue.Ace));
			Assert.IsFalse(EvaluatePokerHand.FullHouse(player.cards));
		}


		// Four of a Kind
		[TestMethod]
		public void FourOfAKind() {
			var player = GenerateHand_FourOfAKindAces();
			Assert.IsTrue(EvaluatePokerHand.FourOfAKind(player.cards));
		}
		[TestMethod]
		public void Score_FourOfAKind() {
			var player = GenerateHand_FourOfAKindAces();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), (int)EvaluatePokerHand.PokerHand.FourOfAKind);
		}


		// Royal Flush
		[TestMethod]
		public void RoyalFlushAceHigh() {
			Player player = Hand_RoyalFlushAceKingClubs();
			Assert.IsTrue(EvaluatePokerHand.RoyalFlush(player.cards));
		}
		[TestMethod]
		public void Score_RoyalFlushAceHigh() {
			Player player = Hand_RoyalFlushAceKingClubs();
			Assert.AreEqual(EvaluatePokerHand.Score(player.cards), (int)EvaluatePokerHand.PokerHand.RoyalFlush);
		}

		// Misc
		[TestMethod]
		public void NumberOfSets() {
			Player player = GenerateHand_FourOfAKindAces();
			Assert.AreEqual(EvaluatePokerHand.NumberOfSets(player.cards, 2), 1);
		}

	}
}
