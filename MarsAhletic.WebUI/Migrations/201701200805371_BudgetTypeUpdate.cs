namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BudgetTypeUpdate : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PurchaseDetails", new[] { "BudgetType_Id" });
            AlterColumn("dbo.PurchaseDetails", "BudgetType_Id", c => c.Int());
            CreateIndex("dbo.PurchaseDetails", "BudgetType_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PurchaseDetails", new[] { "BudgetType_Id" });
            AlterColumn("dbo.PurchaseDetails", "BudgetType_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.PurchaseDetails", "BudgetType_Id");
        }
    }
}
