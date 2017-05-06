namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurrencyValues : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CurrencyValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrencyId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currencies", t => t.CurrencyId, cascadeDelete: true)
                .Index(t => t.CurrencyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CurrencyValues", "CurrencyId", "dbo.Currencies");
            DropIndex("dbo.CurrencyValues", new[] { "CurrencyId" });
            DropTable("dbo.CurrencyValues");
        }
    }
}
