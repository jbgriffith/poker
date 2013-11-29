using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
	class Program
	{
		static void Main(string[] args)
		{
			for (int w = 0; w < 10000; w++)
			{


				//List<string> InitialQuestions = new List<string> { "What is your name? ", "How old are you? ", "How many people would you like to play with? ", "How many rounds would you like to play? " };
				//List<string> InitialResponses = new List<string> { "", "", "", "" };

				//int i = 0;
				//while ( i < InitialQuestions.Count() )
				//{
				//	Console.WriteLine(InitialQuestions[i]);
				//	InitialResponses[i] = Console.ReadLine();

				//	if (i != 0 && !InitialResponses[i].IsNumber())
				//	{
				//		i--;
				//	}
				//	i++;
				//}

				//List<string> PlayerNames = new List<string> { "Holen", "Garrili", "Anelm", "Chal", "Metin", "Ahasu", "Omvori", "Mosbwar", "Oldrod", "Ymosi", "Umll", "Omagekelat", "Awpoli", "Tanys", "Rilen", "Nytolo", "Oldk", "Steyfaugh", "Ucide", "Verivor", "Omgarlye", "Noes", "Skelokin", "Rakust", "Ackez", "Esspqua", "Sulerd", "Honabel", "Undyer", "Undy", "Hinold", "Worageash", "Turay", "Aoni", "Achl", "Pybij", "Daor", "Keller", "Awther", "Tasnys", "Untyq", "Perch", "Issum", "Erwther", "Delabur", "Kymiss", "Toria", "Qua'rad", "Shykalche", "Engtur", "Hatlver", "Belton", "Yenda", "Ildendar", "Oldc", "Eror", "Oldolor", "Endiv", "Dueldard", "Whethin", "Is'aughu", "Eskeli", "Esttiaril", "Reyso", "Drounper-den", "Ildhin", "Dadyn", "Nalryn", "Dania", "Sytir", "Cer'cha", "Blyech", "Nijere", "Etura", "Bantur", "Drajmor", "Okaly", "Rakchanal", "Orutin", "Umag", "Rynshye", "Rynale", "Nos", "Molril", "Yanonu", "Ermor", "Tiaroth", "Daronal", "Tydis", "Owaru", "Tateh", "Rynathris", "Untgaright", "Ingoneld", "Inghat", "Emund", "Swoesset", "Quoryn", "Teough", "Ustjque", "Oasa", "Kysay", "Ryness", "Danusk", "Pheassul", "Skeltai", "Tiorelm", "Ementh", "Hyr", "Thereim'n", "Therbia", "Angale", "Fasik", "Tostas", "Echchaban" };
				//Dealer theDealer = new Dealer("Joe the Dealer", 45);
				//bool value = Misc.IsNumber(InitialResponses[1]);
				//int NumPLayers = 0;
				//Misc.TryParseIsInt32(InitialResponses[2], out NumPLayers);
				List<string> PlayerNames = new List<string> { "Holen", "Garrili", "Anelm", "Chal", "Metin" };
				List<Player> PlayersAtPokerTable = new List<Player>();
				int numPLayers = 0;
				numPLayers = PlayerNames.Count();
				// Mix it up a little
				PlayerNames.Shuffle();
				for (int j = 0; j < numPLayers; j++)
				{
					PlayersAtPokerTable.Add(new Player(PlayerNames[j], j));
				}
				//PlayersAtPokerTable.Add(new Player(InitialResponses[0], Convert.ToInt32(InitialResponses[1])));
			
				int numberOfDecks = (int)Math.Ceiling((5.0 * (numPLayers + 1)) / 52.0);
				Deck Decks = new Deck(numberOfDecks);

				for (int j = 0; j < 5; j++) 
				{
					foreach (var player in PlayersAtPokerTable)
					{
						player.AddToHand(Decks.Deal());
					}
				}
				foreach (var player in PlayersAtPokerTable)
				{
					//Console.WriteLine(player);
					//Console.WriteLine(Scoring.NumCardsSameCheck(player.HandOfCards, 2));
					var GroupedCardValue =
						from card in player.HandOfCards
						group card by card.integerValue;
				
					foreach (var value in GroupedCardValue) {
						
						if (value.Count() > 3)
						{
							Console.WriteLine("Game Number {0}", w);
							Console.WriteLine("Key Value:{0}\nKey Value Count {1}", value.Key, value.Count());
							Console.WriteLine("Damn!!!!");
							Console.ReadLine();
						}
					}
				}
				//Console.WriteLine(Decks);
			}
#if !DEBUG
			Console.WriteLine("Press any key to close...");
			Console.ReadLine();
#endif
			
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
	class Player : PersonAtPokerTable
	{
		public Player() : this("Unnamed Player", 0) { }
		public Player(string NameText, int AgeValue) : base(NameText, AgeValue) { }
	}
	/// <summary>
	/// Abstract Class for any person that will be at a Poker Table.
	/// </summary>
	abstract class PersonAtPokerTable
	{
		public string Name { get; set; }
		public int Age { get; set; }
		public List<Card> HandOfCards = new List<Card>();

		public PersonAtPokerTable() : this("Unnamed person", 0) { }
		public PersonAtPokerTable(string NameText, int AgeValue)
		{
			Name = NameText;
			Age = AgeValue;
		}
		/// <summary>
		/// Method to allow a Person to receive a card which will add to their HandOfCards.
		/// </summary>
		/// <param name="card">a Card Object</param>
		public void AddToHand(Card card)
		{
			HandOfCards.Add(card);
		}
		/// <summary>
		/// Method to allow a Person to fold their hand.
		/// </summary>
		public void Fold()
		{
			HandOfCards.Clear();
		}

		public override string ToString()
		{
			string result = "";

			if (HandOfCards.Count() > 0)
			{
				HandOfCards.Sort();
				StringBuilder HandInfo = new StringBuilder();
				foreach (Card card in HandOfCards)
				{
					HandInfo.AppendLine("\t" + card.ToString());
				}
				result = string.Format("{0, -30} \n{1}", Name + ":", HandInfo);
			}
			else
				result = string.Format("{0, -30} {1} years old", Name + ":", Age);

			return result;
		}
	}

	public class Card : IComparable
	{
		public int CompareTo(object obj)
		{
			if (obj == null) return 1;

			Card otherCard = obj as Card;
			if (otherCard != null)
				return this.integerValue.CompareTo(otherCard.integerValue);
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

		public int integerValue;
		public string suit;
		public List<string> integerValueNames = new List<string>(new string[] { "Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King" });

		public Card(int intValue, string suitText)
		{
			suit = suitText;
			integerValue = intValue % 13;
		}
		 
		public override string ToString()
		{
			return string.Format("{0} of {1}", integerValueNames[integerValue], suit);
		}
	}

	/// <summary>
	/// Class to represent A Deck of Cards (Cards Class)
	/// </summary>
	public class Deck
	{
		public List<string> suits = new List<string> { "Hearts", "Spades", "Diamonds", "Clubs" };
		public List<Card> cards = new List<Card>();
		/// <summary>
		/// Default Contrustor creates one Deck's worth of Cards
		/// </summary>
		public Deck() : this(1) { }
		/// <summary>
		/// Overriding the 
		/// </summary>
		/// <param name="numberOfDecks"></param>
		public Deck(int numberOfDecks)
		{
			for (int i = 0; i < 13 * numberOfDecks; i++)
			{
				foreach (var suit in suits)
				{
					cards.Add(new Card(i, suit));
				}
			}
			cards.Shuffle();
		}

		/// <summary>
		/// This Method returns (Deals) one Card object. 
		/// </summary>
		/// <returns>Single Card Object</returns>
		public Card Deal()
		{
			return cards.Pop();
		}

		/// <summary>
		/// This Method overrides the Default .ToString Method which allows for printing of the Deck Object with a user friendly message. If no cards are in the Deck, it will also include a message for that.
		/// </summary>
		/// <returns>A user friendly representastion of the Deck object.</returns>
		public override string ToString()
		{
			string result = "";
			if (cards.Count > 0)
			{
				foreach (var card in cards)
				{
					result += string.Format("{0}\n", card);
				}
			}
			else
			{
				result = "No cards in the Deck.\n";
			}
			return result;
		}
	}

	/// <summary>
	/// Class for Scoring poker hands.
	/// </summary>
	public static class Scoring
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cards"></param>
		/// <returns></returns>
		public static List<int> ValueOfCards(List<Card> cards)
		{
			List<int> Values = new List<int>();
			foreach (var card in cards)
			{
				Values.Add(card.integerValue % 13);
			}
			return Values;
		}

		/// <summary>
		/// Checks
		/// </summary>
		/// <param name="cards">List of Card Objects</param>
		/// <param name="numCardsSame">Number of cards that need to be the same to return True</param>
		/// <returns>True if cards has numSameIntegerValue cards that have the same value, otherwise it returns False</returns>
		//public static bool NumCardsSameCheck(List<Card> cards, int numCardsSame)
		//{
			
		//	List<int> Values = new List<int>(ValueOfCards(cards));
		//	Values.Sort();
		//	for (int i = 0; i < cards.Count - numCardsSame + 1; i++)
		//	{
		//		List<Card> firstSetOfNCards = cards.Sort();
		//		return true;
		//	}
		//	return true;
		//}
	}

	/// <summary>
	/// Miscelanious Methods including some Generic Methods
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Method to test if value can be converted into a 32-bit Integer.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsNumber(this String value)
		{
			return value.ToCharArray().Where(x => !Char.IsDigit(x)).Count() == 0;
		}

		/// <summary>
		/// Method to test if value can be converted into a 32-bit Integer.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="result"></param>
		/// <returns>True if it can be conveted to an 32-bit Integer, otherwise False</returns>
		public static bool TryParseIsInt32(string value, out int result)
		{
			return Int32.TryParse(value, out result);
		}
	}

	public static class ListExtensions
	{
		/// <summary>
		/// This Generic Method shuffles (reorginizes) the order of elements in a List of any Type.
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="theList">A list of any Type</param>
		public static void Shuffle<T>(this IList<T> theList)
		{
			Random rng = new Random();
			int n = theList.Count;
			int NumLoops = 0;
			while (n-- > 1)
			{
				int k = rng.Next(n + 1);
				T value = theList[k];
				theList[k] = theList[n];
				theList[n] = value;
				if (k % 2 == rng.Next(0, 1))
					n++;
				NumLoops++;
			}
#if ! DEBUG
			Console.WriteLine("\n\nShuffled {0} times...\n\n", NumLoops);
#endif
		}

		/// <summary>
		/// Removes the last element from a List of any Type
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="theList">A List of any Type</param>
		/// <returns></returns>
		public static T Pop<T>(this List<T> theList)
		{
			var local = theList[0];
			theList.RemoveAt(0);
			return local;
		}
	} 
}
