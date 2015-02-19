namespace Poker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2nd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Suit = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        PersonAtPokerTable_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PersonAtPokerTables", t => t.PersonAtPokerTable_Id)
                .Index(t => t.PersonAtPokerTable_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cards", "PersonAtPokerTable_Id", "dbo.PersonAtPokerTables");
            DropIndex("dbo.Cards", new[] { "PersonAtPokerTable_Id" });
            DropTable("dbo.Cards");
        }
    }
}
