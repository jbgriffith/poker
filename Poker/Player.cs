using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Poker
{
	/// <summary>
	/// Class to represent a Poker Player at a Poker Table.
	/// </summary>
	public class Player : PersonAtPokerTable
	{
		[Key]
		public new int Id { get; set; }

		public Player() : this("Unnamed Player", 0) { }
		public Player(string name, int age) : base(name, age) { }

		public int Score { get; set; }
	}
}
