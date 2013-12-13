using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Poker
{
	class Program
	{
		static void Main(string[] args)
		{
			for (int w = 0; w < 100000; w++)
			{
				Collection<string> PlayerNames = new Collection<string> { "Holen", "Garrili", "Anelm", "Chal", "Metin" };
				Collection<Player> PlayersAtPokerTable = new Collection<Player>();
				int numPLayers = PlayerNames.Count;

				// Mix it up a little
				PlayerNames.Shuffle();

				// Add each player to the table
				for (int j = 0; j < numPLayers; j++)
				{
					PlayersAtPokerTable.Add(new Player(PlayerNames[j], j));
				}

				// Create New Deck
				//int numberOfDecks = (int)Math.Ceiling((5.0 * (numPLayers + 1)) / 52);
				Deck decks = new Deck();

				// Deal out the Cards to the players
				for (int j = 0; j < 5; j++)
					foreach (var player in PlayersAtPokerTable)
						player.AddCard(decks.DealCard());

				// Scoring

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

				foreach (var player in PlayersAtPokerTable)
				{
					// Not the best way to do this...
					if (ParsePokerHand.RoyalFlush(player))
					{
						break;
					}
					else if (ParsePokerHand.StraightFlush(player))
					{
						break;
					}
					else if (ParsePokerHand.FullHouse(player))
					{
						break;
					}
					else if (ParsePokerHand.FourOfAKind(player))
					{
						break;
					}
					else if (ParsePokerHand.Flush(player))
					{
						break;
					}
					else if (ParsePokerHand.Straight(player))
					{
						break;
					}
					else if (ParsePokerHand.ThreeOfAKind(player))
					{
						break;
					}
					else if (ParsePokerHand.TwoPair(player))
					{
						break;
					}
					else if (ParsePokerHand.OnePair(player))
					{
						break;
					}
					// Need to add to ParsePokerHand
					//else if (PokerScoring.HighCard(player))
					//{
					//	break;
					//}

					player.Fold();
				}
#if DEBUG
				if (w % 100 == 0)
					Console.WriteLine("Game #{0}", w);
			//Console.WriteLine("Press any key to close...");
			//Console.ReadLine();
#endif

			}
			
		}
	}

	class Dealer : PersonAtPokerTable
	{
		public Dealer() : this("Unnamed Dealer", 0) { }
		public Dealer(string NameText, int AgeValue) : base(NameText, AgeValue) { }

	}
	/// <summary>
	/// Class to represent a Poker Player at a Poker Table.
	/// </summary>
	public class Player : PersonAtPokerTable
	{
		public Player() : this("Unnamed Player", 0) { }
		public Player(string name, int age) : base(name, age) { }
	}
	/// <summary>
	/// Abstract Class for any person that will be at a Poker Table.
	/// </summary>
	public abstract class PersonAtPokerTable : Hand
	{
		public string Name { get; set; }
		public int Age { get; set; }

		public PersonAtPokerTable(string name, int age)
		{
			Name = name;
			Age = age;
		}

		/// <summary>
		/// Method to allow a Person to fold their hand.
		/// </summary>
		public void Fold()
		{
			cards.Clear();
		}

		public override string ToString()
		{
			string result = "";

			if (cards.Count > 0)
			{
				var sortedCards = cards.OrderBy(x => x.Number).ToList();
				StringBuilder HandInfo = new StringBuilder();
				foreach (Card card in sortedCards)
					HandInfo.AppendLine("\t" + card.ToString());
				result = string.Format("{0, -30} \n{1}", Name + ":", HandInfo);
			}
			else
				result = string.Format("{0, -30} {1} years old", Name + ":", Age);

			return result;
		}
	}
	/// <summary>
	/// Enumeration for the Suit influenced from http://www.gamedev.net/topic/483122-c-poker-game-problem/
	/// </summary>
	public enum CardSuit
	{
		Spades,
		Hearts,
		Clubs,
		Diamonds,
	}
	/// <summary>
	/// Enumeration for Card Value influenced from link above
	/// </summary>
	public enum CardValue
	{
		Ace = 0,
		Two = 1,
		Three = 2,
		Four = 3,
		Five = 4,
		Six = 5,
		Seven = 6,
		Eight = 7,
		Nine = 8,
		Ten = 9,
		Jack = 10,
		Queen = 11,
		King = 12,
	}
	public class Card// : IComparable
	{
		public CardSuit Suit { get; private set; }
		public CardValue Number { get; private set; }

		public Card(CardSuit suit, CardValue number)
		{
			Suit = suit;
			Number = number;
		}

		public override string ToString()
		{
			return string.Format("{0} of {1}", Number, Suit);
		}

	}

	public class Cards
	{
		public Collection<Card> cards = new Collection<Card>();

		
		public void AddCard(Card card)
		{
			cards.Add(card);

		}

		public Card ReturnCard()
		{
			return cards.Pop();
		}
	}

	public class Hand : Cards
	{
	}

	public class Deck : Cards
	{
		public Deck()
		{
			foreach (CardSuit currentSuit in Enum.GetValues(typeof(CardSuit)))
				foreach (CardValue currentNumber in Enum.GetValues(typeof(CardValue)))
					AddCard(new Card(currentSuit, currentNumber));

			cards.Shuffle();
		}
		public Card DealCard()
		{
			return base.ReturnCard();
		}

		/// <summary>
		/// This Method overrides the Default .ToString Method which allows for printing of the Deck Object with a user friendly message. If no cards are in the Deck, it will also include a message for that.
		/// </summary>
		/// <returns>A user friendly representation of the Deck object.</returns>
		public override string ToString()
		{
			string result = "";
			if (cards.Count > 0)
				foreach (var card in cards)
					result += string.Format("{0}\n", card);
			else
				result = "No cards in the Deck.\n";

			return result;
		}
	}

	public static class ParsePokerHand
	{
		public static bool RoyalFlush(Player player)
		{
			// Need solution for TJQKA!!!!!!!!!
			return false; // :-(
		}

		public static bool FullHouse(Player player)
		{
			// Need to simplify!!!!
			var ThreeOfAKindForFullHouse =
				from card in player.cards
				group card by card.Number into c
				where c.Count() == 3
				select c.Key;

			var PairForFullHouse =
				from card in player.cards
				group card by card.Number into c
				where c.Count() == 2
				select c.Key;

			var ValidFullHouse =
						ThreeOfAKindForFullHouse.Except(PairForFullHouse);

			return (ThreeOfAKindForFullHouse.Count() > 0 && PairForFullHouse.Count() > 0 && ValidFullHouse.Count() > 0);
		}

		public static bool StraightFlush(Player player)
		{
			// Works! except for TJQKA..... :-/
			return (Straight(player) && Flush(player));
		}

		public static bool Flush(Player player)
		{
			// Works! Probably more complex that it needs to be
			var Flush =
				from card in player.cards
				group card by card.Suit into f
				where f.Select(fl => fl.Suit).Count() == 5
				select new { suit = f.Key };

			return Flush.Count() > 0;
		}

		public static bool Straight(Player player)
		{
			// Works! except for TJQKA
			var Straight =
				from card in player.cards
				orderby card.Number ascending
				select card.Number;

			// Returns True if all cards are sequential, else False
			// Influenced by http://stackoverflow.com/a/6150439/3042939
			return Straight.Zip(Straight.Skip(1), (a, b) => b - a).All(x => x == 1);
		}

		public static bool FourOfAKind(Player player)
		{
			return SetCheck(player, 4);
		}

		public static bool TwoPair(Player player)
		{
			return SetCheck(player, 2, 2);
		}

		public static bool ThreeOfAKind(Player player)
		{
			return SetCheck(player, 3);
		}

		public static bool OnePair(Player player)
		{
			return SetCheck(player, 2);
		}

		public static bool SetCheck(Player player, int numberCardsInSet, int numberOfSetsCheck = 1)
		{
			var SetsOfCards =
				from card in player.cards
				group card by card.Number into c
				where c.Count() == numberCardsInSet
				select c.Key;

			return SetsOfCards.Count() == numberOfSetsCheck;
		}

	}

	public static class ThreadSafeRandom
	{
		[ThreadStatic]
		private static Random Local;

		public static Random ThisThreadsRandom
		{
			get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
		}
	}

	static class MyExtensions
	{
		public static void Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n-- > 1)
			{
				int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		/// <summary>
		/// Removes the last element from a List of any Type
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="list">A List of any Type</param>
		/// <returns></returns>
		public static T Pop<T>(this Collection<T> list)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			var local = list[0];
			list.RemoveAt(0);
			return local;
		}
	}

}