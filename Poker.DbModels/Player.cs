using Poker.NHib.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Poker.DbModels {
	/// <summary>
	/// Class to represent a Player at a Card Table.
	/// </summary>
	public class Player : ModelBaseGuid {
		public virtual string Name { get; set; }
		public virtual int Age { get; set; }
        public virtual Hand CurrentHand { get; set; }

        #region private backed properties
        //private Hand _hand = new Hand();
        //public Hand Hand { get { return _hand; } }

        [ManyToMany]
        public virtual ISet<Hand> Hands { get; set; }
        public virtual IList<Game> Games { get; set; }
        //private IList<Game> _games = new List<Game>();
		//public IList<Game> Games { get { return _games; } } //need to add methods
		#endregion
		//[Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		//public DateTime CreatedUtc { get; set; }

		#region Methods
		public Player() : this("Unnamed Player", 0) { }
		public Player(string name, int age) {
			Name = name;
			Age = age;
            //CurrentHand = new Hand();
            Hands = new HashSet<Hand>();
            Games = new List<Game>();
		}

		public override string ToString() {
			return string.Format("{0, -20}:{1} years old{2}{3}", Name, Age, Environment.NewLine, Hands.ToString());
		}

        public virtual void SaveHand() {
            Hands.Add(CurrentHand);
            CurrentHand = null;
        }
        #endregion
    }
}
