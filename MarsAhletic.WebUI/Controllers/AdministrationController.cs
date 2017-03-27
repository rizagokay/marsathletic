using MarsAhletic.WebUI.Helpers;
using MarsAhletic.WebUI.Interfaces;
using MarsAhletic.WebUI.Models;
using Mechsoft.ADServices.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MarsAhletic.WebUI.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdministrationController : Controller
    {

        private ApplicationDbContext appDb;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private IApplicationOperations appOps;
        private IDirectoryOperations directory;

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

        public AdministrationController()
        {
            appDb = new ApplicationDbContext();
            appOps = new ApplicationOperations();
            directory = new DirectoryOperations();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddNewUser()
        {

            var model = new AddNewUserViewModel();

            model.Departments = new MultiSelectList(appDb.Departments.ToList(), "Id", "Name");


            var userList = new List<LoginAccount>();

            foreach (var item in appDb.Users.ToList())
            {
                var userCount = appDb.AppUsers.Where(u => u.LoginAccount.Id == item.Id && !u.IsDeleted).ToList().Count;
                if (userCount == 0)
                {
                    userList.Add(item);
                }
            }

            model.LoginAccounts = new MultiSelectList(userList, "Id", "UserName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewUser(AddNewUserViewModel model)
        {

            var userList = new List<LoginAccount>();

            foreach (var item in appDb.Users.ToList())
            {
                var userCount = appDb.AppUsers.Where(u => u.LoginAccount.Id == item.Id).ToList().Count;
                if (userCount == 0)
                {
                    userList.Add(item);
                }
            }

            model.Departments = new MultiSelectList(appDb.Departments.ToList(), "Id", "Name");
            model.LoginAccounts = new MultiSelectList(userList, "Id", "UserName", appDb.Users.Where(x => x.Id == model.LoginAccountId).ToList());

            if (ModelState.IsValid)
            {

                var loginAccount = appDb.Users.Where(u => u.Id == model.LoginAccountId).First();

                var appUser = new ApplicationUser() { IsDisabled = model.IsDeactive, IsManager = model.IsManager, LoginAccount = loginAccount, Username = loginAccount.UserName };

                var departmentList = new List<Department>();

                foreach (var item in model.SelectedDepartments)
                {
                    var foundDepartment = appDb.Departments.Find(item);
                    if (foundDepartment != null)
                    {
                        departmentList.Add(foundDepartment);
                    }
                }

                appUser.Departments = departmentList;

                appDb.AppUsers.Add(appUser);
                appDb.SaveChanges();

                return RedirectToAction("ListUsers");
            }

            return View(model);
        }

        public ActionResult ListUsers()
        {

            var users = appDb.AppUsers.Where(u => !u.IsDeleted).ToList();

            return View(users);
        }

        public ActionResult EditUser(int id)
        {

            var user = appDb.AppUsers.Where(x => x.Id == id).FirstOrDefault();

            if (user.IsDeleted)
            {
                return RedirectToAction("ListUsers");
            }

            if (user == null)
            {
                return HttpNotFound("Kullanıcı bulunamadı.");
            }


            var userList = new List<LoginAccount>();

            foreach (var item in appDb.Users.ToList())
            {
                if (user.LoginAccount != null)
                {
                    var userCount = appDb.AppUsers.Where(u => u.LoginAccount.Id == item.Id && !u.IsDeleted).ToList().Count;
                    if (userCount == 0 || item.Id == user.LoginAccount.Id)
                    {
                        userList.Add(item);
                    }
                }
                else
                {
                    var userCount = appDb.AppUsers.Where(u => u.LoginAccountId == item.Id && !u.IsDeleted).ToList().Count;
                    if (userCount == 0)
                    {
                        userList.Add(item);
                    }
                }

            }

            var model = new EditUserViewModel()
            {
                UserId = user.Id,
                Username = user.Username,
                OldLoginAccountId = user.LoginAccount != null ? user.LoginAccount.Id : "Bulunamadı",
                IsDeactive = user.IsDisabled,
                IsManager = user.IsManager,
                Departments = new SelectList(appDb.Departments.ToList(), "Id", "Name", user.Departments.Select(d => d.Id).ToList().ToArray()),
                SelectedDepartments = user.Departments.Select(d => d.Id).ToArray()

            };

            if (user.LoginAccount != null)
            {
                model.LoginAccounts = new MultiSelectList(userList, "Id", "UserName", appDb.Users.Where(x => x.Id == user.LoginAccount.Id).Select(u => u.Id).ToList().ToArray());
            }
            else
            {
                model.LoginAccounts = new MultiSelectList(userList, "Id", "UserName");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(EditUserViewModel model)
        {

            var user = appDb.AppUsers.Include("LoginAccount").Where(u => u.Id == model.UserId).FirstOrDefault();

            if (user == null)
            {
                return HttpNotFound("Kullanıcı bulunamadı");
            }


            if (model.OldLoginAccountId != model.LoginAccountId)
            {
                user.LoginAccount = appDb.Users.Find(model.LoginAccountId);
                user.Username = appDb.Users.Find(model.LoginAccountId).UserName;
            }

            var departmentList = new List<Department>();

            foreach (var item in model.SelectedDepartments)
            {
                var foundDepartment = appDb.Departments.Find(item);
                if (foundDepartment != null)
                {
                    departmentList.Add(foundDepartment);
                }
            }

            var departments = user.Departments;

            var addedDepartments = departmentList.Except(departments).ToList();
            var deletedDepartments = departments.Except(departmentList).ToList();

            foreach (var item in deletedDepartments)
            {
                user.Departments.Remove(item);
            }

            foreach (var item in addedDepartments)
            {
                user.Departments.Add(item);
            }


            user.IsDisabled = model.IsDeactive;
            user.IsManager = model.IsManager;

            appDb.Entry(user).State = System.Data.Entity.EntityState.Modified;
            try
            {
                appDb.SaveChanges();
            }
            catch (Exception Ex)
            {

                throw;
            }

            return RedirectToAction("ListUsers", "Administration");
        }

        public async Task<ActionResult> DeleteUserConfirmation(int id)
        {

            var user = await appDb.AppUsers.FindAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteUserConfirmation")]
        public ActionResult DeleteUser(int id)
        {

            var user = appDb.AppUsers.Include("LoginAccount").Where(u => u.Id == id).FirstOrDefault();

            if (user == null)
            {
                return HttpNotFound();
            }

            user.IsDeleted = true;

            appDb.Entry(user).State = System.Data.Entity.EntityState.Modified;

            try
            {
                appDb.SaveChanges();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return RedirectToAction("ListUsers", "Administration");
        }

        public ActionResult AddNewAccount()
        {

            var model = new LoginAccountViewModel();

            ViewBag.DomainIntegration = appOps.ActiveDirectoryIntegrationIsEnabled();


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewAccount(LoginAccountViewModel model)
        {

            if (model.AccountType == 2)
            {
                model.Password = "model@Secret1pass";
            }

            ViewBag.DomainIntegration = appOps.ActiveDirectoryIntegrationIsEnabled();

            try
            {
                var appUser = new LoginAccount();

                appUser.UserName = model.Username;
                appUser.Email = model.Email ?? "";
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
                    result = await UserManager.CreateAsync(appUser, model.Password);
                }
                else
                {
                    result = await UserManager.CreateAsync(appUser, model.Password);
                }

                if (result.Succeeded)
                {

                    //Try to create the user

                    if (model.CreateUser)
                    {
                        var user = new ApplicationUser()
                        {
                            IsDeleted = false,
                            IsDisabled = false,
                            IsManager = false,
                            Username = appUser.UserName,
                            LoginAccountId = appUser.Id

                        };


                        appDb.AppUsers.Add(user);
                        await appDb.SaveChangesAsync();

                    }


                    return RedirectToAction("ListAccounts");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.Clear();
                        ModelState.AddModelError("", item);
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(model);
            }


            return View(model);


        }

        public async Task<ActionResult> EditAccount(string id)
        {

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAccount(LoginAccount model)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        public ActionResult ListAccounts()
        {

            var users = UserManager.Users.ToList();

            return View(users);
        }

        public async Task<ActionResult> DeleteAccountConfirmation(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteAccountConfirmation")]
        public async Task<ActionResult> DeleteAccount(LoginAccount model)
        {

            var user = await UserManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var users = appDb.AppUsers.Where(u => u.LoginAccountId == user.Id).ToList();

            foreach (var item in users)
            {
                item.LoginAccountId = null;
                appDb.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            await appDb.SaveChangesAsync();

            await UserManager.DeleteAsync(user);

            return RedirectToAction("ListAccounts", "Administration");
        }


        public ActionResult DomainIntegration()

        {
            var model = new DomainIntegrationViewModel();

            var domainAccountKey = "DomainAccountName";
            var domainKey = "DomainName";
            var domainPasswordKey = "DomainPassword";

            model.DomainAdministratorAccount = appOps.GetValue(domainAccountKey);
            model.DomainAdministratorPassword = appOps.GetValue(domainPasswordKey);
            model.DomainName = appOps.GetValue(domainKey);
            model.IsActive = appOps.ActiveDirectoryIntegrationIsEnabled();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DomainIntegration(DomainIntegrationViewModel model)
        {


            if (!model.IsActive)
            {
                appOps.DeactivateActiveDirectoryIntegration();
                model.IsActive = false;
                model.DomainName = "";
                model.DomainAdministratorAccount = "";
                model.DomainAdministratorPassword = "";
                return View(model);
            }

            var domainAccountKey = "DomainAccountName";
            var domainKey = "DomainName";
            var domainPasswordKey = "DomainPassword";

            var AccountNameInDb = appOps.GetValue(domainAccountKey);
            var DomainNameInDb = appOps.GetValue(domainKey);
            var DomainPasswordInDb = appOps.GetValue(domainPasswordKey);

            if (appOps.ActiveDirectoryIntegrationIsEnabled())
            {

                if (model.DomainName.Trim() == "" || model.DomainAdministratorAccount == "")
                {
                    ModelState.AddModelError("", "Domain Adı ve Kullanıcı Adı alanları zorunludur.");
                    return View(model);
                }


                if (model.DomainName != DomainNameInDb)
                {
                    appOps.UpdateValue(domainKey, model.DomainName);
                }

                if (model.DomainAdministratorAccount != AccountNameInDb)
                {
                    appOps.UpdateValue(domainAccountKey, model.DomainAdministratorAccount);
                }

                if (model.DomainAdministratorPassword != null)
                {
                    if (model.DomainAdministratorPassword.Trim() != "")
                    {
                        appOps.UpdateValue(domainPasswordKey, model.DomainAdministratorPassword);
                    }
                }
            }
            else
            {

                if (model.DomainName.Trim() == null || model.DomainAdministratorAccount == null)
                {
                    ModelState.AddModelError("", "Domain Adı ve Kullanıcı Adı alanları zorunludur.");
                    return View(model);
                }

                if (model.DomainName.Trim() == "" || model.DomainAdministratorAccount == "")
                {
                    ModelState.AddModelError("", "Domain Adı ve Kullanıcı Adı alanları zorunludur.");
                    return View(model);
                }

                if (model.DomainAdministratorPassword == null)
                {
                    ModelState.AddModelError("", "Şifre alanı gereklidir.");
                    return View(model);
                }

                if (model.DomainAdministratorPassword.Trim() == "")
                {
                    ModelState.AddModelError("", "Şifre alanı gereklidir.");
                    return View(model);
                }

                appOps.CreateNewConfigKey(domainAccountKey, model.DomainAdministratorAccount);
                appOps.CreateNewConfigKey(domainKey, model.DomainName);
                appOps.CreateNewConfigKey(domainPasswordKey, model.DomainAdministratorPassword);


            }

            return View(model);
        }

        [HttpPost]
        public JsonResult GetUserDetails(string username)
        {

            if (!appOps.ActiveDirectoryIntegrationIsEnabled())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                List<string> errors = new List<string>();
                errors.Add("Domain entegrasyonu aktif edilmemiş.");
                return Json(errors);
            }

            var domainAccountKey = "DomainAccountName";
            var domainKey = "DomainName";
            var domainPasswordKey = "DomainPassword";

            var AccountNameInDb = appOps.GetValue(domainAccountKey);
            var DomainNameInDb = appOps.GetValue(domainKey);
            var DomainPasswordInDb = appOps.GetValue(domainPasswordKey);

            var usernameAndEmail = directory.GetNameAndEmail(AccountNameInDb, DomainPasswordInDb, DomainNameInDb, username);

            if (usernameAndEmail.Count == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                List<string> errors = new List<string>();
                errors.Add("Kullanıcı bulunamadı.");
                return Json(errors);
            }

            var name = usernameAndEmail.Keys.ToList()[0];
            var email = usernameAndEmail.Values.ToList()[0];


            return Json(new { Name = name, Email = email });
        }

        [HttpPost]
        public JsonResult TestActiveDirectory(string username, string password, string domain)
        {
            bool authsuccess = false;

            if (password != null)
            {
                authsuccess = directory.IsAuthenticated(username, password, domain);
            }
            else
            {
                var usernameInDb = appOps.GetValue("DomainAccountName");

                if (usernameInDb != null)
                {
                    if (usernameInDb != "")
                    {
                        if (usernameInDb == username)
                        {
                            var passwordInDb = appOps.GetValue("DomainPassword");
                            authsuccess = directory.IsAuthenticated(username, passwordInDb, domain);
                        }
                    }
                }
            }

            if (authsuccess)
            {
                return Json(new { Message = "Test bağlantısı başarılı." });
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                return Json(new { Message = "Test bağlantısı başarısız." });
            }     
        }

        #region AccessControlListOperations

        public ActionResult ModulePermissions()
        {
            var modules = appDb.Modules.ToList();

            return View(modules);
        }

        public ActionResult CreateNewAccessControlList()
        {
            var model = new AccessControlListViewModel();
            model.Users = new MultiSelectList(appDb.AppUsers.ToList(), "Id", "Username");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewAccessControlList(AccessControlListViewModel model)
        {
            return View();
        }

        public ActionResult ViewAccessControlList(int id)
        {
            return View();
        }

        public ActionResult EditAccessControlList(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccessControlList(AccessControlListViewModel model)
        {
            return View();
        }

        public ActionResult DeleteAccessControlListConfirm(int id)
        {
            return View();
        }

        [HttpPost]
        [ActionName("DeleteAccessControlListConfirm")]
        public ActionResult DeleteAccessControlList(int id)
        {
            return View();
        }

        public ActionResult AccessControlDeleted(int id)
        {
            return View();
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            appDb.Dispose();
            appOps.Dispose();
            base.Dispose(disposing);
        }


    }
}