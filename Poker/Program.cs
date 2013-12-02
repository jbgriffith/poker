using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Poker
{
	class Program
	{
		static void Main(string[] args)
		{
			for (int w = 0; w < 10000; w++)
			{
				Collection<string> PlayerNames = new Collection<string> { "Holen", "Garrili", "Anelm", "Chal", "Metin" };
				Collection<Player> PlayersAtPokerTable = new Collection<Player>();
				int numPLayers = PlayerNames.Count();

				// Mix it up a little
				PlayerNames.ShuffleIt();
				
				for (int j = 0; j < numPLayers; j++)
				{
					PlayersAtPokerTable.Add(new Player(PlayerNames[j], j));
				}

				//int numberOfDecks = (int)Math.Ceiling((5.0 * (numPLayers + 1)) / 52);
				Deck decks = new Deck();

				// Deal out the Cards to the players
				for (int j = 0; j < 5; j++)
				{
					foreach (var player in PlayersAtPokerTable)
					{
						player.AddCard(decks.DealCard());
					}
				}


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
					if (PokerScoring.RoyalFlush(player)) 
					{
						break;
					}
					if (PokerScoring.StraightFlush(player))
					{
						break;
					}
					else if (PokerScoring.FullHouse(player))
					{
						break;
					}
					else if (PokerScoring.FourOfAKind(player))
					{
						break;
					}
					else if (PokerScoring.Flush(player))
					{
						break;
					}
					else if (PokerScoring.Straight(player))
					{
						break;
					}
					else if (PokerScoring.ThreeOfAKind(player))
					{
						break;
					}
					else if (PokerScoring.TwoPair(player))
					{
						break;
					}
					else if (PokerScoring.OnePair(player))
					{
						break;
					}
					//else if (PokerScoring.HighCard(player))
					//{
					//	break;
					//}
				}
#if !DEBUG
			Console.WriteLine("Press any key to close...");
			Console.ReadLine();
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
		public Player(string NameText, int AgeValue) : base(NameText, AgeValue) { }
	}
	/// <summary>
	/// Abstract Class for any person that will be at a Poker Table.
	/// </summary>
	public abstract class PersonAtPokerTable : Hand
	{
		public string Name { get; set; }
		public int Age { get; set; }

		public PersonAtPokerTable() : this("Unnamed person", 0) { }
		public PersonAtPokerTable(string NameText, int AgeValue)
		{
			Name = NameText;
			Age = AgeValue;
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
				//cards.Sort();
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
	/// Enums for the Suit influenced from http://www.gamedev.net/topic/483122-c-poker-game-problem/
	/// </summary>
	public enum CardSuit
	{
		Spades,
		Hearts,
		Clubs,
		Diamonds,
	}
	/// <summary>
	/// Enum for Card Value influenced from linek above
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
	public class Card : IComparable
	{
		public CardSuit Suit { get; set; }
		public CardValue Number { get; set; }

		public Card(CardSuit suit, CardValue number)
		{
			Suit = suit;
			Number = number;
		}

		public override string ToString()
		{
			return string.Format("{0} of {1}", Number, Suit);
		}

		public int CompareTo(object obj)
		{
			if (obj == null) return 1;

			Card otherCard = obj as Card;
			if (otherCard != null)
				return this.Number.CompareTo(otherCard.Number);
			else
				throw new ArgumentException("Object is not a Card");
		}

		public static int Compare(Card left, Card right)
		{
			if (object.ReferenceEquals(left, right))
			{
				return 0;
			}
			if (object.ReferenceEquals(left, null))
			{
				return -1;
			}
			return left.CompareTo(right);
		}

		public override bool Equals(object obj)
		{
			Card other = obj as Card; //avoid double casting 
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			return this.CompareTo(other) == 0;
		}

		public override int GetHashCode()
		{
			char[] c = this.ToString().ToCharArray();
			return (int)c[0];
		}

		public static bool operator ==(Card left, Card right)
		{
			if (object.ReferenceEquals(left, null))
			{
				return object.ReferenceEquals(right, null);
			}
			return left.Equals(right);
		}

		public static bool operator !=(Card left, Card right)
		{
			return !(left == right);
		}

		public static bool operator <(Card left, Card right)
		{
			return (Compare(left, right) < 0);
		}

		public static bool operator >(Card left, Card right)
		{
			return (Compare(left, right) > 0);
		}
	}

	public abstract class Cards
	{
		public Collection<Card> cards = new Collection<Card>();

		public void AddCard(Card card)
		{
			cards.Add(card);
		}

		public void RemoveCard(Card card)
		{
			if (cards.Contains(card) == true)
				cards.Remove(card);
			else
				throw new InvalidOperationException("The card is not in the deck");
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
			cards.ShuffleIt();
		}
		public Card DealCard()
		{
			return base.ReturnCard();
		}

		/// <summary>
		/// This Method overrides the Default .ToString Method which allows for printing of the Deck Object with a user friendly message. If no cards are in the Deck, it will also include a message for that.
		/// </summary>
		/// <returns>A user friendly representastion of the Deck object.</returns>
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

	public class PokerScoring
	{
		public static bool FullHouse(Player player)
		{
			//Full House
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

		public static bool RoyalFlush(Player player)
		{
			// Need solution for TJQKA!!!!!!!!!
			return false;
		}

		public static bool StraightFlush(Player player)
		{
			// Straight Flush
			// Works! except for TJQKA
			return (Straight(player) && Flush(player));
		}

		public static bool Flush(Player player)
		{
			// Works!
			var Flush =
				from card in player.cards
				group card by card.Suit into f
				where f.Select(fl => fl.Suit).Count() == 5
				select new { suit = f.Key, obj = f, NumOfValues = f.Count() };

			return Flush.Count() > 0;
		}

		public static bool Straight(Player player)
		{
			// Works! except for TJQKA
			var Straight =
				from card in player.cards
				orderby card.Number ascending
				select card.Number;

			// Makes sure that all ofthe cards are sequential
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
			return SetCheck(player, 4);
		}

		public static bool SetCheck(Player player, int NumSets, int NumSetCheck = 1 )
		{
			var SetsOfCards =
				from card in player.cards
				group card by card.Number into c
				where c.Count() == NumSets
				select new { NumberOfValues = c.Count() };

			return SetsOfCards.Count() >= NumSetCheck;
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
		public static void ShuffleIt<T>(this IList<T> list)
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