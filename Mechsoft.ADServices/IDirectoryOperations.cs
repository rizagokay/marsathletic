using Mechsoft.ADServices.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mechsoft.ADServices.Helpers
{
    public interface IDirectoryOperations
    {
        bool IsAuthenticated(string Username, string Password, string Domain);
        IDictionary<string, string> GetNameAndEmail(string Username, string Password, string Domain, string QueriedUsername);
        ADUser GetUser(string DomainName, string Username);
        IEnumerable<ADOrganizationalUnit> GetOrganizationalUnits(string Domain);
        IEnumerable<ADDomain> GetDomainNames();
        IEnumerable<ADUser> GetUsersInGroup(string Domain, ADUserGroup Group);
        IEnumerable<ADUser> GetUsersInGroup(string Domain, string SAMAccount);
        IEnumerable<ADUserGroup> GetUserGroups(string Domain, string OUDistinguishedName);
        IEnumerable<ADUserGroup> GetUserGroups(string Domain);

    }
}
