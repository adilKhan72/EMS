using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DemoEFBL.ChangePassword;
using DemoEFBO.ChangePassword;
using Newtonsoft.Json;
using DemoEFCommon.ApiResponseResult;
using EMSAPI.Utility.Nlog;

namespace EMSAPI.Controllers
{
    [Authorize]
    public class ChangePasswordController : ApiController
    {
        [HttpPost]
        [Route("api/ChangePassword/UpdatePassword")]
        public HttpResponseMessage UpdatePassword(ChangePasswordBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the ChangePassword Controller.  UpdatePassword Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                ChangePasswordBL objChangepass = new ChangePasswordBL();
                var result = objChangepass.ChangePassword(obj.UserId, obj.oldpassword, obj.newpassword);
                if (result != null)
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                    MyLogger.GetInstance().Info($"Exit the ChangePassword Controller.  UpdatePassword Method(Success) ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else 
                { 
                MyLogger.GetInstance().Info($"Exit the ChangePassword Controller.  UpdatePassword Method(Failed)  ");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg); 
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ChangePassword Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }


    }
}
