namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedusername : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "Username", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "Username");
        }
    }
}
