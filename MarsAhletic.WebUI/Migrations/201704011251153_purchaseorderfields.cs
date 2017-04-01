namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purchaseorderfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrders", "MFilesProcessName", c => c.String());
            AddColumn("dbo.PurchaseOrders", "MFilesStateName", c => c.String());
            AddColumn("dbo.PurchaseOrders", "MFilesProcessEndedWithApproval", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseOrders", "MFilesProcessEndedWithApproval");
            DropColumn("dbo.PurchaseOrders", "MFilesStateName");
            DropColumn("dbo.PurchaseOrders", "MFilesProcessName");
        }
    }
}
