using DemoEFBL.Notification;
using DemoEFBO.Notification;
using DemoEFCommon.ApiResponseResult;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EMSAPI.Controllers
{
    public class NotificationCountController : ApiController
    {

        //[HttpPost]
        //[Route("api/Notification/NotificationCount")]
        //public HttpResponseMessage NotificationCount(notificationBO param)
        //{

        //    try
        //    {
        //        int id = -1;
        //        int.TryParse(param.UserId, out id);

        //        NotificationCountBL objBL = new NotificationCountBL();
        //        var result = objBL.GetnotificationcountBL(id);
        //        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
        //        return new HttpResponseMessage
        //        {
        //            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
        //                                System.Text.Encoding.UTF8, "application/json")
        //        };
                
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
        //    }
        //}
    }
}
