using MarsAthletic.WebAPI.Interfaces;
using MarsAthletic.WebAPI.Models;
using MarsAthletic.WebAPI.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MarsAthletic.WebAPI.Controllers
{
    [RoutePrefix("Rest")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OperationsController : ApiController
    {

        private readonly IOperations _operations;

        public OperationsController(IOperations operations)
        {
            _operations = operations;
        }

        [Route("Operations/GetWorkLocations"), HttpGet]
        public HttpResponseMessage GetWorkLocations()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _operations.GetWorkLocations());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name });
            }
        }

        [Route("Operations/GetDepartments"), HttpGet]
        public HttpResponseMessage GetDepartments()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _operations.GetDepartments());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name });
            }
        }

        [Route("Operations/GetEmployees"), HttpGet]
        public HttpResponseMessage GetEmployees()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _operations.GetEmployees());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name });
            }
        }

        [Route("Operations/GetDocumentStatus/{documentId}"), HttpGet]
        public HttpResponseMessage GetStatus(int documentId)
        {

            try
            {
                var ended = new { WorkflowEnded = _operations.DocumentsProcessEnded(documentId) };
                return Request.CreateResponse(HttpStatusCode.OK, ended);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name });
            }
        }

        public HttpResponseMessage Create(DocumentData data)
        {
            try
            {
                var createdDocumentID = _operations.CreateDocument(data);
                return Request.CreateResponse(HttpStatusCode.OK, new { CreatedDocumentID = createdDocumentID });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name });
            }
        }

    }
}
