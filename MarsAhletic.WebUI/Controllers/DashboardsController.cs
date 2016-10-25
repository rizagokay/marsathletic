using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarsAhletic.WebUI.Controllers
{
    public class DashboardsController : Controller
    {
        // GET: Dashboards
        public ActionResult CreateNewPlanning()
        {
            return View();
        }

        public ActionResult CreateNewGuestApproval()
        {
            return View();
        }
    }
}