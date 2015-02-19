using System;

namespace Poker
{
	public class Deck : Cards
	{
		public Deck()
		{
			foreach (Card.CardSuit currentSuit in Enum.GetValues(typeof(Card.CardSuit)))
				foreach (Card.CardValue currentNumber in Enum.GetValues(typeof(Card.CardValue)))
					AddCard(new Card(currentSuit, currentNumber));

			CardCollection.QuickShuffle();
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
			if (CardCollection.Count > 0)
				foreach (var card in CardCollection)
					result += string.Format("{0}{1}", card, Environment.NewLine);
			else
				result = string.Format("No cards in the Deck.{0}", Environment.NewLine);

			return result;
		}
	}
}
