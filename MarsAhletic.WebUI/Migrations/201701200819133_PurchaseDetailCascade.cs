namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PurchaseDetailCascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PurchaseDetails", "PurchaseOrder_Id", "dbo.PurchaseOrders");
            DropIndex("dbo.PurchaseDetails", new[] { "PurchaseOrder_Id" });
            AddColumn("dbo.PurchaseOrders", "PurchaseOrderCode", c => c.String());
            AlterColumn("dbo.PurchaseDetails", "PurchaseOrder_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.PurchaseDetails", "PurchaseOrder_Id");
            AddForeignKey("dbo.PurchaseDetails", "PurchaseOrder_Id", "dbo.PurchaseOrders", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseDetails", "PurchaseOrder_Id", "dbo.PurchaseOrders");
            DropIndex("dbo.PurchaseDetails", new[] { "PurchaseOrder_Id" });
            AlterColumn("dbo.PurchaseDetails", "PurchaseOrder_Id", c => c.Int());
            DropColumn("dbo.PurchaseOrders", "PurchaseOrderCode");
            CreateIndex("dbo.PurchaseDetails", "PurchaseOrder_Id");
            AddForeignKey("dbo.PurchaseDetails", "PurchaseOrder_Id", "dbo.PurchaseOrders", "Id");
        }
    }
}
