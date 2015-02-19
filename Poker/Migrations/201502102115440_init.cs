namespace Poker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonAtPokerTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Age = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Player_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PersonAtPokerTables", t => t.Player_Id)
                .Index(t => t.Player_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonAtPokerTables", "Player_Id", "dbo.PersonAtPokerTables");
            DropIndex("dbo.PersonAtPokerTables", new[] { "Player_Id" });
            DropTable("dbo.PersonAtPokerTables");
        }
    }
}
