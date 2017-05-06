namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefinedList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DefinedLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RelatedDocumentId = c.Int(nullable:true),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.RelatedDocumentId)
                .Index(t => t.RelatedDocumentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DefinedLists", "RelatedDocumentId", "dbo.Documents");
            DropIndex("dbo.DefinedLists", new[] { "RelatedDocumentId" });
            DropTable("dbo.DefinedLists");
        }
    }
}
