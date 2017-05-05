namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseDetails", "Discount", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "AccountCode", c => c.String());
            AddColumn("dbo.Products", "AccountName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "AccountName");
            DropColumn("dbo.Products", "AccountCode");
            DropColumn("dbo.PurchaseDetails", "Discount");
        }
    }
}
