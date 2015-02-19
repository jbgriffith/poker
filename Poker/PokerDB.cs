using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Poker
{
	public class PokerContext : DbContext
	{
		//public DbSet<Card> Deck { get; set; }
		public DbSet<Card> Hand { get; set; }
		public DbSet<Player> Players { get; set; }
		public DbSet<PersonAtPokerTable> People { get; set; }
	}
}
