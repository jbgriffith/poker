using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
	/// <summary>
	/// Class to represent a Poker Player at a Poker Table.
	/// </summary>
	public class Player : PersonAtPokerTable
	{
		public Player() : this("Unnamed Player", 0) { }
		public Player(string name, int age) : base(name, age) { }
	}
}
