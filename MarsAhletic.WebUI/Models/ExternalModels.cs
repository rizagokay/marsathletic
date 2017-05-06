using MarsAhletic.WebUI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarsAhletic.WebUI.Models
{
    public class CompanyEx : ICompany
    {
        public override string Id { get; set; }
        public override string Name { get; set; }
        public string Code { get; set; }
    }

    public class CurrencyEx : ICurrency
    {
        public override string Id
        {
            get; set;
        }
        public override string Name
        {
            get; set;
        }

    }

    public class ProductEx : IProduct
    {
        public override Int32 Id
        {
            get; set;
        }
        public override string Name
        {
            get; set;
        }
        public override decimal UnitPrice
        {
            get; set;
        }

        public string GroupCode { get; set; }
        public string Code { get; set; }
        public string CompanyId { get; set; }
        public string CurrencyId { get; set; }
        public long VATPercantage { get; set; }
        public decimal CurrencyValue { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }

        public string ProducerCode{ get; set; }
        public string Specode { get; set; }
        public string CyphCode { get; set; }
        public CurrencyEx Currency { get; set; }
    }
}