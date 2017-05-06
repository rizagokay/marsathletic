using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarsAhletic.WebUI.Models
{
    public class PurchaseDetailReportItem
    {
        public int ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductGroupCode { get; set; }
        public string Amount { get; set; }

        public string UnitPrice { get; set; }
        public string TotalValue { get; set; }
        public string BudgetStatus { get; set; }
        public string BudgetType { get; set; }
        public decimal BudgetCost { get; set; }
        public string CompanyName { get; set; }
        public DateTime PurchaseOrderDate { get; set; }
        public string CostCenterName { get; set; }
        public string PurchaseOrderNo { get; set; }
    }
}