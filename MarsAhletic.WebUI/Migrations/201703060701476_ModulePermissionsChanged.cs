namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModulePermissionsChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ACLEntries", "CanAccessModule", c => c.Boolean(nullable: false));
            DropColumn("dbo.ACLEntries", "CanCreate");
            DropColumn("dbo.ACLEntries", "CanRead");
            DropColumn("dbo.ACLEntries", "CanUpdate");
            DropColumn("dbo.ACLEntries", "CanDelete");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ACLEntries", "CanDelete", c => c.Boolean(nullable: false));
            AddColumn("dbo.ACLEntries", "CanUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.ACLEntries", "CanRead", c => c.Boolean(nullable: false));
            AddColumn("dbo.ACLEntries", "CanCreate", c => c.Boolean(nullable: false));
            DropColumn("dbo.ACLEntries", "CanAccessModule");
        }
    }
}
