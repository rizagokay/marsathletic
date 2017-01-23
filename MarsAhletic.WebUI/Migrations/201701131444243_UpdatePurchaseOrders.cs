namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePurchaseOrders : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PurchaseOrders", new[] { "CostCenter_Id" });
            AlterColumn("dbo.PurchaseOrders", "CostCenter_Id", c => c.Int());
            CreateIndex("dbo.PurchaseOrders", "CostCenter_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PurchaseOrders", new[] { "CostCenter_Id" });
            AlterColumn("dbo.PurchaseOrders", "CostCenter_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.PurchaseOrders", "CostCenter_Id");
        }
    }
}
