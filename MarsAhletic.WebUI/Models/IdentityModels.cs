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
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            userIdentity.AddClaim(new Claim("IsManager", this.IsManager.ToString()));
            userIdentity.AddClaim(new Claim("Name", this.Name));

            return userIdentity;
        }

        public bool IsManager { get; set; }
        public string Name { get; set; }
        public string ADUserId { get; set; }
        public string ADDomain { get; set; }

        public int? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<TravelPlan> TravelPlans { get; set; }

    }

    public static class IdentityExtensions
    {
        public static string IsManager(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("IsManager");

            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Name");

            return (claim != null) ? claim.Value : string.Empty;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


            modelBuilder.Entity<PurchaseOrder>()
                .HasMany<Comment>(p => p.Comments)
                .WithMany()
                .Map(pc =>
                {
                    pc.MapLeftKey("PurchaseOrderId");
                    pc.MapRightKey("CommentId");
                    pc.ToTable("PurchaseOrderComments");
                });
           

            modelBuilder.Entity<PurchaseOrder>()
                .HasMany<Document>(p => p.Documents)
                .WithMany(d => d.PurchaseOrders)
                .Map(pd =>
                {
                    pd.MapLeftKey("PurchaseOrderId");
                    pd.MapRightKey("DocumentId");
                    pd.ToTable("PurchaseOrderDocuments");
                });

            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(p => p.PurchaseDetails)
                .WithRequired(po => po.PurchaseOrder)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Company>()
                .HasMany(c => c.PurchaseOrders)
                .WithRequired(p => p.Company)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<CostCenter>()
                .HasMany(c => c.Orders)
                .WithOptional(p => p.CostCenter)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany<Comment>(au => au.Comments)
                .WithRequired(c => c.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany<PurchaseOrder>(au => au.PurchaseOrders)
                .WithRequired(c => c.CreatedBy)
                .WillCascadeOnDelete(false);



            modelBuilder.Entity<PurchaseDetail>()
                .HasOptional(pd => pd.BudgetType)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany<PurchaseDetail>(p => p.PurchaseDetails)
                .WithRequired(pd => pd.Product)
                .WillCascadeOnDelete(false);

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

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}