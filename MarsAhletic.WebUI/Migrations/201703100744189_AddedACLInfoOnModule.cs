namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedACLInfoOnModule : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AccessControlLists", "IsInUse");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccessControlLists", "IsInUse", c => c.Boolean(nullable: false));
        }
    }
}
