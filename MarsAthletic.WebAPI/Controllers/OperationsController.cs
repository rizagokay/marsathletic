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
using MFiles.Mfws;

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
                if (ex.GetType() == typeof(MfwsException))
                {
                    var mfEx = ex as MfwsException;

                    return Request.CreateResponse(mfEx.StatusCode, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name, CompleteException = ex.ToString() });

                }

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

                if (ex.GetType() == typeof(MfwsException))
                {
                    var mfEx = ex as MfwsException;

                    return Request.CreateResponse(mfEx.StatusCode, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name, CompleteException= ex.ToString() });

                }

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
                if (ex.GetType() == typeof(MfwsException))
                {
                    var mfEx = ex as MfwsException;

                    return Request.CreateResponse(mfEx.StatusCode, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name, CompleteException = ex.ToString() });

                }

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name });
            }
        }

        [Route("Operations/GetDocumentStatus/{documentId}"), HttpGet]
        public HttpResponseMessage GetStatus(int documentId)
        {

            try
            {
                var ended = new Document { WorkflowStatus = _operations.GetStatusOfDocument(documentId), CreatedDocumentID = documentId };
                return Request.CreateResponse(HttpStatusCode.OK, ended);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(MfwsException))
                {
                    var mfEx = ex as MfwsException;

                    return Request.CreateResponse(mfEx.StatusCode, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name, CompleteException = ex.ToString() });

                }

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name });
            }
        }

        [HttpPost]
        public HttpResponseMessage Create(DocumentData data)
        {
            try
            {
                var createdDocumentID = _operations.CreateDocument(data);
                return Request.CreateResponse(HttpStatusCode.OK, new Document { CreatedDocumentID = createdDocumentID, WorkflowStatus = 0 });
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(MfwsException))
                {
                    var mfEx = ex as MfwsException;

                    return Request.CreateResponse(mfEx.StatusCode, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name, CompleteException = ex.ToString() });

                }

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name }); ;
            }
        }

        [HttpPost]
        public HttpResponseMessage AddFile(AttachmentData data)
        {
            try
            {
                var attachmentInfo = _operations.AddDocument(data);

                if (attachmentInfo.CreationStatus == CreationStatus.Created)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new Attachment { AttachmentID = attachmentInfo.CreatedFileId });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new  { Message = "Ek dosya eklenebilmesi için ana dosyanın check-in edilmiş olması gerekli." });
                }      
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(MfwsException))
                {
                    var mfEx = ex as MfwsException;

                    return Request.CreateResponse(mfEx.StatusCode, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name, CompleteException = ex.ToString() });

                }

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ErrorWrapper() { ErrorMessage = ex.Message.ToString(), ExceptionType = ex.GetType().ToString(), ExceptionSource = ex.TargetSite.Name }); ;
            }
        }
    }
}
