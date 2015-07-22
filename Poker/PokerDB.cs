//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Data.Entity;
//using System.Data.Entity.Migrations;
//using System.Data.Entity.SqlServer;
//using System.Data.Entity.Migrations.Model;
//using System.Transactions;

//namespace Poker {
//	public class PokerContext : DbContext {

//		public void AddToDb(IEnumerable<Player> someCollectionOfEntitiesToInsert) {
//			using (TransactionScope scope = new TransactionScope()) {
//				PokerContext context = null;
//				try {
//					context = new PokerContext();
//					context.Configuration.AutoDetectChangesEnabled = false;

//					int count = 0;
//					foreach (var entityToInsert in someCollectionOfEntitiesToInsert) {
//						++count;
//						context = AddToContext(context, entityToInsert, count, 100, true);
//					}

//					context.SaveChanges();
//				}
//				finally {
//					if (context != null)
//						context.Dispose();
//				}

//				scope.Complete();
//			}

//	}
//		private PokerContext AddToContext(PokerContext context, Player entity, int count, int commitCount, bool recreateContext) {
//			context.Set<Player>().Add(entity);

//			if (count % commitCount == 0) {
//				context.SaveChanges();
//				if (recreateContext) {
//					context.Dispose();
//					context = new PokerContext();
//					context.Configuration.AutoDetectChangesEnabled = false;
//				}
//			}

//			return context;
//		}
//		//public class ForceDeleteInitializer : IDatabaseInitializer<PokerContext> {
//		//	private readonly IDatabaseInitializer<PokerContext> _initializer;

//		//	public ForceDeleteInitializer(IDatabaseInitializer<PokerContext> innerInitializer) {
//		//		_initializer = innerInitializer;
//		//	}
//		//	public void InitializeDatabase(PokerContext PokerContext) {
//		//		PokerContext.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "ALTER DATABASE [" + PokerContext.Database.Connection.Database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
//		//		_initializer.InitializeDatabase(PokerContext);
//		//	}
//		//}


//		//internal sealed class Configuration : DbMigrationsConfiguration<PokerContext> {
//		//	public Configuration() {
//		//		AutomaticMigrationsEnabled = false;
//		//		SetSqlGenerator("System.Data.SqlClient", new CustomSqlServerMigrationSqlGenerator());
//		//	}
//		//}

//		//internal class CustomSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator {
//		//	protected override void Generate(AddColumnOperation addColumnOperation) {
//		//		SetCreatedUtcColumn(addColumnOperation.Column);

//		//		base.Generate(addColumnOperation);
//		//	}

//		//	protected override void Generate(CreateTableOperation createTableOperation) {
//		//		SetCreatedUtcColumn(createTableOperation.Columns);

//		//		base.Generate(createTableOperation);
//		//	}

//		//	private static void SetCreatedUtcColumn(IEnumerable<ColumnModel> columns) {
//		//		foreach (var columnModel in columns) {
//		//			SetCreatedUtcColumn(columnModel);
//		//		}
//		//	}

//		//	private static void SetCreatedUtcColumn(PropertyModel column) {
//		//		if (column.Name == "CreatedUtc") {
//		//			column.DefaultValueSql = "GETUTCDATE()";
//		//		}
//		//	}
//		//}

//		public PokerContext()
//			: base() {
//				Database.SetInitializer<PokerContext>(new DropCreateDatabaseIfModelChanges<PokerContext>());
//		}
//		//public DbSet<Card> Deck { get; set; }
//		public DbSet<Card> IndividualCards { get; set; }
//		public DbSet<Cards> CardSets { get; set; }
//		//public DbSet<Hand> Hands { get; set; }
//		//public DbSet<Deck> Decks { get; set; }
//		public DbSet<Player> Players { get; set; }
//		public DbSet<Game> Games { get; set; }

//		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
//			modelBuilder.Entity<Player>().HasMany(t => t.Games).WithMany(t => t.Players);
//		}

//	}
//}
