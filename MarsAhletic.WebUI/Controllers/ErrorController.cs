using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MarsAhletic.WebUI.Controllers
{
    public class ErrorController : Controller
    {
        private ILog logger;

        public ErrorController()
        {
            logger = LogManager.GetLogger(typeof(ErrorController));
        }

        public ActionResult PageNotFound()
        {

            if (ViewData.Model != null)
            {
                var model = (HandleErrorInfo)ViewData.Model;
                logger.Error("Sayfa bulunamadı.", model.Exception);
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();

        }

        public ActionResult CustomError()
        {

            if (ViewData.Model != null)
            {
                var model = (HandleErrorInfo)ViewData.Model;
                logger.Error("Uygulamada hata tespit edildi.", model.Exception);

                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return View();
            }

            else
            {
                throw new HttpException(404, "Sayfa Bulunamadı.");
            }

        }
    }
}