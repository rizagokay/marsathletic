namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PurchaseOrderUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrders", "MFilesId", c => c.String());
            AddColumn("dbo.PurchaseOrders", "MFilesProcessId", c => c.Int(nullable: false));
            AddColumn("dbo.PurchaseOrders", "MFilesStateId", c => c.Int(nullable: false));
            AddColumn("dbo.PurchaseOrders", "MFilesProcessEnded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseOrders", "MFilesProcessEnded");
            DropColumn("dbo.PurchaseOrders", "MFilesStateId");
            DropColumn("dbo.PurchaseOrders", "MFilesProcessId");
            DropColumn("dbo.PurchaseOrders", "MFilesId");
        }
    }
}
