namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoginAccountIdAdded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "AppUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUsers", "LoginAccount_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUsers", new[] { "AppUser_Id" });
            AddForeignKey("dbo.ApplicationUsers", "LoginAccount_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            DropColumn("dbo.AspNetUsers", "AppUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "AppUser_Id", c => c.Int());
            DropForeignKey("dbo.ApplicationUsers", "LoginAccount_Id", "dbo.AspNetUsers");
            CreateIndex("dbo.AspNetUsers", "AppUser_Id");
            AddForeignKey("dbo.ApplicationUsers", "LoginAccount_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUsers", "AppUser_Id", "dbo.ApplicationUsers", "Id");
        }
    }
}
