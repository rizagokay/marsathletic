namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppUserAddedForLoginAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AppUser_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "AppUser_Id");
            AddForeignKey("dbo.AspNetUsers", "AppUser_Id", "dbo.ApplicationUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "AppUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.AspNetUsers", new[] { "AppUser_Id" });
            DropColumn("dbo.AspNetUsers", "AppUser_Id");
        }
    }
}
