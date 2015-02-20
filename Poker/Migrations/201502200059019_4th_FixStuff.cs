namespace Poker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _4th_FixStuff : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cards", "PersonAtPokerTable_Id", "dbo.PersonAtPokerTables");
            DropIndex("dbo.Cards", new[] { "PersonAtPokerTable_Id" });
            DropColumn("dbo.Cards", "PersonAtPokerTable_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cards", "PersonAtPokerTable_Id", c => c.Int());
            CreateIndex("dbo.Cards", "PersonAtPokerTable_Id");
            AddForeignKey("dbo.Cards", "PersonAtPokerTable_Id", "dbo.PersonAtPokerTables", "Id");
        }
    }
}
