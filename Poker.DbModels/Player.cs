using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using NodaTime;

namespace Poker.DbModels {
	/// <summary>
	/// Class to represent a Player at a Card Table.
	/// </summary>
	public class Player {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public DateTimeOffset CreatedUtc { get; set; }
		public virtual ICollection<Hand> Hands { get; set; }
		public virtual ICollection<Game> Games { get; set; }

		[NotMapped]
		public int Age { get { return (DateTime.Today - DateOfBirth.Value).Days / 365; } }
		[NotMapped]
		public Hand CurrentHand { get; set; }

		#region Methods
		public Player() : this("Unnamed Player") { }
		public Player(string name, DateTime? dob = null) {
			Id = GuidComb.Generate();
			Name = name;
			DateOfBirth = dob;
			CreatedUtc = DateTimeOffset.UtcNow;
            Hands = new List<Hand>();
            Games = new List<Game>();
			CurrentHand = new Hand();
		}

		public override string ToString() {
			return string.Format("{0, -20}:{1} years old{2}{3}", Name, Age, Environment.NewLine, CurrentHand.ToString());
		}

        public void ArchiveHand() {
            Hands.Add(CurrentHand);
            CurrentHand = null;
        }
        #endregion
    }
}
