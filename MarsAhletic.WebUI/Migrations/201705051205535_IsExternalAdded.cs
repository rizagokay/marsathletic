namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsExternalAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsExternal", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsExternal");
        }
    }
}
