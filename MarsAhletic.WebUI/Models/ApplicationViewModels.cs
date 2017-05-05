using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarsAhletic.WebUI.Models
{
    public class LoginAccountViewModel
    {
        [Required]
        [MinLength(6, ErrorMessage = "Kullanıcı adı minimum 6 karakter olmalıdır.")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        public string Domain { get; set; }

        public bool IsAdministrator { get; set; }
        public bool CreateUser { get; set; }
        public int AccountType { get; set; }

        public int[] SelectedDepartments { get; set; }
        public MultiSelectList Departments { get; set; }

    }

    public class ListViewModel
    {

        public ListViewModel()
        {
            DefinedLists = new List<DocumentViewModel>();
        }

        public List<DocumentViewModel> DefinedLists { get; set; }
    }

    public class DocumentViewModel
    {
        public string Name { get; set; }
        public int ListId { get; set; }
        public int? FileId { get; set; }
        public HttpPostedFileBase FilePath { get; set; }
    }

    public class AddNewListViewModel
    {
        public string Name { get; set; }
        public HttpPostedFileBase FilePath { get; set; }
    }

    public class PurchaseListViewModel
    {

        public PurchaseListViewModel()
        {
            PurchaseOrders = new List<PurchaseOrder>();
        }

        public int AllPurchaseCount { get; set; }
        public int ApprovedPurchaseCount { get; set; }
        public int RejectedPurchaseCount { get; set; }
        public IEnumerable<PurchaseOrder> PurchaseOrders { get; set; }


    }

    public class DomainIntegrationViewModel
    {
        [Display(Name = "Domain Adı")]
        public string DomainName { get; set; }
        [Display(Name = "Kullanıcı Adı")]
        public string DomainAdministratorAccount { get; set; }
        [Display(Name = "Şifre")]
        public string DomainAdministratorPassword { get; set; }

        public bool IsActive { get; set; }
    }

    public class TravelPlanViewModel
    {
        public TravelPlanViewModel()
        {
            PlanDetails = new List<TravelPlanDetail>();
        }

        public string TravelId { get; set; }
        public string NameSurname { get; set; }
        public string ExpanseCenterId { get; set; }
        public DateTime Date { get; set; }
        public string TravelRoute { get; set; }

        public List<TravelPlanDetail> PlanDetails { get; set; }

        public MultiSelectList ExpanseCenters { get; set; }
        public MultiSelectList ExpanseItems { get; set; }
    }

    public class PurchaseOrderViewModel
    {

        public PurchaseOrderViewModel()
        {
            PurchaseDetails = new List<PurchaseDetail>();
            Comments = new List<Comment>();
            Files = new List<HttpPostedFileBase>();
        }

        public string PurchaseOrderId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "alanı gereklidir")]
        [Display(Name = "Adı Soyadı")]
        public string NameSurname { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "alanı gereklidir.")]
        [Display(Name = "Tarih")]
        public DateTime Date { get; set; }
        public string CompanyId { get; set; }
        public string CompanyCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "alanı gereklidir.")]
        [Display(Name = "Genel Toplam")]
        public decimal TotalValue { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "alanı gereklidir.")]
        [Display(Name = "KDV Toplamı")]
        public decimal TotalValueWithVAT { get; set; }
        [Required(ErrorMessage = "alanı gereklidir.")]
        [Display(Name = "KDV'siz Toplam")]
        public decimal TotalValueWithoutVAT { get; set; }
        public string AddedComment { get; set; }

        public string MFilesId { get; set; }
        public int MFilesProcessId { get; set; }
        public int MFilesStateId { get; set; }
        public bool MFilesProcessEnded { get; set; }

        public List<Document> Documents { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
        public List<PurchaseDetail> PurchaseDetails { get; set; }
        public List<Comment> Comments { get; set; }
        public MultiSelectList Companies { get; set; }
        public MultiSelectList Products { get; set; }
        public MultiSelectList Currencies { get; set; }
        public MultiSelectList BudgetTypes { get; set; }

    }


    public class AccessControlListViewModel
    {
        public AccessControlListViewModel()
        {
            ApplicationUsers = new List<ApplicationUser>();
        }

        [Display(Name = "Adı")]
        public string Name { get; set; }

        public MultiSelectList Users { get; set; }

        public List<ApplicationUser> ApplicationUsers { get; set; }
    }

    public class ModulePermissionsViewModel
    {
        public Module RelatedModule { get; set; }
        public int AccessControlListId { get; set; }
        public MultiSelectList AccessControlLists { get; set; }
    }

    public class ReportsViewModel
    {
        public ReportsViewModel()
        {
            Result = new List<PurchaseOrder>();
        }

        public DateTime firstDate { get; set; }
        public DateTime secondDate { get; set; }
        public int[] Products { get; set; }
        public int[] CostCenter { get; set; }
        public int[] Company { get; set; }
        public int[] States { get; set; }
        public IEnumerable<PurchaseOrder> Result { get; set; }


        public MultiSelectList Companies { get; set; }
        public MultiSelectList ProductList { get; set; }
        public MultiSelectList CoastCenters { get; set; }
        public MultiSelectList StateList { get; set; }



    }



    public class AddNewUserViewModel
    {
        public string LoginAccountId { get; set; }
        public int? Department { get; set; }
        public string DepartmentText { get; set; }
        public int? Office { get; set; }
        public string OfficeText { get; set; }
        public bool IsDeactive { get; set; }
        public bool IsDeptManager { get; set; }
        public bool IsHighManager { get; set; }

        public MultiSelectList LoginAccounts { get; set; }
        public MultiSelectList Departments { get; set; }
        public MultiSelectList Offices { get; set; }
    }

    public class EditUserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string OldLoginAccountId { get; set; }
        public string LoginAccountId { get; set; }
        public int? Department { get; set; }
        public string DepartmentText { get; set; }
        public string OfficeText { get; set; }
        public int? Office { get; set; }
        public bool IsDeactive { get; set; }
        public bool IsDeptManager { get; set; }
        public bool IsHighManager { get; set; }

        public MultiSelectList LoginAccounts { get; set; }
        public MultiSelectList Departments { get; set; }
        public MultiSelectList Offices { get; set; }
    }

}