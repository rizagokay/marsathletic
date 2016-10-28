using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MarsAhletic.WebUI.Models
{

    #region Database Models

    public class Notification
    {
        [Key]
        public string Id { get; set; }
        public string NotificationMessage { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ReadOn { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

    public class TravelPlan
    {
        public TravelPlan()
        {
            PlanDetails = new List<TravelPlanDetail>();
        }

        [Key]
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string TravelRoute { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public float AdvancePaymentValue { get; set; }
        public string CostCenterId { get; set; }



        [ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenter { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<TravelPlanDetail> PlanDetails { get; set; }
    }

    public class TravelPlanDetail
    {
        [Key]
        public string Id { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string ExpanseItemId { get; set; }
        public float Budget { get; set; }
        public float TotalBudget { get; set; }
        public float PlannedPrice { get; set; }
        public float PlannedTotalBudget { get; set; }
        public float BudgetDifference { get; set; }

        [ForeignKey("ExpanseItemId")]
        public virtual ExpanseItem ExpanseItem { get; set; }
        public virtual TravelPlan TravelPlan { get; set; }
    }

    public class CostCenter
    {
        public CostCenter()
        {
            Plans = new List<TravelPlan>();
        }

        [Key]
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TravelPlan> Plans { get; set; }
    }

    public class ExpanseItem
    {
        [Key]
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Budget { get; set; }
    }

    #endregion
    #region View Models 
    public class ApplicationUserVM
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
        public bool IsManager { get; set; }
        public bool IsAdministrator { get; set; }
        public int AccountType { get; set; }
    }

    public class TravelPlanViewModel
    {
        public string TravelId { get; set; }
        public string NameSurname { get; set; }
        public string ExpanseCenterId { get; set; }
        public DateTime Date { get; set; }
        public string TravelRoute { get; set; }
    }
    #endregion
}