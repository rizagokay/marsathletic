namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AclEntryUpdated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ACLEntries", "User_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.ACLEntries", new[] { "User_Id" });
            AlterColumn("dbo.ACLEntries", "User_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.ACLEntries", "User_Id");
            AddForeignKey("dbo.ACLEntries", "User_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ACLEntries", "User_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.ACLEntries", new[] { "User_Id" });
            AlterColumn("dbo.ACLEntries", "User_Id", c => c.Int());
            CreateIndex("dbo.ACLEntries", "User_Id");
            AddForeignKey("dbo.ACLEntries", "User_Id", "dbo.ApplicationUsers", "Id");
        }
    }
}
