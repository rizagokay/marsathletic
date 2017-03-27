namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullableloginAccountId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationUsers", "LoginAccount_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUsers", new[] { "LoginAccount_Id" });
            RenameColumn(table: "dbo.ApplicationUsers", name: "LoginAccount_Id", newName: "LoginAccountId");
            AlterColumn("dbo.ApplicationUsers", "LoginAccountId", c => c.String(maxLength: 128));
            CreateIndex("dbo.ApplicationUsers", "LoginAccountId");
            AddForeignKey("dbo.ApplicationUsers", "LoginAccountId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUsers", "LoginAccountId", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUsers", new[] { "LoginAccountId" });
            AlterColumn("dbo.ApplicationUsers", "LoginAccountId", c => c.String(nullable: false, maxLength: 128));
            RenameColumn(table: "dbo.ApplicationUsers", name: "LoginAccountId", newName: "LoginAccount_Id");
            CreateIndex("dbo.ApplicationUsers", "LoginAccount_Id");
            AddForeignKey("dbo.ApplicationUsers", "LoginAccount_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
