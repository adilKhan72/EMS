using DemoEFBL.ForgetPassword;
using DemoEFBO.ForgetPassword;
using EMSAPI.Utility.Nlog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EMSAPI.Controllers
{
    public class ForgetPasswordController : ApiController
    {
        [HttpPost]
        [Route("api/ForgetPassword/forgetPassword")]
        public HttpResponseMessage forgetPassword(UserEmail obj)
        {
            MyLogger.GetInstance().Info($"Entering the ForgetPassword Controller.  forgetPassword Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                ForgetPasswordBL objForgetpass = new ForgetPasswordBL();
                var result = objForgetpass.ForgetPasswordLogic(obj.EmailAddress);
                if (result != null)
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = true, Result = result };
                    MyLogger.GetInstance().Info($"Exit the ForgetPassword Controller.  forgetPassword Method(Success)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = false, ResponseMsg = "Email does not exist ", Result = (string)null };
                    MyLogger.GetInstance().Info($"Exit the ForgetPassword Controller.  forgetPassword Method(Success)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                    //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ForgetPassword Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}
