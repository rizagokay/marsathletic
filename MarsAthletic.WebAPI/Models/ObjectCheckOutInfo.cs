using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAthletic.WebAPI.Models
{
    public class ObjectCheckOutInfo
    {
        public int ObjectID { get; set; }
        public int ObjectVersion { get; set; }
        public bool IsCheckedOut { get; set; }
    }
}
