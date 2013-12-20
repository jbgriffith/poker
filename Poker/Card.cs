using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
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
}
