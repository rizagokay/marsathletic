using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAhletic.WebUI.Interfaces
{
    public interface IExternalSystemOperations
    {
        IEnumerable<IProduct> GetProducts();
        IEnumerable<ICompany> GetCompanies();
        IEnumerable<ICurrency> GetCurrencies();
        decimal GetCurrencyValue(string CurrencyId);
    }
}
