using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAhletic.WebUI.Interfaces
{
    public interface ISynchroniseData
    {
        void SyncCompanyData();
        void SyncProducts();
        void SyncCurrencies();
    }
}
