using MarsAhletic.WebUI.Interfaces;
using MarsAhletic.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarsAhletic.WebUI.Helpers
{
    public class SynchroniseData : ISynchroniseData, IDisposable
    {

        private IExternalSystemOperations ops;
        private ApplicationDbContext db;

        public SynchroniseData()
        {
            ops = new ExternalSystemOperations();
            db = new ApplicationDbContext();
        }

        public void Dispose()
        {
            ops = null;
            db.Dispose();

        }

        public void SyncCompanyData()
        {
            var companies = ops.GetCompanies();
            foreach (var item in companies)
            {
                var companyEx = item as CompanyEx;


                var companyInDb = db.Companies.Where(c => c.ExternalId == companyEx.Id).FirstOrDefault();
                if (companyInDb == null)
                {
                    db.Companies.Add(new Company() { Name = companyEx.Name, ExternalId = companyEx.Id, Code = companyEx.Code });
                }
                else
                {
                    //Check Fields
                    if (companyInDb.Name != companyEx.Name)
                    {
                        companyInDb.Name = companyEx.Name;
                        db.Entry(companyInDb).State = System.Data.Entity.EntityState.Modified;
                    }

                    //
                    if (companyInDb.Code != companyEx.Code)
                    {
                        companyInDb.Code = companyEx.Code;
                        db.Entry(companyInDb).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }

            db.SaveChanges();
        }

        public void SyncCurrencies()
        {
            var currencies = ops.GetCurrencies();

            foreach (var item in currencies)
            {
                var currencyEx = item as CurrencyEx;

                var currencyInDb = db.Currencies.Where(c => c.ExternalId == currencyEx.Id).FirstOrDefault();
                if (currencyInDb == null)
                {
                    db.Currencies.Add(new Currency() { ExternalId = currencyEx.Id, Name = currencyEx.Name });
                }
                else
                {
                    if (currencyInDb.Name != currencyEx.Name)
                    {
                        currencyInDb.Name = currencyEx.Name;
                        db.Entry(currencyInDb).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }

            db.SaveChanges();

        }

        public void SyncProducts()
        {
            var products = ops.GetProducts();

            foreach (var item in products)
            {
                var productEx = item as ProductEx;
                var productInDb = db.Products.Include("Company").Where(p => p.ExternalId == productEx.Id.ToString()).FirstOrDefault();
                var currency = db.Currencies.Where(c => c.ExternalId == productEx.CurrencyId).FirstOrDefault();


                if (productInDb == null)
                {
                    Company company = null;
                    if (productEx.CompanyId != null)
                    {
                        company = db.Companies.Where(c => c.ExternalId == productEx.CompanyId).FirstOrDefault();
                    }

                    db.Products.Add(new Product() { ExternalId = productEx.Id.ToString(), Name = productEx.Name, UnitPrice = productEx.UnitPrice, GroupCode = productEx.GroupCode, Code = productEx.Code, Currency = currency != null ? currency : null, Company = company, VATPercentage = productEx.VATPercantage });
                }
                else
                {
                    var changed = false;

                    //Check Name
                    if (productInDb.Name != productEx.Name)
                    {
                        productInDb.Name = productEx.Name;
                        changed = true;
                    }
                    //Check Code
                    if (productInDb.Code != productEx.Code)
                    {
                        productInDb.Code = productEx.Code;
                        changed = true;
                    }

                    //Check Group Code
                    if (productInDb.GroupCode != productEx.GroupCode)
                    {
                        productInDb.GroupCode = productEx.GroupCode;
                        changed = true;
                    }

                    //Check UnitPrice
                    if (productInDb.UnitPrice != productEx.UnitPrice)
                    {
                        productInDb.UnitPrice = productEx.UnitPrice;
                        changed = true;
                    }

                    //Check VATPercentage
                    if (productInDb.VATPercentage != productEx.VATPercantage)
                    {
                        productInDb.VATPercentage = productEx.VATPercantage;
                        changed = true;
                    }

                    //Check Company
                    if (productInDb.Company != null)
                    {
                        if (productInDb.Company.ExternalId != productEx.CompanyId)
                        {
                            var newCompany = db.Companies.Where(c => c.ExternalId == productEx.CompanyId).FirstOrDefault();
                            if (newCompany != null)
                            {
                                productInDb.Company = newCompany;
                                changed = true;
                            }
                        }
                    }
                    else
                    {
                        if (productEx.CompanyId != "" && productEx.CompanyId != null)
                        {
                            var newCompany = db.Companies.Where(c => c.ExternalId == productEx.CompanyId).FirstOrDefault();
                            if (newCompany != null)
                            {
                                productInDb.Company = newCompany;
                                changed = true;
                            }

                        }
                    }




                    if (changed)
                    {
                        db.Entry(productInDb).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }

            db.SaveChanges();
        }

    }
}