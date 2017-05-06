using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAhletic.WebUI.Interfaces
{
    public abstract class IProduct
    {
        public abstract Int32 Id { get; set; }
        public abstract string Name { get; set; }
        public abstract decimal UnitPrice { get; set; }
    }

    public abstract class ICompany
    {
        public abstract string Id { get; set; }
        public abstract string Name { get; set; }
    }

    public abstract class ICurrency
    {
        public abstract string Id { get; set; }
        public abstract string Name { get; set; }
    }
}
