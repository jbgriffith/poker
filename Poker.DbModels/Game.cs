
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.DbModels {
	public class Game {
		public Guid Id { get; set; }
		public virtual Deck Deck { get; set; }
		public ICollection<Player> Players { get; set; }
		public DateTimeOffset CreatedUtc { get; set; }

		public Game() { } // required for Nhibernate & EF...

		public Game(HashSet<Player> players) {
			Id = GuidComb.Generate();
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
