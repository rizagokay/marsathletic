using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarsAthletic.WebAPI.Models
{
    [DataContract]
    public class ErrorWrapper
    {
        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public string ExceptionType { get; set; }

        [DataMember]
        public string ExceptionSource { get; set; }
    }
}
