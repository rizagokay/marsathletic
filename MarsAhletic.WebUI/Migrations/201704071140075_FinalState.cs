namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinalState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "IsDeptManager", c => c.Boolean(nullable: false));
            AddColumn("dbo.ApplicationUsers", "IsHighManager", c => c.Boolean(nullable: false));
            AddColumn("dbo.PurchaseOrders", "MFilesProcessName", c => c.String());
            AddColumn("dbo.PurchaseOrders", "MFilesStateName", c => c.String());
            AddColumn("dbo.PurchaseOrders", "MFilesProcessEndedWithApproval", c => c.Boolean(nullable: false));
            DropColumn("dbo.ApplicationUsers", "IsManager");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ApplicationUsers", "IsManager", c => c.Boolean(nullable: false));
            DropColumn("dbo.PurchaseOrders", "MFilesProcessEndedWithApproval");
            DropColumn("dbo.PurchaseOrders", "MFilesStateName");
            DropColumn("dbo.PurchaseOrders", "MFilesProcessName");
            DropColumn("dbo.ApplicationUsers", "IsHighManager");
            DropColumn("dbo.ApplicationUsers", "IsDeptManager");
        }
    }
}
