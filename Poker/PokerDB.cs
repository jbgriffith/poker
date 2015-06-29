using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Poker {
	public class PokerContext : DbContext {
		//public class ForceDeleteInitializer : IDatabaseInitializer<PokerContext> {
		//	private readonly IDatabaseInitializer<PokerContext> _initializer;

		//	public ForceDeleteInitializer(IDatabaseInitializer<PokerContext> innerInitializer) {
		//		_initializer = innerInitializer;
		//	}
		//	public void InitializeDatabase(PokerContext PokerContext) {
		//		PokerContext.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "ALTER DATABASE [" + PokerContext.Database.Connection.Database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
		//		_initializer.InitializeDatabase(PokerContext);
		//	}
		//}

		public PokerContext()
			: base() {
				Database.SetInitializer<PokerContext>(new DropCreateDatabaseIfModelChanges<PokerContext>());
		}
		//public DbSet<Card> Deck { get; set; }
		public DbSet<Card> Hand { get; set; }
		public DbSet<Player> Players { get; set; }


	}
}
