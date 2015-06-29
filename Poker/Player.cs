using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Poker {
	/// <summary>
	/// Class to represent a Poker Player at a Poker Table.
	/// </summary>
	public class Player {
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public int Age { get; set; }
		public int Score { get; set; }
		public List<int> ScoreDetails { get; set; }
		public ICollection<Card> cards { get; set; }
		public Player() : this("Unnamed Player", 0) { }
		public Player(string name, int age) {
			Name = name;
			Age = age;
			cards = new List<Card>();
		}

		//public ScoredHand ScoredHand { get; set; }

		/// <summary>
		/// Method to allow a Person to fold their hand.
		/// </summary>
		public void Fold() {
			cards.Clear();
		}

		public void AddCard(Card card) {
			cards.Add(card);
		}

		public override string ToString() {
			return string.Format("{0, -20}:{1} years old{2}{3}", Name, Age, Environment.NewLine, string.Join(Environment.NewLine, cards.OrderBy(x => x.Number).Select(x => x.ToString())));
		}
	}
}
