namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurrencyAddedPurchaseDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseDetails", "Currency_Id", c => c.Int());
            CreateIndex("dbo.PurchaseDetails", "Currency_Id");
            AddForeignKey("dbo.PurchaseDetails", "Currency_Id", "dbo.Currencies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseDetails", "Currency_Id", "dbo.Currencies");
            DropIndex("dbo.PurchaseDetails", new[] { "Currency_Id" });
            DropColumn("dbo.PurchaseDetails", "Currency_Id");
        }
    }
}
