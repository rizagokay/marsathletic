using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;

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
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TravelPlan> TravelPlans { get; set; }
        public DbSet<TravelPlanDetail> TravelPlanDetails { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<ExpanseItem> ExpanseItems { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}