using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
	class Dealer : PersonAtPokerTable
	{
		public Dealer() : this("Unnamed Dealer", 0) { }
		public Dealer(string NameText, int AgeValue) : base(NameText, AgeValue) { }

	}

}
