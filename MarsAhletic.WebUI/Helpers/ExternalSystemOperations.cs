using MarsAhletic.WebUI.Interfaces;
using MarsAhletic.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MarsAhletic.WebUI.Helpers
{
    public class ExternalSystemOperations : IExternalSystemOperations
    {
        public IEnumerable<ICompany> GetCompanies()
        {
            //Mock Data
            return new List<CompanyEx>()
            {
                new CompanyEx() {Id = "QSQWEQWE123123", Name="Mechsoft Bilişim Sistemleri", Code="XQHY" },
                new CompanyEx() {Id = "QAWSQWE123123", Name="Ventek Vana Sistemleri", Code="VQHY" }
            };

            //Todo: Call Live

        }

        public IEnumerable<ICurrency> GetCurrencies()
        {
            //Mock Data
            return new List<CurrencyEx>() {
                new CurrencyEx() { Id = "1231313", Name = "TRY" },
                new CurrencyEx() { Id = "12321ASD", Name = "EUR" }
            };

            //Todo: Call Live
        }

        public decimal GetCurrencyValue(string CurrencyId)
        {
            //Mock Data
            if (CurrencyId == "1231313")
            {
                return 1;
            }
            else if (CurrencyId == "12321ASD")
            {
                return 3.65M;
            }
            else
            {
                return 1;
            }

            //TODO: Call Live
            throw new NotImplementedException();
        }

        public IEnumerable<IProduct> GetProducts()
        {
            priusWebService.PriusWebService srv = new priusWebService.PriusWebService();
            string token = "FA6191FA-C702-4005-9BC0-77716A9104C4";
                                                                  
            DataTable dt = srv.getItemList(Convert.ToDateTime("2010-01-01"), "", token);

           return dt.AsEnumerable().Select(x => new ProductEx() {
               Id = x.Field<Int32>("LOGICALREF"),
               Code = x.Field<string>("CODE"),
               Name = x.Field<string>("NAME"),
               Specode = x.Field<string>("SPECODE"),
               CyphCode = x.Field<string>("CYPHCODE"),
               GroupCode = x.Field<string>("STGRPCODE"),
               ProducerCode = x.Field<string>("PRODUCERCODE")
               
           }).ToList();
            
            //return new List<ProductEx>() {
            //    new ProductEx() { UnitPrice = 5500.00M, Code = "54213", GroupCode = "213131", Id = "SADASWQ", Name = "Head & Shoulders Şampuan", CurrencyId = "1231313"},
            //    new ProductEx() { UnitPrice = 5500.00M, Code = "54214", GroupCode = "213131", Id = "SADASWE", Name = "Palmolive Şampuan", CurrencyId = "1231313"},
            //    new ProductEx() { UnitPrice = 5500.00M, Code = "54222", GroupCode = "213131", Id = "SADFSWQ", Name = "Zowie ZA12 Mouse", CurrencyId = "1231313", CompanyId = "QSQWEQWE123123"},
            //    new ProductEx() { VATPercantage =18, Code = "233321", GroupCode = "213131", Id = "ZAASWQ", Name = "Tükenmez Kalem", CurrencyId = "1231313"}
            //};

            //TODO: Call Live
        }
    }
}