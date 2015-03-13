using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAthletic.WebAPI.Interfaces
{
    public interface IConfigHelper
    {
        string GetAccountName();
        string GetPassword();
        string GetMFilesUrl();
        string GetVaultGuid();
    }
}
