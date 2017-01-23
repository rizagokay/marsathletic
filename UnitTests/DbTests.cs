using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarsAhletic.WebUI.Models;

namespace UnitTests
{
    [TestClass]
    public class DbTests
    {
        [TestMethod]
        public void AllTests()
        {

            using (var db = new ApplicationDbContext())
            {

                if (db.CostCenters.ToList().Count == 0)
                {
                    var costCenter = new CostCenter() { Name = "Ortaköy Merkez", ExternalId = "4554", Address = "Ortaköy" };

                    db.CostCenters.Add(costCenter);
                }


                if (db.TravelPlans.ToList().Count == 0)
                {
                    var travelPlan = new TravelPlan() { CostCenter = db.CostCenters.FirstOrDefault(), User = db.Users.FirstOrDefault(), StartDate = DateTime.Now, Description = "Test", Date = DateTime.Now, EndDate = DateTime.Now };

                    db.TravelPlans.Add(travelPlan);
                }

                var oldCenter = db.CostCenters.FirstOrDefault();

                db.CostCenters.Remove(oldCenter);

                db.SaveChanges();
            }

        }
    }
}
