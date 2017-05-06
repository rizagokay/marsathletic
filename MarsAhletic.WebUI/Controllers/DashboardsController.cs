using MarsAhletic.WebUI.Helpers;
using MarsAhletic.WebUI.Interfaces;
using MarsAhletic.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Dynamic;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace MarsAhletic.WebUI.Controllers
{
    [Authorize]
    public class DashboardsController : Controller
    {
        private ISynchroniseData syncData;

        private ApplicationDbContext appDb;

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            //Sync Database
            syncData = new SynchroniseData();
            syncData.SyncCompanyData();
            syncData.SyncCurrencies();
            syncData.SyncProducts();

            //Bind ApplicationDb
            appDb = new ApplicationDbContext();
            return base.BeginExecute(requestContext, callback, state);
        }

        protected override void Dispose(bool disposing)
        {
            appDb.Dispose();
            base.Dispose(disposing);
        }

        // GET: Dashboards
        public ActionResult CreateNewPlanning()
        {

            var model = new TravelPlanViewModel();

            model.TravelId = "#" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8).ToUpper();
            model.ExpanseCenters = new MultiSelectList(appDb.CostCenters.ToList(), "Id", "Name");
            model.ExpanseItems = new MultiSelectList(appDb.ExpanseItems.ToList(), "Id", "Name");


            if (User != null)
            {
                model.NameSurname = User.Identity.GetName();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewPlanning(TravelPlanViewModel model)
        {
            return View(model);
        }

        public ActionResult CreateNewPurchaseOrder()
        {
            if (!PermissionsHelper.CanAccessPurchasingModule(User.Identity.Name))
            {
                throw new HttpException(403, "Yetersiz Yetki");
            }


            var model = new PurchaseOrderViewModel();



            model.PurchaseOrderId = "#" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8).ToUpper();

            model.Products = new MultiSelectList(appDb.Products.ToList(), "Id", "Name");
            model.Companies = new MultiSelectList(appDb.Companies.ToList(), "Id", "Name");
            model.Currencies = new MultiSelectList(appDb.Currencies.ToList(), "ExternalId", "Name");


            if (User != null)
            {
                model.NameSurname = User.Identity.GetName();
            }

            ViewBag.IsPostBack = false;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewPurchaseOrder(PurchaseOrderViewModel model)
        {


            model.Products = new MultiSelectList(appDb.Products.ToList(), "Id", "Name");
            model.Companies = new MultiSelectList(appDb.Companies.ToList(), "Id", "Name", new string[] { model.CompanyId });
            model.Currencies = new MultiSelectList(appDb.Currencies.ToList(), "ExternalId", "Name");

            ViewBag.IsPostBack = true;

            if (ModelState.IsValid)
            {

                var purchaseOrder = new PurchaseOrder();
                var user = appDb.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                ApplicationUser appUser;

                if (user == null)
                {
                    throw new HttpException(403, "Mevcut kullanıcının giriş hesabı bulunamadı.");
                }
                else
                {
                    appUser = appDb.AppUsers.Where(x => x.LoginAccount.Id == user.Id && !x.IsDisabled).FirstOrDefault();
                    if (appUser == null)
                    {
                        throw new HttpException(403, "Mevcut kullanıcının giriş hesabı bulunamadı.");
                    }
                }

                //Assign Created By
                purchaseOrder.CreatedBy = appUser;

                //Find Company
                var company = appDb.Companies.Where(c => c.Id.ToString() == model.CompanyId).FirstOrDefault();

                if (company == null)
                {
                    ModelState.AddModelError("", "Seçilen firma veritabanında bulunamadı.");
                    return View(model);
                }

                purchaseOrder.Company = company;

                //Set Other Fields
                purchaseOrder.OrderDate = model.Date;
                purchaseOrder.TotalValue = model.TotalValue;
                purchaseOrder.TotalValueWithoutVAT = model.TotalValueWithoutVAT;
                purchaseOrder.TotalValueWithVAT = model.TotalValueWithVAT;
                purchaseOrder.PurchaseOrderCode = model.PurchaseOrderId;

                //Add Comments
                if (model.AddedComment != null)
                {
                    if (model.AddedComment.Trim() != "")
                    {
                        var comment = new Comment();
                        comment.User = appUser;
                        comment.CommentDate = DateTime.Now;
                        comment.Message = model.AddedComment;

                        purchaseOrder.Comments = new List<Comment>() { comment };
                    }
                }

                var purchaseDetails = new List<PurchaseDetail>();

                foreach (var item in model.PurchaseDetails)
                {
                    var productCode = item.Product.Code;
                    var product = appDb.Products.Where(p => p.Code == productCode).FirstOrDefault();
                    if (productCode == null)
                    {
                        ModelState.AddModelError("", "Seçilen ürünlerden biri veritabanında bulunamadı.");
                        return View(model);
                    }

                    //Create Purchase Detail
                    var purchaseDetail = new PurchaseDetail();
                    purchaseDetail.Amount = item.Amount;
                    purchaseDetail.Product = product;
                    purchaseDetail.Value = item.Value;
                    purchaseDetail.ValueLocal = item.ValueLocal;

                    //Add Currency
                    var currency = appDb.Currencies.Where(c => c.Name == item.Currency.Name).FirstOrDefault();
                    if (currency == null)
                    {
                        ModelState.AddModelError("", "Seçilen ürünlerden birisine ait para birimi veritabanında bulunamadı.");
                        return View(model);
                    }

                    purchaseDetail.Currency = currency;
                    purchaseDetails.Add(purchaseDetail);

                }

                var documents = new List<Document>();

                //Save Files
                foreach (var item in model.Files)
                {
                    if (item != null)
                    {
                        var targetFolder = Server.MapPath("~/uploads/attachments/purchaseorders/");

                        if (!Directory.Exists(targetFolder))
                            Directory.CreateDirectory(targetFolder);

                        var filename = Guid.NewGuid().ToString() + ".bin";
                        var targetPath = Path.Combine(targetFolder, filename);
                        item.SaveAs(targetPath);

                        var document = new Document()
                        {
                            Extension = Path.GetExtension(item.FileName),
                            FullName = item.FileName,
                            StoredName = filename
                        };

                        documents.Add(document);
                    }
                }


                purchaseOrder.Documents = documents;
                purchaseOrder.PurchaseDetails = purchaseDetails;

                var addedObject = appDb.PurchaseOrders.Add(purchaseOrder);


                try
                {
                    appDb.SaveChanges();
                    return RedirectToAction("ViewPurchaseOrder", new { id = addedObject.Id });
                }
                catch (Exception Ex)
                {
                    ModelState.AddModelError("", "Kayıt edilirken bir hata ile karşılaşıldı.");
                    ModelState.AddModelError("", Ex);
                }

            }


            return View(model);

        }

        [HttpPost]
        public ActionResult ViewPurchaseOrder(PurchaseOrderViewModel model)
        {
            var user = appDb.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ApplicationUser appUser;

            if (user == null && !User.IsInRole("Administrators"))
            {

                throw new HttpException(403, "Mevcut kullanıcının giriş hesabı bulunamadı.");
            }
            else
            {
                appUser = appDb.AppUsers.Where(x => x.LoginAccount.Id == user.Id && !x.IsDisabled).FirstOrDefault();
                if (appUser == null && !User.IsInRole("Administrators"))
                {
                    throw new HttpException(403, "Mevcut kullanıcının giriş hesabı bulunamadı.");
                }
            }

            var purchaseOrder = appDb.PurchaseOrders
                .Where(po => po.PurchaseOrderCode == model.PurchaseOrderId)
                .FirstOrDefault();


            if (model.AddedComment != null)
            {
                if (model.AddedComment.Trim() != "")
                {
                    var comment = new Comment();
                    comment.User = appUser;
                    comment.CommentDate = DateTime.Now;
                    comment.Message = model.AddedComment;

                    purchaseOrder.Comments = new List<Comment>() { comment };
                }
            }

            appDb.Entry(purchaseOrder).State = System.Data.Entity.EntityState.Modified;
            appDb.SaveChanges();


            return RedirectToAction("ViewPurchaseOrder", new { id = purchaseOrder.Id });


        }

        public ActionResult ViewPurchaseOrder(int id)
        {
            if (!PermissionsHelper.CanAccessPurchasingModule(User.Identity.Name))
            {
                throw new HttpException(403, "Yetersiz Yetki");
            }


            var purchaseOrder = appDb.PurchaseOrders
                .Where(po => po.Id == id)
                .FirstOrDefault();

            if (purchaseOrder == null)
            {
                throw new HttpException(404, "Satınalma Talebi Bulunamadı.");
            }

            var purchaseOrderVM = new PurchaseOrderViewModel();
            purchaseOrderVM.Comments = purchaseOrder.Comments.ToList();
            purchaseOrderVM.PurchaseDetails = purchaseOrder.PurchaseDetails.ToList();
            purchaseOrderVM.Date = purchaseOrder.OrderDate;
            purchaseOrderVM.CompanyCode = purchaseOrder.Company.Code;
            purchaseOrderVM.NameSurname = purchaseOrder.CreatedBy.LoginAccount.Name;
            purchaseOrderVM.PurchaseOrderId = purchaseOrder.PurchaseOrderCode;
            purchaseOrderVM.TotalValue = purchaseOrder.TotalValue;
            purchaseOrderVM.TotalValueWithoutVAT = purchaseOrder.TotalValueWithoutVAT;
            purchaseOrderVM.TotalValueWithVAT = purchaseOrder.TotalValueWithVAT;
            purchaseOrderVM.Documents = purchaseOrder.Documents.ToList();

            purchaseOrderVM.Companies = new MultiSelectList(appDb.Companies.ToList(), "Id", "Name", new int[] { purchaseOrder.Company.Id });

            return View(purchaseOrderVM);

        }

        public ActionResult AllPurchaseOrders()
        {
            if (!PermissionsHelper.CanAccessPurchasingModule(User.Identity.Name))
            {
                throw new HttpException(403, "Yetersiz Yetki");
            }

            var model = new PurchaseListViewModel();

            var appuserId = User.Identity.GetUserId();
            var loginAccount = appDb.AppUsers.Where(a => a.LoginAccountId == appuserId && !a.IsDisabled).FirstOrDefault();

            if (loginAccount == null && !User.IsInRole("Administrators"))
            {
                throw new HttpException(403, "Mevcut kullanıcının giriş hesabı bulunamadı.");
            }

            var purchaseOrders = new List<PurchaseOrder>();

            //Check Role
            if (User.IsInRole("Administrators") || loginAccount.IsHighManager)
            {
                purchaseOrders = appDb.PurchaseOrders.ToList();

            }
            else if (loginAccount.IsDeptManager && !User.IsInRole("Administrators"))
            {
                var currentusersDepts = loginAccount.Departments.ToList();

                foreach (var item in currentusersDepts)
                {
                    var purchaseorderOfDepartments = appDb.PurchaseOrders.Where(x => x.CreatedBy.Departments.Any(d => d.Id == item.Id)).ToList();

                    foreach (var purchaseOrder in purchaseorderOfDepartments)
                    {
                        if (!purchaseOrders.Select(p => p.Id).Contains(purchaseOrder.Id))
                        {
                            purchaseOrders.Add(purchaseOrder);
                        }
                    }
                }

            }
            else
            {
                purchaseOrders = appDb.PurchaseOrders.Where(x => x.CreatedBy.Id == loginAccount.Id).ToList();
            }


            model.PurchaseOrders = purchaseOrders;
            model.AllPurchaseCount = purchaseOrders.Count;
            model.ApprovedPurchaseCount = purchaseOrders.Where(x => x.MFilesProcessEndedWithApproval).Count();
            model.RejectedPurchaseCount = purchaseOrders.Where(x => x.MFilesProcessEnded && !x.MFilesProcessEndedWithApproval).Count();

            return View(model);

        }

        public ActionResult OnGoingPurchaseOrders()
        {

            if (!PermissionsHelper.CanAccessPurchasingModule(User.Identity.Name))
            {
                throw new HttpException(403, "Yetersiz Yetki");
            }


            var model = new PurchaseListViewModel();

            var appuserId = User.Identity.GetUserId();
            var loginAccount = appDb.AppUsers.Where(a => a.LoginAccountId == appuserId && !a.IsDisabled).FirstOrDefault();


            if (loginAccount == null && !User.IsInRole("Administrators"))
            {
                throw new HttpException(403, "Mevcut kullanıcının giriş hesabı bulunamadı.");
            }

            var purchaseOrders = new List<PurchaseOrder>();

            //Check Role
            if (User.IsInRole("Administrators") || loginAccount.IsHighManager)
            {
                purchaseOrders = appDb.PurchaseOrders.ToList();

            }
            else if (loginAccount.IsDeptManager && !User.IsInRole("Administrators"))
            {
                var currentusersDepts = loginAccount.Departments.ToList();

                foreach (var item in currentusersDepts)
                {
                    var purchaseorderOfDepartments = appDb.PurchaseOrders.Where(x => x.CreatedBy.Departments.Any(d => d.Id == item.Id)).ToList();

                    foreach (var purchaseOrder in purchaseorderOfDepartments)
                    {
                        if (!purchaseOrders.Select(p => p.Id).Contains(purchaseOrder.Id))
                        {
                            purchaseOrders.Add(purchaseOrder);
                        }
                    }
                }

            }
            else
            {
                purchaseOrders = appDb.PurchaseOrders.Where(x => x.CreatedBy.Id == loginAccount.Id).ToList();
            }


            model.PurchaseOrders = purchaseOrders.Where(x => !x.MFilesProcessEnded).ToList();
            model.AllPurchaseCount = purchaseOrders.Count;
            model.ApprovedPurchaseCount = purchaseOrders.Where(x => x.MFilesProcessEndedWithApproval).Count();
            model.RejectedPurchaseCount = purchaseOrders.Where(x => x.MFilesProcessEnded && !x.MFilesProcessEndedWithApproval).Count();

            return View(model);




        }

        public ActionResult ApprovedPurchaseOrders()
        {

            if (!PermissionsHelper.CanAccessPurchasingModule(User.Identity.Name))
            {
                throw new HttpException(403, "Yetersiz Yetki");
            }


            var model = new PurchaseListViewModel();

            var appuserId = User.Identity.GetUserId();
            var loginAccount = appDb.AppUsers.Where(a => a.LoginAccountId == appuserId && !a.IsDisabled).FirstOrDefault();

            if (loginAccount == null && !User.IsInRole("Administrators"))
            {
                throw new HttpException(403, "Mevcut kullanıcının giriş hesabı bulunamadı.");
            }

            var purchaseOrders = new List<PurchaseOrder>();


            if (User.IsInRole("Administrators") || loginAccount.IsHighManager)
            {
                purchaseOrders = appDb.PurchaseOrders.ToList();

            }
            else if (loginAccount.IsDeptManager && !User.IsInRole("Administrators"))
            {
                var currentusersDepts = loginAccount.Departments.ToList();

                foreach (var item in currentusersDepts)
                {
                    var purchaseorderOfDepartments = appDb.PurchaseOrders.Where(x => x.CreatedBy.Departments.Any(d => d.Id == item.Id)).ToList();

                    foreach (var purchaseOrder in purchaseorderOfDepartments)
                    {
                        if (!purchaseOrders.Select(p => p.Id).Contains(purchaseOrder.Id))
                        {
                            purchaseOrders.Add(purchaseOrder);
                        }
                    }
                }

            }
            else
            {
                purchaseOrders = appDb.PurchaseOrders.Where(x => x.CreatedBy.Id == loginAccount.Id).ToList();
            }


            model.PurchaseOrders = purchaseOrders.Where(x => x.MFilesProcessEndedWithApproval).ToList();
            model.AllPurchaseCount = purchaseOrders.Count;
            model.ApprovedPurchaseCount = purchaseOrders.Where(x => x.MFilesProcessEndedWithApproval).Count();
            model.RejectedPurchaseCount = purchaseOrders.Where(x => x.MFilesProcessEnded && !x.MFilesProcessEndedWithApproval).Count();

            return View(model);


        }

        public ActionResult RejectedPurchaseOrders()
        {

            if (!PermissionsHelper.CanAccessPurchasingModule(User.Identity.Name))
            {
                throw new HttpException(403, "Yetersiz Yetki");
            }


            var model = new PurchaseListViewModel();

            var appuserId = User.Identity.GetUserId();
            var loginAccount = appDb.AppUsers.Where(a => a.LoginAccountId == appuserId && !a.IsDisabled).FirstOrDefault();


            if (loginAccount == null && !User.IsInRole("Administrators"))
            {
                throw new HttpException(403, "Mevcut kullanıcının giriş hesabı bulunamadı.");
            }


            var purchaseOrders = new List<PurchaseOrder>();


            if (User.IsInRole("Administrators") || loginAccount.IsHighManager)
            {
                purchaseOrders = appDb.PurchaseOrders.ToList();

            }
            else if (loginAccount.IsDeptManager && !User.IsInRole("Administrators"))
            {
                var currentusersDepts = loginAccount.Departments.ToList();

                foreach (var item in currentusersDepts)
                {
                    var purchaseorderOfDepartments = appDb.PurchaseOrders.Where(x => x.CreatedBy.Departments.Any(d => d.Id == item.Id)).ToList();

                    foreach (var purchaseOrder in purchaseorderOfDepartments)
                    {
                        if (!purchaseOrders.Select(p => p.Id).Contains(purchaseOrder.Id))
                        {
                            purchaseOrders.Add(purchaseOrder);
                        }
                    }
                }

            }
            else
            {
                purchaseOrders = appDb.PurchaseOrders.Where(x => x.CreatedBy.Id == loginAccount.Id).ToList();
            }

            model.PurchaseOrders = purchaseOrders.Where(x => x.MFilesProcessEnded && !x.MFilesProcessEndedWithApproval).ToList();
            model.AllPurchaseCount = purchaseOrders.Count;
            model.ApprovedPurchaseCount = purchaseOrders.Where(x => x.MFilesProcessEndedWithApproval).Count();
            model.RejectedPurchaseCount = purchaseOrders.Where(x => x.MFilesProcessEnded && !x.MFilesProcessEndedWithApproval).Count();

            return View(model);

        }

        public ActionResult CreateNewGuestApproval()
        {
            return View();
        }

        //public ActionResult RequestReportView()
        //{
        //    var model = new ReportViewModel();

        //    model.Companies = new MultiSelectList(appDb.Companies.ToList(), "Id", "Name");
        //    model.ProductList = new MultiSelectList(appDb.Products.ToList(), "Id", "Name");
        //    model.CoastCenters = new MultiSelectList(appDb.CostCenters.ToList(), "Id", "Name");
        //    model.StateList = new MultiSelectList(new List<object>() { new { Id = 1, Name = "Onaylananlar" }, new { Id = 2, Name = "Reddedilenler" }, new { Id = 3, Name = "Onayda Bekleyenler" } }, "Id", "Name");
        //    return View(model);
        //}

        public ActionResult RequestReportView()
        {

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ProductReport.rdlc";

            var appDbSource = appDb.PurchaseOrders.ToList();

            MPortDbDataSet dbSet = new MPortDbDataSet();

            ReportParameter rp1 = new ReportParameter("Sube", "1");
            // ReportParameter rp2 = new ReportParameter("Parameter2");

            var c1 = new SqlParameter("@CompanyIDs", 1);
            var c2 = new SqlParameter("@ProductIDs", 3);
            var c3 = new SqlParameter("@CostCenterIDs", 1);
            var c4 = new SqlParameter("@firstDate", "2010-12-12");
            var c5 = new SqlParameter("@secondDate", "2019-12-12");

            var result = appDb.Database
                .SqlQuery<PurchaseDetailReportItem>("GetReportData @CompanyIDs, @ProductIDs, @CostCenterIDs, @firstDate, @secondDate", c1, c2, c3, c4, c5)
                .ToList();

            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("MainDataset", result));

            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        [HttpPost]
        public ActionResult RequestReportView(ReportViewModel model)
        {
            var user = appDb.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ApplicationUser appUser;

            if (user == null)
            {
                throw new HttpException(404, "Mevcut kullanıcının giriş hesabı bulunamadı.");
            }
            else
            {
                appUser = appDb.AppUsers.Where(x => x.LoginAccount.Id == user.Id).FirstOrDefault();
                if (appUser == null)
                {
                    throw new HttpException(404, "Mevcut kullanıcının hesabı bulunamadı.");
                }
            }

            if (model.AllProducts)
            {
                model.Products = appDb.PurchaseDetails.Select(s => s.Product.Id).ToArray();
            }

            if (model.AllCompanies)
            {
                model.Company = appDb.PurchaseDetails.Select(s => s.PurchaseOrder.Company.Id).ToArray();
            }

            if (model.AllCostCenter)
            {
                model.CostCenter = appDb.PurchaseDetails.Select(s => s.PurchaseOrder.CostCenter.Id).ToArray();
            }

            if (model.AllStates)
            {
                model.States = null;
            }

            var PurchaseOrders = new List<PurchaseOrder>();

            PurchaseOrders = appDb.PurchaseOrders.Where(x => x.OrderDate >= model.firstDate && x.OrderDate <= model.secondDate)
                .Where(x => model.CostCenter.Contains(x.CostCenter.Id))
                .Where(x => model.Company.Contains(x.Company.Id))
                .ToList();
            var PurchaseOrders2 = new List<PurchaseOrder>();
            var PurchaseOrders3 = new List<PurchaseOrder>();

            if (model.Products != null)
            {
                foreach (var item in model.Products)
                {
                    var po = PurchaseOrders.Where(x => x.PurchaseDetails.Any(p => p.Product.Id == item)).FirstOrDefault();

                    if (po != null)
                    {
                        if (!PurchaseOrders2.Select(p => p.Id).Contains(po.Id))
                        {
                            PurchaseOrders2.Add(po);
                        }
                    }
                }
            }


            List<PurchaseOrder> pos = null;
            if (model.States != null)
            {
                foreach (var item in model.States)
                {


                    //Onaylananlar
                    if (item == 1)
                    {
                        pos = PurchaseOrders2.Where(x => x.MFilesProcessEnded).ToList();

                    } //Reddedilenler
                    else if (item == 2)
                    {
                        pos = PurchaseOrders2.Where(x => x.MFilesProcessEnded && !x.MFilesProcessEndedWithApproval).ToList();
                    }//Onayda Bekleyenler
                    else
                    {
                        pos = PurchaseOrders2.Where(x => !x.MFilesProcessEnded && !x.MFilesProcessEndedWithApproval).ToList();

                    }

                    if (pos != null)
                    {
                        PurchaseOrders3 = PurchaseOrders3.Union(pos).ToList();
                    }
                }
            }

            model.Companies = new MultiSelectList(appDb.Companies.ToList(), "Id", "Name");
            model.ProductList = new MultiSelectList(appDb.Products.ToList(), "Id", "Name");
            model.CoastCenters = new MultiSelectList(appDb.CostCenters.ToList(), "Id", "Name");
            model.StateList = new MultiSelectList(new List<object>() { new { Id = 1, Name = "Onaylananlar" }, new { Id = 2, Name = "Reddedlenler" }, new { Id = 3, Name = "Onayda Bekleyenler" } }, "Id", "Name");

            if (model.States != null)
            {
                model.Result = PurchaseOrders3;
            }
            else
            {
                model.Result = PurchaseOrders2;
            }
            return View(model);
        }

        public ActionResult ProductReportView()
        {
            var model = new ReportViewModel();

            model.Companies = new MultiSelectList(appDb.Companies.ToList(), "Id", "Name");
            model.ProductList = new MultiSelectList(appDb.Products.ToList(), "Id", "Name");
            model.CoastCenters = new MultiSelectList(appDb.CostCenters.ToList(), "Id", "Name");
            model.StateList = new MultiSelectList(new List<object>() { new { Id = 1, Name = "Onaylananlar" }, new { Id = 2, Name = "Reddedilenler" }, new { Id = 3, Name = "Onayda Bekleyenler" } }, "Id", "Name");

            return View(model);

        }

        [HttpPost]
        public ActionResult ProductReportView(ReportViewModel model)
        {
            var user = appDb.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ApplicationUser appUser;

            if (user == null)
            {
                throw new HttpException(404, "Mevcut kullanıcının giriş hesabı bulunamadı.");
            }
            else
            {
                appUser = appDb.AppUsers.Where(x => x.LoginAccount.Id == user.Id).FirstOrDefault();
                if (appUser == null)
                {
                    throw new HttpException(404, "Mevcut kullanıcının hesabı bulunamadı.");
                }
            }

            if (model.AllProducts)
            {
                model.Products = appDb.PurchaseDetails.Select(s => s.Product.Id).ToArray();
            }

            if (model.AllCompanies)
            {
                model.Company = appDb.PurchaseDetails.Select(s => s.PurchaseOrder.Company.Id).ToArray();
            }

            if (model.AllCostCenter)
            {
                model.CostCenter = appDb.PurchaseDetails.Select(s => s.PurchaseOrder.CostCenter.Id).ToArray();
            }

            if (model.AllStates)
            {
                model.States = null;
            }

            var PurchaseDetails = new List<PurchaseDetail>();

            PurchaseDetails = appDb.PurchaseDetails
                .Where(x => x.PurchaseOrder.OrderDate >= model.firstDate && x.PurchaseOrder.OrderDate <= model.secondDate)
                .Where(m => model.Products.Contains(m.Product.Id))
                .Where(m => model.CostCenter.Contains(m.PurchaseOrder.CostCenter.Id))
                .Where(m => model.Company.Contains(m.PurchaseOrder.Company.Id))
                .ToList();


            var PurchaseDetails2 = new List<PurchaseDetail>();
            var po = new List<PurchaseDetail>();

            if (model.States != null)
            {
                foreach (var item in model.States)
                {


                    //Onaylananlar
                    if (item == 1)
                    {
                        po = PurchaseDetails.Where(x => x.PurchaseOrder.MFilesProcessEnded).ToList();

                    } //Reddedilenler
                    else if (item == 2)
                    {
                        po = PurchaseDetails.Where(x => x.PurchaseOrder.MFilesProcessEnded && !x.PurchaseOrder.MFilesProcessEndedWithApproval).ToList();
                    }//Onayda Bekleyenler
                    else
                    {
                        po = PurchaseDetails.Where(x => !x.PurchaseOrder.MFilesProcessEnded && !x.PurchaseOrder.MFilesProcessEndedWithApproval).ToList();
                    }

                    if (po != null)
                    {
                        PurchaseDetails2 = PurchaseDetails2.Union(po).ToList();
                    }
                }
            }
            //appDb.Database.SqlQuery<PurchaseDetail>("");

            model.Companies = new MultiSelectList(appDb.Companies.ToList(), "Id", "Name");
            model.ProductList = new MultiSelectList(appDb.Products.ToList(), "Id", "Name");
            model.CoastCenters = new MultiSelectList(appDb.CostCenters.ToList(), "Id", "Name");
            model.StateList = new MultiSelectList(new List<object>() { new { Id = 1, Name = "Onaylananlar" }, new { Id = 2, Name = "Reddedlenler" }, new { Id = 3, Name = "Onayda Bekleyenler" } }, "Id", "Name");

            if (model.States != null)
            {
                model.ResultDetails = PurchaseDetails2;
            }
            else
            {
                model.ResultDetails = PurchaseDetails;
            }



            var grouped = PurchaseDetails
                .GroupBy(p => new { p.Product, p.PurchaseOrder.CostCenter })
                .Select(g =>
                {
                    dynamic o = new ExpandoObject();
                    o.FirstProperty = g.Key.CostCenter.Name;
                    o.SecondProperty = g.Key.Product.Name;
                    o.ThirdProperty = g.Sum(p => p.Amount);
                    o.FourthProperty = g.Sum(p => p.ValueLocal);
                    return o;
                });

            model.ColumnNames = new List<string>() { "Masraf Merkezi", "Ürün Adı", "Adet", "Toplam" };
            model.Results = grouped;

            return View(model);
        }

        [HttpPost]
        public JsonResult GetBudgetForExpanseItem(int BudgetId)
        {

            var budget = appDb.ExpanseItems.Where(x => x.Id == BudgetId).SingleOrDefault();

            if (budget != null)
            {
                return Json(budget.Budget);

            }

            return null;

        }

        [HttpPost]
        public ActionResult GetCurrencyValue(string CurrencyId)
        {
            var exOps = new ExternalSystemOperations();
            var currencyValue = exOps.GetCurrencyValue(CurrencyId);

            return Json(currencyValue);
        }

        [HttpPost]
        public JsonResult GetProduct(int ProductId)
        {

            var product = appDb.Products
                .Where(x => x.Id == ProductId).FirstOrDefault();
            var exops = new ExternalSystemOperations();

            if (product == null)
            {
                throw new HttpException(404, "Ürün bulunamadı.");
            }

            var productEx = new ProductEx()
            {
                Code = product.Code,
                GroupCode = product.GroupCode,
                Id = product.ExternalId,
                UnitPrice = product.UnitPrice,
                CompanyId = product.Company != null ? product.Company.Id.ToString() : "",
                Name = product.Name,
                VATPercantage = product.VATPercentage
            };

            if (product.Currency != null)
            {
                productEx.Currency = new CurrencyEx() { Id = product.Currency.ExternalId, Name = product.Currency.Name };
            }

            return Json(productEx);

        }

        [HttpPost]
        public JsonResult GetCompanyId(string ProductCode)
        {
            int companyId = 0;


            var product = appDb.Products.Where(p => p.Code == ProductCode).FirstOrDefault();

            if (product != null)
            {
                if (product.Company != null)
                {
                    return Json(new { CompanyId = product.Company.Id });
                }
            }


            return Json(new { CompanyId = companyId });

        }

        [HttpPost]
        public JsonResult GetCompanyCode(int CompanyId)
        {
            var company = appDb.Companies.Where(c => c.Id == CompanyId).FirstOrDefault();

            if (company != null)
            {
                return Json(company.Code);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetProducts(int CompanyId)
        {
            var products = appDb.Products.Where(p => p.Company.Id == CompanyId).Select(c => new ProductEx { Name = c.Name, Id = c.Id.ToString() }).ToList();

            return Json(products);
        }

        [HttpPost]
        public JsonResult GetAllProducts()
        {
            var products = appDb.Products.Select(c => new ProductEx { Name = c.Name, Id = c.Id.ToString() }).ToList();
            return Json(products);
        }

        public FileResult Download(int Id)
        {

            var document = appDb.Documents.Find(Id);

            if (document == null)
            {
                throw new HttpException(404, "Doküman bulunamadı");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/uploads/attachments/purchaseorders/" + document.StoredName));
            string fileName = document.FullName;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }
    }
}