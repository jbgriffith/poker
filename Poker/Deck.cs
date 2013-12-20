using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
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
}
