namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsManagerUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "IsManager", c => c.Boolean(nullable: false));
            DropColumn("dbo.AspNetUsers", "IsManager");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "IsManager", c => c.Boolean(nullable: false));
            DropColumn("dbo.ApplicationUsers", "IsManager");
        }
    }
}
