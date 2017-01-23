namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentUpdated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Documents", "PurchaseOrder_Id", "dbo.PurchaseOrders");
            DropIndex("dbo.Documents", new[] { "PurchaseOrder_Id" });
            DropColumn("dbo.Documents", "PurchaseOrder_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "PurchaseOrder_Id", c => c.Int());
            CreateIndex("dbo.Documents", "PurchaseOrder_Id");
            AddForeignKey("dbo.Documents", "PurchaseOrder_Id", "dbo.PurchaseOrders", "Id");
        }
    }
}
