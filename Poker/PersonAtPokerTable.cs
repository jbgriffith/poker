using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
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
}
