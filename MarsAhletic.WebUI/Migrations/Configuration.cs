namespace MarsAhletic.WebUI.Migrations
{
    using Models;
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

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var LoginAccount = new LoginAccount();
                LoginAccount.UserName = "admin";
                LoginAccount.Name = "Sistem Yöneticisi";
                LoginAccount.Email = "";
                var password = "model@Secret1pass";


                var store = new Microsoft.AspNet.Identity.EntityFramework.RoleStore<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(context);
                var rolemanager = new ApplicationRoleManager(store);

                var adminRole = rolemanager.FindByNameAsync("Administrators").Result;
                var userRole = new Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole();
                userRole.RoleId = adminRole.Id;
                userRole.UserId = LoginAccount.Id;

                var userstore = new Microsoft.AspNet.Identity.EntityFramework.UserStore<LoginAccount>(context);

                LoginAccount.Roles.Add(userRole);


                var usermanager = new ApplicationUserManager(userstore);

                var usercreated = usermanager.CreateAsync(LoginAccount, password).Result;

            }

            if (!context.Modules.Any(r => r.Name == "PurchaseOrders"))
            {
                var module = new Module() { Name = "PurchaseOrders" };
                context.Modules.Add(module);
                context.SaveChanges();
            }

        }
    }
}
