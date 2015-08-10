using System;
using System.Collections.Generic;

using Poker.NHib.DataAnnotations;

using NodaTime;

namespace Poker.DbModels {
	/// <summary>
	/// Class to represent a Player at a Card Table.
	/// </summary>
	public class Player : ModelBaseGuid {
		public virtual string Name { get; set; }
		public virtual DateTime? DateOfBirth { get; set; }
		public virtual DateTimeOffset CreatedUtc { get; set; }
		[Inverse]
		public virtual IList<Hand> Hands { get; set; }
		public virtual IList<Game> Games { get; set; }

		[NotPersisted]
		public virtual int Age { get { return (DateTime.Today - DateOfBirth.Value).Days / 365; } }
		[NotPersisted]
		public virtual Hand CurrentHand { get; set; }

		#region Methods
		public Player() : this("Unnamed Player") { }
		public Player(string name, DateTime? dob = null) {
			Name = name;
			DateOfBirth = dob;
			CreatedUtc = DateTimeOffset.UtcNow;
            Hands = new List<Hand>();
            Games = new List<Game>();
		}

		public override string ToString() {
			return string.Format("{0, -20}:{1} years old{2}{3}", Name, Age, Environment.NewLine, Hands.ToString());
		}

        public virtual void ArchiveHand() {
            Hands.Add(CurrentHand);
            CurrentHand = null;
        }
        #endregion
    }
}
