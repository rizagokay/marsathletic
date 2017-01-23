namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class documentstorednameadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "StoredName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "StoredName");
        }
    }
}
