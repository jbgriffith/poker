namespace Poker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3rd_AddScore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonAtPokerTables", "Score", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonAtPokerTables", "Score");
        }
    }
}
