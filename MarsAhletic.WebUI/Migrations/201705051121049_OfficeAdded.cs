namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfficeAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Offices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExternalId = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ApplicationUsers", "OfficeId", c => c.Int());
            CreateIndex("dbo.ApplicationUsers", "OfficeId");
            AddForeignKey("dbo.ApplicationUsers", "OfficeId", "dbo.Offices", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUsers", "OfficeId", "dbo.Offices");
            DropIndex("dbo.ApplicationUsers", new[] { "OfficeId" });
            DropColumn("dbo.ApplicationUsers", "OfficeId");
            DropTable("dbo.Offices");
        }
    }
}
