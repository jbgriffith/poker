using Poker.NHib.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.DbModels {
	public class Game : ModelBaseGuid {
		public virtual Deck Deck { get; set; }
		[ManyToMany]
		public virtual IList<Player> Players { get; set; }
		public virtual DateTimeOffset CreatedUtc { get; set; }

		public Game() { } // required for Nhibernate...

		public Game(HashSet<Player> players) {
			Deck = new Deck();
			Players = new List<Player>();
			CreatedUtc = DateTimeOffset.UtcNow;

			//Players.AddRange(players);
			foreach (var player in players) {
				Players.Add(player);
				player.Games.Add(this);
			}
		}

		public void ArchivePlayersHands() {
			foreach (var player in Players)
				player.ArchiveHand();
		}
	}
}
