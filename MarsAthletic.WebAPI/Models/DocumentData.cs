using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAthletic.WebAPI.Models
{
    public class DocumentData
    {
        public string DocumentName { get; set; }
        public string DocumentExtension { get; set; }
        public byte [] ByteData { get; set; }
        public int EmployeeId { get; set; }
        public int LocationId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Cost { get; set; }
        public bool WithProductInfo { get; set; }
        public string Description { get; set; }
    }
}
