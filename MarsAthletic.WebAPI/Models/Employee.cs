using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarsAthletic.WebAPI.Models
{
    [DataContract]
    public class Employee
    {
        [DataMember]
        public string InternalID { get; set; }

        [DataMember]
        public int ExternalID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
