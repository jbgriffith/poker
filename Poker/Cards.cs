using System.Collections.ObjectModel;

namespace Poker
{
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
}
