using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.DbModels {
	public class Game : ModelBaseGuid {
		private Deck _deck = new Deck();
		private List<Player> players_ = new List<Player>();
		private List<Player> winners_ = new List<Player>();
		private List<List<Card>> _winningHands = new List<List<Card>>();

		public virtual Deck Deck { get { return _deck; } }
		public virtual List<Player> Players { get { return players_; } }
		public virtual List<Player> Winners { get { return winners_; } }
		public virtual List<List<Card>> WinningHands { get { return _winningHands; } }

		public Game() {}

		public Game(ICollection<Player> players) {
			foreach (var player in players)
				player.Hand.Clear();
			players_.AddRange(players);
		}
	}
}
