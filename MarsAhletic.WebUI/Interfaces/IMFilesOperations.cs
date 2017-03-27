using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAhletic.WebUI.Interfaces
{
    public interface IMFilesOperations
    {
        object CreateObject();
        object GetObject(string objectId);
        void AddCommentForObject(string objectId);
    }
}
