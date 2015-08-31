using MarsAthletic.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MarsAthletic.WebAPI.Interfaces
{
    public interface IOperations
    {

        IEnumerable<Department> GetDepartments();
        IEnumerable<WorkLocation> GetWorkLocations();
        IEnumerable<Employee> GetEmployees();

        int GetStatusOfDocument(int id);
        int CreateDocument(DocumentData data);
        AttachmentInfo AddDocument(AttachmentData data);
        Task<AttachmentInfo> AddMultipleDocumentsAtOnce(HttpFileCollection Files, int FileID);
    }
}
