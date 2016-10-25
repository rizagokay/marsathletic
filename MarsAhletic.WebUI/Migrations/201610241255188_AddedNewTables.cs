namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNewTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CostCenters",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ExternalId = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TravelPlans",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        TravelRoute = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        AdvancePaymentValue = c.Single(nullable: false),
                        CostCenterId = c.String(maxLength: 128),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CostCenters", t => t.CostCenterId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.CostCenterId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.TravelPlanDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Amount = c.Int(nullable: false),
                        Description = c.String(),
                        ExpanseItemId = c.String(maxLength: 128),
                        Budget = c.Single(nullable: false),
                        TotalBudget = c.Single(nullable: false),
                        PlannedPrice = c.Single(nullable: false),
                        PlannedTotalBudget = c.Single(nullable: false),
                        BudgetDifference = c.Single(nullable: false),
                        TravelPlan_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExpanseItems", t => t.ExpanseItemId)
                .ForeignKey("dbo.TravelPlans", t => t.TravelPlan_Id)
                .Index(t => t.ExpanseItemId)
                .Index(t => t.TravelPlan_Id);
            
            CreateTable(
                "dbo.ExpanseItems",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ExternalId = c.String(),
                        Name = c.String(),
                        Budget = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "IsManager", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TravelPlans", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TravelPlanDetails", "TravelPlan_Id", "dbo.TravelPlans");
            DropForeignKey("dbo.TravelPlanDetails", "ExpanseItemId", "dbo.ExpanseItems");
            DropForeignKey("dbo.TravelPlans", "CostCenterId", "dbo.CostCenters");
            DropIndex("dbo.TravelPlanDetails", new[] { "TravelPlan_Id" });
            DropIndex("dbo.TravelPlanDetails", new[] { "ExpanseItemId" });
            DropIndex("dbo.TravelPlans", new[] { "User_Id" });
            DropIndex("dbo.TravelPlans", new[] { "CostCenterId" });
            DropColumn("dbo.AspNetUsers", "IsManager");
            DropTable("dbo.ExpanseItems");
            DropTable("dbo.TravelPlanDetails");
            DropTable("dbo.TravelPlans");
            DropTable("dbo.CostCenters");
        }
    }
}
