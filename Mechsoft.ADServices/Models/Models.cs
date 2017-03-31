using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mechsoft.ADServices.Helpers.Models
{
    public class ADDomain
    {
        public string FullName { get; set; }
        public string Name { get; set; }
        public string ForestName { get; set; }
    }

    public class ADOrganizationalUnit
    {
        public string FullName { get; set; }
        public string Name { get; set; }
        public ADDomain Domain { get; set; }
    }

    public class ADUser
    {
        public string SID { get; set; }
        public string ActiveDirectorySID { get; set; }
        public string EMailAddress { get; set; }
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
    }

    public class ADUserGroup
    {
        public string SID { get; set; }
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public string SamAccountName { get; set; }
    }


}
