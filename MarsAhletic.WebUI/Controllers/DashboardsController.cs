using MarsAhletic.WebUI.Helpers;
using MarsAhletic.WebUI.Interfaces;
using MarsAhletic.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MarsAhletic.WebUI.Controllers
{
    [Authorize]
    public class DashboardsController : Controller
    {
        private ISynchroniseData syncData;


        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            //Sync Database
            syncData = new SynchroniseData();
            syncData.SyncCompanyData();
            syncData.SyncCurrencies();
            syncData.SyncProducts();
            return base.BeginExecute(requestContext, callback, state);
        }

        // GET: Dashboards
        public ActionResult CreateNewPlanning()
        {

            var model = new TravelPlanViewModel();

            using (var appDb = new ApplicationDbContext())
            {

                model.TravelId = "#" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8).ToUpper();
                model.ExpanseCenters = new MultiSelectList(appDb.CostCenters.ToList(), "Id", "Name");
                model.ExpanseItems = new MultiSelectList(appDb.ExpanseItems.ToList(), "Id", "Name");
            }

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
            var model = new PurchaseOrderViewModel();



            model.PurchaseOrderId = "#" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8).ToUpper();

            using (var appDb = new ApplicationDbContext())
            {
                model.Products = new MultiSelectList(appDb.Products.ToList(), "Id", "Name");
                model.Companies = new MultiSelectList(appDb.Companies.ToList(), "Id", "Name");
                model.Currencies = new MultiSelectList(appDb.Currencies.ToList(), "ExternalId", "Name");
            }

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

            using (var appDb = new ApplicationDbContext())
            {
                model.Products = new MultiSelectList(appDb.Products.ToList(), "Id", "Name");
                model.Companies = new MultiSelectList(appDb.Companies.ToList(), "Id", "Name", new string[] { model.CompanyId });
                model.Currencies = new MultiSelectList(appDb.Currencies.ToList(), "ExternalId", "Name");

                ViewBag.IsPostBack = true;

                if (ModelState.IsValid)
                {

                    var purchaseOrder = new PurchaseOrder();
                    var user = appDb.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                    //Assign Created By
                    purchaseOrder.CreatedBy = user;

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
                            comment.User = user;
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
        }

        public ActionResult ViewPurchaseOrder(int id)
        {
            using (var db = new ApplicationDbContext())
            {

                var purchaseOrder = db.PurchaseOrders
                    .Include("Comments")
                    .Include("CreatedBy")
                    .Include("Documents")
                    .Include("Company")
                    .Include("PurchaseDetails")
                    .Include("PurchaseDetails.Product")
                    .Include("PurchaseDetails.Product.Company")
                    .Include("PurchaseDetails.Product.Currency")
                    .Where(po => po.Id == id)
                    .FirstOrDefault();

                if (purchaseOrder == null)
                {
                    return HttpNotFound("Aranılan satınalma talebi veritabanında bulunamadı.");
                }

                var purchaseOrderVM = new PurchaseOrderViewModel();
                purchaseOrderVM.Comments = purchaseOrder.Comments.ToList();
                purchaseOrderVM.PurchaseDetails = purchaseOrder.PurchaseDetails.ToList();
                purchaseOrderVM.Date = purchaseOrder.OrderDate;
                purchaseOrderVM.CompanyCode = purchaseOrder.Company.Code;
                purchaseOrderVM.NameSurname = purchaseOrder.CreatedBy.Name;
                purchaseOrderVM.PurchaseOrderId = purchaseOrder.PurchaseOrderCode;
                purchaseOrderVM.TotalValue = purchaseOrder.TotalValue;
                purchaseOrderVM.TotalValueWithoutVAT = purchaseOrder.TotalValueWithoutVAT;
                purchaseOrderVM.TotalValueWithVAT = purchaseOrder.TotalValueWithVAT;
                purchaseOrderVM.Documents = purchaseOrder.Documents.ToList();

                purchaseOrderVM.Companies = new MultiSelectList(db.Companies.ToList(), "Id", "Name", new int[] { purchaseOrder.Company.Id });

                return View(purchaseOrderVM);
            }
        }

        public ActionResult CreateNewGuestApproval()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetBudgetForExpanseItem(int BudgetId)
        {
            using (var appdb = new ApplicationDbContext())
            {
                var budget = appdb.ExpanseItems.Where(x => x.Id == BudgetId).SingleOrDefault();

                if (budget != null)
                {
                    return Json(budget.Budget);

                }

                return null;
            }
        }

        [HttpPost]
        public ActionResult GetCurrencyValue(string CurrencyId)
        {
            var exOps = new ExternalSystemOperations();
            var currencyValue = exOps.GetCurrencyValue(CurrencyId);

            return Json(currencyValue);
        }

        [HttpPost]
        public ActionResult GetProduct(int ProductId)
        {
            using (var db = new ApplicationDbContext())
            {
                var product = db.Products
                    .Include("Currency")
                    .Include("Company")
                    .Where(x => x.Id == ProductId).FirstOrDefault();
                var exops = new ExternalSystemOperations();

                if (product == null)
                {
                    return HttpNotFound("Ürün bulunamadı");
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
        }

        [HttpPost]
        public JsonResult GetCompanyId(string ProductCode)
        {
            int companyId = 0;

            using (var db = new ApplicationDbContext())
            {
                var product = db.Products.Include("Company").Where(p => p.Code == ProductCode).FirstOrDefault();



                if (product != null)
                {
                    if (product.Company != null)
                    {
                        return Json(new { CompanyId = product.Company.Id });
                    }
                }
            }

            return Json(new { CompanyId = companyId });

        }

        [HttpPost]
        public JsonResult GetCompanyCode(int CompanyId)
        {
            using (var db = new ApplicationDbContext())
            {

                var company = db.Companies.Where(c => c.Id == CompanyId).FirstOrDefault();

                if (company != null)
                {
                    return Json(company.Code);
                }
                else
                {
                    return null;
                }

            }
        }

        [HttpPost]
        public JsonResult GetProducts(int CompanyId)
        {
            using (var db = new ApplicationDbContext())
            {
                var products = db.Products.Include("Company").Where(p => p.Company.Id == CompanyId).Select(c => new ProductEx { Name = c.Name, Id = c.Id.ToString() }).ToList();

                return Json(products);
            }


        }

        [HttpPost]
        public JsonResult GetAllProducts()
        {
            using (var db = new ApplicationDbContext())
            {
                var products = db.Products.Select(c => new ProductEx { Name = c.Name, Id = c.Id.ToString() }).ToList();

                return Json(products);
            }

        }
    }
}