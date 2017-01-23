namespace MarsAhletic.WebUI.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MarsAhletic.WebUI.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

        }

        protected override void Seed(MarsAhletic.WebUI.Models.ApplicationDbContext context)
        {

            if (!context.Roles.Any(r => r.Name == "Administrators"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole() { Name = "Administrators" };
                var store = new Microsoft.AspNet.Identity.EntityFramework.RoleStore<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(context);
                var manager = new ApplicationRoleManager(store);

                var created = manager.CreateAsync(role).Result;
            }

        }
    }
}
