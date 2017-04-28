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

                var appUser = new ApplicationUser() { IsDisabled = model.IsDeactive, IsDeptManager = model.IsDeptManager, IsHighManager = model.IsHighManager, LoginAccount = loginAccount, Username = loginAccount.UserName };

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
                throw new HttpException(404, "Kullanıcı Bulunamadı.");
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
                IsDeptManager = user.IsDeptManager,
                IsHighManager = user.IsHighManager,
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
                throw new HttpException(404, "Kullanıcı Bulunamadı.");
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
            user.IsDeptManager = model.IsDeptManager;
            user.IsHighManager = model.IsHighManager;

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
                throw new HttpException(404, "Kullanıcı Bulunamadı.");
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
                throw new HttpException(404, "Kullanıcı Bulunamadı.");
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
            string DomainNameInDb = "";

            if (model.AccountType == 2)
            {
                model.Password = "model@Secret1pass";

                var domainKey = "DomainName";
                DomainNameInDb = appOps.GetValue(domainKey);
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
                if (DomainNameInDb != "")
                {
                    appUser.ADDomain = DomainNameInDb;
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
                            IsDeptManager = false,
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

        }

        public async Task<ActionResult> EditAccount(string id)
        {

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new HttpException(404, "Kullanıcı Bulunamadı.");
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
                throw new HttpException(404, "Kullanıcı Bulunamadı.");
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
                throw new HttpException(404, "Kullanıcı Bulunamadı.");
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
                throw new HttpException(404, "Kullanıcı Bulunamadı.");
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

        public ActionResult EditModulePermissions(int? id, string targetId)
        {
            if (id == null)
            {
                throw new HttpException(400, "Geçersiz istek.");
            }

            var module = appDb.Modules.Find(id);

            if (module == null)
            {
                throw new HttpException(404, "Modül Bulunamadı.");
            }

            var model = new ModulePermissionsViewModel();
            model.RelatedModule = module;


            if (targetId != null)
            {
                model.AccessControlListId = Convert.ToInt32(targetId);
            }
            else
            {
                model.AccessControlListId = appDb.AccessControlLists.Where(acl => acl.ModuleId == module.Id).FirstOrDefault() != null ? appDb.AccessControlLists.Where(acl => acl.ModuleId == module.Id).FirstOrDefault().Id : 0;
            }

            model.AccessControlLists = new SelectList(appDb.AccessControlLists.ToList(), "Id", "Name");


            return View(model);
        }

        [HttpPost]
        public ActionResult EditModulePermissions(ModulePermissionsViewModel model)
        {

            if (model == null)
            {
                throw new HttpException(400, "Geçersiz İstek.");
            }

            var module = appDb.Modules.Find(model.RelatedModule.Id);
            var desiredAcl = appDb.AccessControlLists.Find(model.AccessControlListId);

            if (module == null || desiredAcl == null)
            {
                throw new HttpException(404, "Modül veya İzin Listesi Bulunamadı.");
            }

            var previousAcls = appDb.AccessControlLists.Where(m => m.ModuleId == module.Id).ToList();

            foreach (var item in previousAcls)
            {
                item.ModuleId = null;
                appDb.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            desiredAcl.ModuleId = module.Id;

            appDb.Entry(desiredAcl).State = System.Data.Entity.EntityState.Modified;

            appDb.SaveChanges();

            return RedirectToAction("ModulePermissions");
        }

        public ActionResult CreateNewAccessControlList(string returntomodules, string returnto)
        {
            var model = new AccessControlListViewModel();
            model.Users = new MultiSelectList(appDb.AppUsers.Where(a => !a.IsDeleted).ToList(), "Id", "Username");

            if (returntomodules == "true")
            {
                TempData["ReturnToModules"] = true;
                TempData["ReturnId"] = returnto;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewAccessControlList(AccessControlListViewModel model)
        {
            var selectedusers = new List<ApplicationUser>();

            foreach (var item in model.ApplicationUsers)
            {
                var selectedUser = appDb.AppUsers.Where(au => au.Username == item.Username).FirstOrDefault();
                if (selectedUser != null)
                {
                    selectedusers.Add(selectedUser);
                }
            }


            model.ApplicationUsers = selectedusers;
            model.Users = new MultiSelectList(appDb.AppUsers.Where(a => !a.IsDeleted).ToList(), "Id", "Username");

            var willreturn = false;
            var returnId = "0";
            if (TempData.Keys.Contains("ReturnToModules") && TempData.Keys.Contains("ReturnId"))
            {
                willreturn = (bool)TempData["ReturnToModules"];
                returnId = TempData["ReturnId"].ToString();

            }

            var accesslist = new AccessControlList();
            accesslist.Name = model.Name;

            appDb.AccessControlLists.Add(accesslist);
            appDb.Entry(accesslist).State = System.Data.Entity.EntityState.Added;


            foreach (var item in selectedusers)
            {
                var aclEntry = new ACLEntry() { CanAccessModule = true, User = item, ACL = accesslist };
                appDb.ACLEntries.Add(aclEntry);
                appDb.Entry(aclEntry).State = System.Data.Entity.EntityState.Added;

            }

            appDb.SaveChanges();

            if (willreturn)
            {
                return RedirectToAction("EditModulePermissions", new { id = returnId, targetId = accesslist.Id });
            }

            return RedirectToAction("AccessControlLists");
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

        public ActionResult AccessControlLists()
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