using MarsAhletic.WebUI.Interfaces;
using MarsAhletic.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarsAhletic.WebUI.Helpers
{
    public class ApplicationOperations : IApplicationOperations, IDisposable
    {

        private IEncryptionOperations encOps;
        private ApplicationDbContext db;

        public ApplicationOperations()
        {
            encOps = new EncryptionOperations();
            db = new ApplicationDbContext();
        }

        public void CreateNewConfigKey(string Key, string Value)
        {
            var encodedKey = encOps.EncodeString(Key);
            var encodedValue = encOps.EncodeString(Value);

            var config = new Configuration() { Key = encodedKey, Value = encodedValue };

            db.Configs.Add(config);
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
            encOps = null;
        }

        public string GetValue(string Key)
        {
            var value = "";
            var encodedKey = encOps.EncodeString(Key);

            var config = db.Configs.Where(x => x.Key == encodedKey).FirstOrDefault();

            if (config != null)
            {
                var encodedValue = config.Value;
                var decodedValue = encOps.DecodeString(encodedValue);

                value = decodedValue;
            }


            return value;
        }

        public void DeactivateActiveDirectoryIntegration()
        {
            var encDomainAccName = encOps.EncodeString("DomainAccountName");
            var accountNameConfigs = db.Configs.Where(p => p.Key == encDomainAccName).ToList();

            foreach (var item in accountNameConfigs)
            {
                db.Configs.Remove(item);
            }

            var encDomainAName = encOps.EncodeString("DomainName");
            var domainNameConfigs = db.Configs.Where(p => p.Key == encDomainAName).ToList();

            foreach (var item in domainNameConfigs)
            {
                db.Configs.Remove(item);
            }

            var encPassword = encOps.EncodeString("DomainPassword");
            var passwordConfigs = db.Configs.Where(p => p.Key == encPassword).ToList();

            foreach (var item in passwordConfigs)
            {
                db.Configs.Remove(item);
            }

            db.SaveChanges();
        }

        public bool ActiveDirectoryIntegrationIsEnabled()
        {
            var result = (GetValue("DomainAccountName") != "" && GetValue("DomainName") != "" && GetValue("DomainPassword") != "");
            return result;
        }

        public void UpdateValue(string Key, string Value)
        {
            var encodedKey = encOps.EncodeString(Key);
            var encodedValue = encOps.EncodeString(Value);

            var config = db.Configs.Where(x => x.Key == encodedKey).FirstOrDefault();

            if (config != null)
            {
                config.Value = encodedValue;
                db.Entry(config).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}