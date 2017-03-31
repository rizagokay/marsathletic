using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;
using Mechsoft.ADServices.Helpers.Models;
using System.DirectoryServices.AccountManagement;

namespace Mechsoft.ADServices.Helpers
{
    public class DirectoryOperations : IDirectoryOperations
    {
        private Logger logger;
        public DirectoryOperations()
        {
            logger = new Logger();
        }
        public IEnumerable<ADDomain> GetDomainNames()
        {
            try
            {
                var domainList = new List<ADDomain>();
                var currentForest = Forest.GetCurrentForest();

                //Current Forest Domains
                foreach (Domain objDomain in currentForest.Domains)
                {
                    domainList.Add(new ADDomain() { FullName = objDomain.Name, Name = GetNetbiosDomainName(objDomain.Name), ForestName = objDomain.Forest.Name });
                }

                //Get Trusted Forests
                TrustRelationshipInformationCollection trustedForests = currentForest.GetAllTrustRelationships();

                //Get Domains Of Trusted Forests
                foreach (TrustRelationshipInformation item in trustedForests)
                {
                    if (item.TrustType == TrustType.Forest)
                    {
                        var dc = new DirectoryContext(DirectoryContextType.Forest, item.TargetName);
                        var trustedForest = Forest.GetForest(dc);

                        foreach (Domain trustedDomain in trustedForest.Domains)
                        {
                            domainList.Add(new ADDomain() { FullName = trustedDomain.Name, Name = GetNetbiosDomainName(trustedDomain.Name), ForestName = trustedDomain.Forest.Name });
                        }
                    }
                }


                return domainList;
            }
            catch (Exception Ex)
            {
                if (logger != null)
                {
                    logger.Write(Ex);
                }
                throw;
            }
        }
        public IEnumerable<ADOrganizationalUnit> GetOrganizationalUnits(string Domain)
        {
            try
            {
                var orgUnits = new List<ADOrganizationalUnit>();


                using (var dEntry = new DirectoryEntry("LDAP://" + Domain))
                {
                    DirectorySearcher ouSearch = new DirectorySearcher(dEntry,
                                                 "(objectClass=organizationalUnit)",
                                                 null, SearchScope.Subtree);

                    foreach (SearchResult resEnt in ouSearch.FindAll())
                    {
                        string OUName = resEnt.GetDirectoryEntry().Properties["Name"].Value.ToString();
                        var OUFullName = resEnt.GetDirectoryEntry().Properties["distinguishedName"].Value.ToString();
                        orgUnits.Add(new ADOrganizationalUnit() { Name = OUName, FullName = OUFullName, Domain = new ADDomain() { FullName = Domain, Name = GetNetbiosDomainName(Domain) } });
                    }

                }

                return orgUnits;
            }
            catch (Exception Ex)
            {
                if (logger != null)
                {
                    logger.Write(Ex);
                }

                throw;
            }
        }
        public ADUser GetUser(string DomainName, string Username)
        {

            try
            {
                using (var domainContext = new PrincipalContext(ContextType.Domain, DomainName))
                {
                    using (var foundUser = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, Username))
                    {

                        if (foundUser != null)
                        {
                            var adUsr = new ADUser() { ActiveDirectorySID = foundUser.Sid.AccountDomainSid.ToString(), EMailAddress = foundUser.EmailAddress, DistinguishedName = foundUser.DistinguishedName, DisplayName = foundUser.DisplayName, SID = foundUser.Sid.Value };

                            return adUsr;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                if (logger != null)
                {
                    logger.Write(Ex);
                }
                throw;
            }
        }
        public IEnumerable<ADUserGroup> GetUserGroups(string Domain)
        {
            try
            {
                var allItems = new List<ADUserGroup>();
                var yourOU = new PrincipalContext(ContextType.Domain, Domain);
                var findAllGroups = new GroupPrincipal(yourOU, "*");
                var ps = new PrincipalSearcher(findAllGroups);
                foreach (var group in ps.FindAll())
                {
                    allItems.Add(new ADUserGroup() { DisplayName = group.Name, DistinguishedName = group.DistinguishedName, SID = group.Sid.Value, SamAccountName = group.SamAccountName });
                }
                return allItems;
            }
            catch (Exception Ex)
            {
                if (logger != null)
                {
                    logger.Write(Ex);
                }
                throw;
            }
        }
        public IEnumerable<ADUserGroup> GetUserGroups(string Domain, string OuDistinguishedName)
        {
            try
            {
                var allItems = new List<ADUserGroup>();
                var yourOU = new PrincipalContext(ContextType.Domain, Domain, OuDistinguishedName);
                var findAllGroups = new GroupPrincipal(yourOU, "*");
                var ps = new PrincipalSearcher(findAllGroups);

                foreach (var group in ps.FindAll())
                {
                    allItems.Add(new ADUserGroup() { DisplayName = group.Name, DistinguishedName = group.DistinguishedName, SID = group.Sid.Value, SamAccountName = group.SamAccountName });
                }

                return allItems;
            }
            catch (Exception Ex)
            {
                if (logger != null)
                {
                    logger.Write(Ex);
                }
                throw;
            }
        }
        public IEnumerable<ADUser> GetUsersInGroup(string Domain, ADUserGroup Group)
        {
            try
            {
                var allItems = new List<ADUser>();
                var context = new PrincipalContext(ContextType.Domain, Domain);
                var foundGroup = GroupPrincipal.FindByIdentity(context, Group.SamAccountName);

                foreach (var item in foundGroup.Members)
                {
                    var foundUser = UserPrincipal.FindByIdentity(context, item.SamAccountName);
                    allItems.Add(new ADUser() { ActiveDirectorySID = foundUser.Sid.AccountDomainSid.ToString(), EMailAddress = foundUser.EmailAddress, DistinguishedName = foundUser.DistinguishedName, DisplayName = foundUser.DisplayName, SID = foundUser.Sid.Value });
                }

                return allItems;
            }
            catch (Exception Ex)
            {
                if (logger != null)
                {
                    logger.Write(Ex);
                }
                throw;
            }
        }
        public IEnumerable<ADUser> GetUsersInGroup(string Domain, string SAMAccountName)
        {
            try
            {
                var allItems = new List<ADUser>();
                var context = new PrincipalContext(ContextType.Domain, Domain);
                var foundGroup = GroupPrincipal.FindByIdentity(context, SAMAccountName);

                foreach (var item in foundGroup.Members)
                {
                    var foundUser = UserPrincipal.FindByIdentity(context, item.SamAccountName);
                    allItems.Add(new ADUser() { ActiveDirectorySID = foundUser.Sid.AccountDomainSid.ToString(), EMailAddress = foundUser.EmailAddress, DistinguishedName = foundUser.DistinguishedName, DisplayName = foundUser.DisplayName, SID = foundUser.Sid.Value });
                }

                return allItems;
            }
            catch (Exception Ex)
            {
                if (logger != null)
                {
                    logger.Write(Ex);
                }
                throw;
            }
        }
        public bool IsAuthenticated(string Username, string Password, string Domain)
        {
            try
            {

                var path = "LDAP://" + Domain;

                using (DirectoryEntry adsEntry = new DirectoryEntry(path, Username, Password))
                {
                    using (DirectorySearcher adsSearcher = new DirectorySearcher(adsEntry))
                    {
                        adsSearcher.Filter = "(sAMAccountName=" + Username + ")";

                        try
                        {
                            SearchResult adsSearchResult = adsSearcher.FindOne();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                        finally
                        {
                            adsEntry.Close();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                if (logger != null)
                    logger.Write(Ex);
                throw;
            }
        }

        public IDictionary<string, string> GetNameAndEmail(string Username, string Password, string Domain, string QueriedUsername)
        {
            var dict = new Dictionary<string, string>();

            using (var pc = new PrincipalContext(ContextType.Domain, Domain, Username, Password))
            {
                var user = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, QueriedUsername);

                if (user != null)
                {
                    dict.Add(user.Name, user.EmailAddress);
                }

            }

            return dict;
        }

        private string GetNetbiosDomainName(string dnsDomainName)
        {
            string netbiosDomainName = string.Empty;

            DirectoryEntry rootDSE = new DirectoryEntry(string.Format("LDAP://{0}/RootDSE", dnsDomainName));

            string configurationNamingContext = rootDSE.Properties["configurationNamingContext"][0].ToString();

            DirectoryEntry searchRoot = new DirectoryEntry("LDAP://cn=Partitions," + configurationNamingContext);

            DirectorySearcher searcher = new DirectorySearcher(searchRoot);
            searcher.SearchScope = SearchScope.OneLevel;
            searcher.PropertiesToLoad.Add("netbiosname");
            searcher.Filter = string.Format("(&(objectcategory=Crossref)(dnsRoot={0})(netBIOSName=*))", dnsDomainName);

            SearchResult result = searcher.FindOne();

            if (result != null)
            {
                netbiosDomainName = result.Properties["netbiosname"][0].ToString();
            }

            return netbiosDomainName;
        }
    }
}
