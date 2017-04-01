using MarsAhletic.WebUI.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarsAhletic.WebUI.Helpers
{
    public class PermissionsHelper
    {
        public static bool CanAccessPurchasingModule(string UserId)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Where(x => x.UserName == UserId).FirstOrDefault();

                if (user != null)
                {
                    //Check Roles
                    foreach (var item in user.Roles)
                    {
                        var role = db.Roles.Where(x => x.Id == item.RoleId).FirstOrDefault();
                        if (role != null)
                        {
                            if (role.Name == "Administrators")
                            {
                                return true;
                            }
                        }
                    }

                    var module = db.Modules.Where(m => m.Name == "PurchaseOrders").FirstOrDefault();


                    if (module != null)
                    {
                        var aclInUse = db.AccessControlLists.Where(acl => acl.Module.Id == module.Id).FirstOrDefault();

                        if (aclInUse != null)
                        {
                            foreach (var item in aclInUse.ACLEntries)
                            {
                                if (item.User != null)
                                {
                                    if (item.User.LoginAccount != null)
                                    {
                                        if (item.User.LoginAccount.UserName == UserId)
                                        {
                                            if (item.CanAccessModule)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}