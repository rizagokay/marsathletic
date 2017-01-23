namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Extension = c.String(),
                        FullName = c.String(),
                        PurchaseOrder_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PurchaseOrders", t => t.PurchaseOrder_Id)
                .Index(t => t.PurchaseOrder_Id);
            
            CreateTable(
                "dbo.PurchaseOrderDocuments",
                c => new
                    {
                        PurchaseOrderId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PurchaseOrderId, t.DocumentId })
                .ForeignKey("dbo.PurchaseOrders", t => t.PurchaseOrderId, cascadeDelete: true)
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .Index(t => t.PurchaseOrderId)
                .Index(t => t.DocumentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseOrderDocuments", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.PurchaseOrderDocuments", "PurchaseOrderId", "dbo.PurchaseOrders");
            DropForeignKey("dbo.Documents", "PurchaseOrder_Id", "dbo.PurchaseOrders");
            DropIndex("dbo.PurchaseOrderDocuments", new[] { "DocumentId" });
            DropIndex("dbo.PurchaseOrderDocuments", new[] { "PurchaseOrderId" });
            DropIndex("dbo.Documents", new[] { "PurchaseOrder_Id" });
            DropTable("dbo.PurchaseOrderDocuments");
            DropTable("dbo.Documents");
        }
    }
}
