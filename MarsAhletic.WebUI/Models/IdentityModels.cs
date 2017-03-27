using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarsAhletic.WebUI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class LoginAccount : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<LoginAccount> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            userIdentity.AddClaim(new Claim("Name", this.Name));

            return userIdentity;
        }

        public string Name { get; set; }
        public string ADUserId { get; set; }
        public string ADDomain { get; set; }

        public virtual ICollection<ApplicationUser> AppUsers { get; set; }

    }

    public static class IdentityExtensions
    {
        public static string GetName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Name");

            return (claim != null) ? claim.Value : string.Empty;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<LoginAccount>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(p => p.Comments)
                .WithMany()
                .Map(pc =>
                {
                    pc.MapLeftKey("PurchaseOrderId");
                    pc.MapRightKey("CommentId");
                    pc.ToTable("PurchaseOrderComments");
                });

            modelBuilder.Entity<LoginAccount>()
                .HasMany(l => l.AppUsers)
                .WithOptional(u => u.LoginAccount)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(p => p.Documents)
                .WithMany(d => d.PurchaseOrders)
                .Map(pd =>
                {
                    pd.MapLeftKey("PurchaseOrderId");
                    pd.MapRightKey("DocumentId");
                    pd.ToTable("PurchaseOrderDocuments");
                });

            modelBuilder.Entity<Department>()
                .HasMany(d => d.Users)
                .WithMany(u => u.Departments)
                .Map(du =>
                {
                    du.MapLeftKey("DeptId");
                    du.MapRightKey("UserId");
                    du.ToTable("DepartmentsUsers");
                });

            //Cascade Rule for Purchase order and Purchase Details
            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(p => p.PurchaseDetails)
                .WithRequired(po => po.PurchaseOrder)
                .WillCascadeOnDelete(true);

            //Cascade Rule For Company and Purchase Orders
            modelBuilder.Entity<Company>()
                .HasMany(c => c.PurchaseOrders)
                .WithRequired(p => p.Company)
                .WillCascadeOnDelete(false);

            //Cascade Rule for Cost Center and Purchase Orders
            modelBuilder.Entity<CostCenter>()
                .HasMany(c => c.Orders)
                .WithOptional(p => p.CostCenter)
                .WillCascadeOnDelete(false);

            //Cascade Rule for User and Comments
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(au => au.Comments)
                .WithRequired(c => c.User)
                .WillCascadeOnDelete(false);

            //Cascade Rule for User and Purchase Orders
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(au => au.PurchaseOrders)
                .WithRequired(c => c.CreatedBy)
                .WillCascadeOnDelete(false);

            //Cascade Rule for Purchase Detail and Budget Type
            modelBuilder.Entity<PurchaseDetail>()
                .HasOptional(pd => pd.BudgetType)
                .WithMany()
                .WillCascadeOnDelete(false);

            //Cascade Rule for AccessControlList
            modelBuilder.Entity<AccessControlList>()
                .HasMany(ac => ac.ACLEntries)
                .WithRequired(ae => ae.ACL)
                .WillCascadeOnDelete(true);

            //Cascade Rule for Product and PurchaseDetails
            modelBuilder.Entity<Product>()
                .HasMany(p => p.PurchaseDetails)
                .WithRequired(pd => pd.Product)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<ACLEntry>()
                .HasRequired(a => a.User)
                .WithMany()
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TravelPlan> TravelPlans { get; set; }
        public DbSet<TravelPlanDetail> TravelPlanDetails { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<ExpanseItem> ExpanseItems { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<AccessControlList> AccessControlLists { get; set; }
        public DbSet<ACLEntry> ACLEntries { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ApplicationUser> AppUsers { get; set; }
        public DbSet<Configuration> Configs { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}