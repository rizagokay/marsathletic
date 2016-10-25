using MarsAhletic.WebUI.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarsAhletic.WebUI.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdministrationController : Controller
    {

        private ApplicationUserManager _userManager;

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

        // GET: Administration
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult AddNewUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNewUser(AddUserViewModel model)
        {
            var appUser = new ApplicationUser();

            if (!model.SendConfirmationMail)
                appUser.EmailConfirmed = true;

            if (model.ADUserId != null)
            {

            }

            return View();
        }
    }
}