using MarsAhletic.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MarsAhletic.WebUI.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdministrationController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }

        }

        // GET: Administration
        public ActionResult Index()
        {

            return View();
        }

        [AllowAnonymous]
        public ActionResult AddNewUser()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewUser(ApplicationUserVM model)
        {

            if (ModelState.IsValid)
            {
                var appUser = new ApplicationUser();

                appUser.UserName = model.Username;
                appUser.Email = model.Email ?? "";
                appUser.IsManager = model.IsManager;
                appUser.Name = model.Name;


                IdentityRole adminRole;
                if (model.IsAdministrator)
                {
                    //Setup Administrators            
                    var roleIsAvailable = await RoleManager.RoleExistsAsync("Administrators");
                    IdentityResult roleCreateResult;
                    if (!roleIsAvailable)
                    {
                        roleCreateResult = await RoleManager.CreateAsync(new IdentityRole() { Name = "Administrators" });
                        if (!roleCreateResult.Succeeded)
                        {
                            foreach (var item in roleCreateResult.Errors)
                            {
                                ModelState.AddModelError("", item);
                            }
                            return View(model);
                        }

                        adminRole = await RoleManager.FindByNameAsync("Administrators");
                    }
                    else
                    {
                        adminRole = await RoleManager.FindByNameAsync("Administrators");
                    }

                    var adminUserRole = new IdentityUserRole() { RoleId = adminRole.Id, UserId = appUser.Id };
                    appUser.Roles.Add(adminUserRole);

                }

                IdentityResult result;
                if (model.Domain != null)
                {
                    appUser.ADDomain = model.Domain;
                    appUser.UserName = model.Domain + "\\" + model.Username;
                    result = await UserManager.CreateAsync(appUser, "model@secret.pass");

                }
                else
                {
                    result = await UserManager.CreateAsync(appUser, model.Password);
                }

                if (result.Succeeded)
                {
                    return RedirectToAction("AddNewUserComplete");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item);
                    }
                    return View(model);
                }

            }
            else
            {
                return View(model);
            }

        }

    }
}