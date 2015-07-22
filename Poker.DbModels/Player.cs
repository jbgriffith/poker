using System;
using System.Collections.Generic;

namespace Poker.DbModels {
	/// <summary>
	/// Class to represent a Player at a Card Table.
	/// </summary>
	public class Player : ModelBaseGuid {
		public string Name { get; set; }
		public int Age { get; set; }

		#region private backed properties
		private Hand _hand = new Hand();
		public Hand Hand { get { return _hand; } }

		private List<Game> _games = new List<Game>();
		public List<Game> Games { get { return _games; } } //need to add methods
		#endregion
		//[Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		//public DateTime CreatedUtc { get; set; }

		#region Methods
		public Player() : this("Unnamed Player", 0) { }
		public Player(string name, int age) {
			Name = name;
			Age = age;
		}

		public override string ToString() {
			return string.Format("{0, -20}:{1} years old{2}{3}", Name, Age, Environment.NewLine, Hand.ToString());
		}
		#endregion
	}
}
