namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletedUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "IsDeleted");
        }
    }
}
