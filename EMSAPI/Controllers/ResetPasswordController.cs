using DemoEFBL.Email;
using DemoEFBL.ResetPassword;
using DemoEFBL.Shared;
using DemoEFBO.ResetPassword;
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
    public class ResetPasswordController : ApiController
    {
        [HttpPost]
        [Route("api/ResetPassword/ResetPasswordValidation")]
        public HttpResponseMessage ResetPasswordValidation(ResetPasswordValidationBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the ResetPassword Controller.  ResetPasswordValidation Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                ResetPasswordBL objResetPassVal = new ResetPasswordBL();
                var result = objResetPassVal.ResetPasswordValidations(obj.globalID);
                if (result == 1)
                {  
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = true, Result = result };
                    MyLogger.GetInstance().Info($"Exit the ResetPassword Controller.  ResetPasswordValidation Method(Success)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else if (result == 2)
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = false, Result = result, Msg = "Reset Password Link Time Expire" };
                    MyLogger.GetInstance().Info($"Exit the ResetPassword Controller.  ResetPasswordValidation Method(Success)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };

                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = false, Result = result, Msg = "User Does Not Exists" };
                    MyLogger.GetInstance().Info($"Exit the ResetPassword Controller.  ResetPasswordValidation Method(Success)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ResetPassword Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/ResetPassword/ResetPassword")]
        public HttpResponseMessage ResetPassword(ResetPasswordID obj)
        {
            MyLogger.GetInstance().Info($"Entering the ResetPassword Controller.  ResetPassword Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                ResetPasswordBL objResetpass = new ResetPasswordBL();
                var result = objResetpass.ResetPassword(obj.LoginID, obj.changePassword);
                if (result != null)
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = true, Result = result };
                    MyLogger.GetInstance().Info($"Exit the ResetPassword Controller.  ResetPassword Method(Success)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = false, Result = result };
                    MyLogger.GetInstance().Info($"Exit the ResetPassword Controller.  ResetPassword Method(Success)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };

                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ResetPassword Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}
