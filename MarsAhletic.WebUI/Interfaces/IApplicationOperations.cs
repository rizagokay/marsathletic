using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAhletic.WebUI.Interfaces
{
    public interface IApplicationOperations
    {
        void CreateNewConfigKey(string Key, string Value);
        void UpdateValue(string Key, string Value);
        string GetValue(string Key);
        bool ActiveDirectoryIntegrationIsEnabled();
        void DeactivateActiveDirectoryIntegration();
        void Dispose();
    }
}
