using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Poker {
	/// <summary>
	/// Abstract Class for any person that will be at a Poker Table.
	/// </summary>
	public abstract class PersonAtPokerTable : Hand {
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public int Age { get; set; }

		protected PersonAtPokerTable(string name, int age) {
			Name = name;
			Age = age;
		}

		/// <summary>
		/// Method to allow a Person to fold their hand.
		/// </summary>
		public void Fold() {
			CardCollection.Clear();
		}

		public override string ToString() {
			var sortedCards = CardCollection.OrderBy(x => x.Number);
			return string.Format("{0, -30}{1} years old{2}{#}", Name + ":", Age, Environment.NewLine, sortedCards.ToString());
		}

	}
}
