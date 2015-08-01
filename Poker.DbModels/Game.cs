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
        public virtual IList<Player> Winners { get; set; }


        //private Deck _deck = new Deck();
        //private List<Player> players_ = new List<Player>();
        //private List<Player> winners_ = new List<Player>();
        //private List<List<Card>> _winningHands = new List<List<Card>>();

        //public Deck Deck { get { return _deck; } }
        //public List<Player> Players { get { return players_; } }
        //public List<Player> Winners { get { return winners_; } }
        //public List<List<Card>> WinningHands { get { return _winningHands; } }

        public Game() { }

        public Game(List<Player> players) {
            Deck = new Deck();
            Players = new List<Player>();

            foreach (var player in players) {
                Players.Add(player);
                player.Games.Add(this);
            }

        }
    }
}
