using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarsAhletic.WebUI.Models
{

    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeptManager { get; set; }
        public bool IsHighManager { get; set; }
        public bool IsDeleted { get; set; }
        public string Username { get; set; }
        public int? DepartmentId { get; set; }
        public int? OfficeId { get; set; }

        public string LoginAccountId { get; set; }

        [ForeignKey("LoginAccountId")]
        public virtual LoginAccount LoginAccount { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        [ForeignKey("OfficeId")]
        public virtual Office Office { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<TravelPlan> TravelPlans { get; set; }
    }

    public class Office
    {
        [Key]
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
    }


    public class Configuration
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
    }

    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string NotificationMessage { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ReadOn { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalValueWithVAT { get; set; }
        public decimal TotalValueWithoutVAT { get; set; }
        public string ExtenalId { get; set; }

        public string PurchaseOrderCode { get; set; }
        public decimal USDValueOnDate { get; set; }
        public decimal EURValueOnDate { get; set; }
        public decimal GBPValueOnDate { get; set; }

        public string MFilesId { get; set; }
        public int MFilesProcessId { get; set; }
        public int MFilesStateId { get; set; }
        public string MFilesProcessName { get; set; }
        public string MFilesStateName { get; set; }
        public bool MFilesProcessEndedWithApproval { get; set; }
        public bool MFilesProcessEnded { get; set; }

        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual Company Company { get; set; }
        public virtual CostCenter CostCenter { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
    }

    public class PurchaseDetail
    {
        [Key]
        public int Id { get; set; }
        public int Amount { get; set; }
        public decimal Value { get; set; }
        public decimal ValueLocal { get; set; }
        public bool IncludedInBudget { get; set; }
        public decimal BudgetCost { get; set; }
        public int Discount { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual BudgetType BudgetType { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual Product Product { get; set; }
    }

    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CommentDate { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }

    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Code { get; set; }
        public string GroupCode { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public long VATPercentage { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Company Company { get; set; }

        public ICollection<PurchaseDetail> PurchaseDetails { get; set; }

    }

    public class BudgetType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Currency
    {
        [Key]
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
    }

    public class CurrencyValues
    {
        [Key]
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }

        [ForeignKey("CurrencyId")]
        public Currency Currency { get; set; }
    }

    public class TravelPlan
    {
        public TravelPlan()
        {
            PlanDetails = new List<TravelPlanDetail>();
        }

        [Key]
        public int Id { get; set; }
        public string TravelPlanNo { get; set; }
        public DateTime Date { get; set; }
        public string TravelRoute { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public float AdvancePaymentValue { get; set; }

        public virtual CostCenter CostCenter { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<TravelPlanDetail> PlanDetails { get; set; }
    }

    public class TravelPlanDetail
    {
        [Key]
        public int Id { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public float Budget { get; set; }
        public float TotalBudget { get; set; }
        public float PlannedPrice { get; set; }
        public float PlannedTotalBudget { get; set; }
        public float BudgetDifference { get; set; }

        public virtual ExpanseItem ExpanseItem { get; set; }
        public virtual ApplicationUser TravelPlan { get; set; }
    }

    public class CostCenter
    {
        public CostCenter()
        {
            Plans = new List<TravelPlan>();
        }

        [Key]
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public virtual ICollection<TravelPlan> Plans { get; set; }
        public virtual ICollection<PurchaseOrder> Orders { get; set; }
    }

    public class ExpanseItem
    {
        [Key]
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Budget { get; set; }
    }

    public class Document
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string StoredName { get; set; }
        public string FullName { get; set; }

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }

    public class AccessControlList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        public int? ModuleId { get; set; }
        public virtual ICollection<ACLEntry> ACLEntries { get; set; }

        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }
    }

    public class ACLEntry
    {
        [Key]
        public int Id { get; set; }

        public bool CanAccessModule { get; set; }

        public virtual AccessControlList ACL { get; set; }
        public virtual ApplicationUser User { get; set; }

    }

    public class Module
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class DefinedList
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? RelatedDocumentId { get; set; }

        [ForeignKey("RelatedDocumentId")]
        public virtual Document RelatedDocument { get; set; }

    }


}