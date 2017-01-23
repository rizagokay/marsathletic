namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        CommentDate = c.DateTime(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IsManager = c.Boolean(nullable: false),
                        Name = c.String(),
                        ADUserId = c.String(),
                        ADDomain = c.String(),
                        DepartmentId = c.Int(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .Index(t => t.DepartmentId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExternalId = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.PurchaseOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderDate = c.DateTime(nullable: false),
                        TotalValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalValueWithVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalValueWithoutVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExtenalId = c.String(),
                        USDValueOnDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EURValueOnDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GBPValueOnDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Company_Id = c.Int(nullable: false),
                        CostCenter_Id = c.Int(nullable: false),
                        CreatedBy_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Company_Id)
                .ForeignKey("dbo.CostCenters", t => t.CostCenter_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
                .Index(t => t.Company_Id)
                .Index(t => t.CostCenter_Id)
                .Index(t => t.CreatedBy_Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExternalId = c.String(),
                        Name = c.String(),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CostCenters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExternalId = c.String(),
                        Name = c.String(),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TravelPlans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TravelPlanNo = c.String(),
                        Date = c.DateTime(nullable: false),
                        TravelRoute = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        AdvancePaymentValue = c.Single(nullable: false),
                        CostCenter_Id = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CostCenters", t => t.CostCenter_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.CostCenter_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.TravelPlanDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Int(nullable: false),
                        Description = c.String(),
                        Budget = c.Single(nullable: false),
                        TotalBudget = c.Single(nullable: false),
                        PlannedPrice = c.Single(nullable: false),
                        PlannedTotalBudget = c.Single(nullable: false),
                        BudgetDifference = c.Single(nullable: false),
                        ExpanseItem_Id = c.Int(),
                        TravelPlan_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExpanseItems", t => t.ExpanseItem_Id)
                .ForeignKey("dbo.TravelPlans", t => t.TravelPlan_Id)
                .Index(t => t.ExpanseItem_Id)
                .Index(t => t.TravelPlan_Id);
            
            CreateTable(
                "dbo.ExpanseItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExternalId = c.String(),
                        Name = c.String(),
                        Budget = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PurchaseDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ValueLocal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IncludedInBudget = c.Boolean(nullable: false),
                        BudgetCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BudgetType_Id = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                        PurchaseOrder_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BudgetTypes", t => t.BudgetType_Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .ForeignKey("dbo.PurchaseOrders", t => t.PurchaseOrder_Id)
                .Index(t => t.BudgetType_Id)
                .Index(t => t.Product_Id)
                .Index(t => t.PurchaseOrder_Id);
            
            CreateTable(
                "dbo.BudgetTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExternalId = c.String(),
                        Code = c.String(),
                        GroupCode = c.String(),
                        Name = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VATPercentage = c.Long(nullable: false),
                        Company_Id = c.Int(),
                        Currency_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Company_Id)
                .ForeignKey("dbo.Currencies", t => t.Currency_Id)
                .Index(t => t.Company_Id)
                .Index(t => t.Currency_Id);
            
            CreateTable(
                "dbo.Currencies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExternalId = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NotificationMessage = c.String(),
                        IsRead = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        ReadOn = c.DateTime(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.PurchaseOrderComments",
                c => new
                    {
                        PurchaseOrderId = c.Int(nullable: false),
                        CommentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PurchaseOrderId, t.CommentId })
                .ForeignKey("dbo.PurchaseOrders", t => t.PurchaseOrderId, cascadeDelete: true)
                .ForeignKey("dbo.Comments", t => t.CommentId, cascadeDelete: true)
                .Index(t => t.PurchaseOrderId)
                .Index(t => t.CommentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Notifications", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PurchaseOrders", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PurchaseDetails", "PurchaseOrder_Id", "dbo.PurchaseOrders");
            DropForeignKey("dbo.PurchaseDetails", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Products", "Currency_Id", "dbo.Currencies");
            DropForeignKey("dbo.Products", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.PurchaseDetails", "BudgetType_Id", "dbo.BudgetTypes");
            DropForeignKey("dbo.TravelPlans", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TravelPlanDetails", "TravelPlan_Id", "dbo.TravelPlans");
            DropForeignKey("dbo.TravelPlanDetails", "ExpanseItem_Id", "dbo.ExpanseItems");
            DropForeignKey("dbo.TravelPlans", "CostCenter_Id", "dbo.CostCenters");
            DropForeignKey("dbo.PurchaseOrders", "CostCenter_Id", "dbo.CostCenters");
            DropForeignKey("dbo.PurchaseOrders", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.PurchaseOrderComments", "CommentId", "dbo.Comments");
            DropForeignKey("dbo.PurchaseOrderComments", "PurchaseOrderId", "dbo.PurchaseOrders");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Comments", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.PurchaseOrderComments", new[] { "CommentId" });
            DropIndex("dbo.PurchaseOrderComments", new[] { "PurchaseOrderId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Notifications", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Products", new[] { "Currency_Id" });
            DropIndex("dbo.Products", new[] { "Company_Id" });
            DropIndex("dbo.PurchaseDetails", new[] { "PurchaseOrder_Id" });
            DropIndex("dbo.PurchaseDetails", new[] { "Product_Id" });
            DropIndex("dbo.PurchaseDetails", new[] { "BudgetType_Id" });
            DropIndex("dbo.TravelPlanDetails", new[] { "TravelPlan_Id" });
            DropIndex("dbo.TravelPlanDetails", new[] { "ExpanseItem_Id" });
            DropIndex("dbo.TravelPlans", new[] { "User_Id" });
            DropIndex("dbo.TravelPlans", new[] { "CostCenter_Id" });
            DropIndex("dbo.PurchaseOrders", new[] { "CreatedBy_Id" });
            DropIndex("dbo.PurchaseOrders", new[] { "CostCenter_Id" });
            DropIndex("dbo.PurchaseOrders", new[] { "Company_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "DepartmentId" });
            DropIndex("dbo.Comments", new[] { "User_Id" });
            DropTable("dbo.PurchaseOrderComments");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Notifications");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Currencies");
            DropTable("dbo.Products");
            DropTable("dbo.BudgetTypes");
            DropTable("dbo.PurchaseDetails");
            DropTable("dbo.ExpanseItems");
            DropTable("dbo.TravelPlanDetails");
            DropTable("dbo.TravelPlans");
            DropTable("dbo.CostCenters");
            DropTable("dbo.Companies");
            DropTable("dbo.PurchaseOrders");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Departments");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Comments");
        }
    }
}
