namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Modules", "ACL_Id", "dbo.AccessControlLists");
            DropForeignKey("dbo.AccessControlLists", "Id", "dbo.Modules");
            DropForeignKey("dbo.ACLEntries", "ACL_Id", "dbo.AccessControlLists");
            DropIndex("dbo.AccessControlLists", new[] { "Id" });
            DropIndex("dbo.Modules", new[] { "ACL_Id" });
            DropPrimaryKey("dbo.AccessControlLists");
            AddColumn("dbo.AccessControlLists", "ModuleId", c => c.Int());
            AlterColumn("dbo.AccessControlLists", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.AccessControlLists", "Id");
            CreateIndex("dbo.AccessControlLists", "ModuleId");
            AddForeignKey("dbo.AccessControlLists", "ModuleId", "dbo.Modules", "Id");
            AddForeignKey("dbo.ACLEntries", "ACL_Id", "dbo.AccessControlLists", "Id", cascadeDelete: true);
            DropColumn("dbo.Modules", "ACL_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Modules", "ACL_Id", c => c.Int());
            DropForeignKey("dbo.ACLEntries", "ACL_Id", "dbo.AccessControlLists");
            DropForeignKey("dbo.AccessControlLists", "ModuleId", "dbo.Modules");
            DropIndex("dbo.AccessControlLists", new[] { "ModuleId" });
            DropPrimaryKey("dbo.AccessControlLists");
            AlterColumn("dbo.AccessControlLists", "Id", c => c.Int(nullable: false));
            DropColumn("dbo.AccessControlLists", "ModuleId");
            AddPrimaryKey("dbo.AccessControlLists", "Id");
            CreateIndex("dbo.Modules", "ACL_Id");
            CreateIndex("dbo.AccessControlLists", "Id");
            AddForeignKey("dbo.ACLEntries", "ACL_Id", "dbo.AccessControlLists", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AccessControlLists", "Id", "dbo.Modules", "Id");
            AddForeignKey("dbo.Modules", "ACL_Id", "dbo.AccessControlLists", "Id");
        }
    }
}
