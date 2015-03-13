using MarsAthletic.WebAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAthletic.WebAPI.Helpers
{
    public class ConfigHelper : IConfigHelper
    {
        public string GetAccountName()
        {
            var value = "";

            value = ConfigurationManager.AppSettings["MFilesAccountName"];

            return value;

        }

        public string GetPassword()
        {
            var value = "";

            value = ConfigurationManager.AppSettings["MFilesPassword"];

            return value;
        }

        public string GetMFilesUrl()
        {
            var value = "";

            value = ConfigurationManager.AppSettings["MFilesAddress"];

            return value;
        }


        public string GetVaultGuid()
        {
            var value = "";

            value = ConfigurationManager.AppSettings["VaultGuid"];

            return value;
        }
    }
}
