namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DepartmentFixed : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DepartmentsUsers", "DeptId", "dbo.Departments");
            DropForeignKey("dbo.DepartmentsUsers", "UserId", "dbo.ApplicationUsers");
            DropIndex("dbo.DepartmentsUsers", new[] { "DeptId" });
            DropIndex("dbo.DepartmentsUsers", new[] { "UserId" });
            AddColumn("dbo.ApplicationUsers", "DepartmentId", c => c.Int(nullable:true));
            CreateIndex("dbo.ApplicationUsers", "DepartmentId");
            AddForeignKey("dbo.ApplicationUsers", "DepartmentId", "dbo.Departments", "Id");
            DropTable("dbo.DepartmentsUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DepartmentsUsers",
                c => new
                    {
                        DeptId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DeptId, t.UserId });
            
            DropForeignKey("dbo.ApplicationUsers", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.ApplicationUsers", new[] { "DepartmentId" });
            DropColumn("dbo.ApplicationUsers", "DepartmentId");
            CreateIndex("dbo.DepartmentsUsers", "UserId");
            CreateIndex("dbo.DepartmentsUsers", "DeptId");
            AddForeignKey("dbo.DepartmentsUsers", "UserId", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DepartmentsUsers", "DeptId", "dbo.Departments", "Id", cascadeDelete: true);
        }
    }
}
