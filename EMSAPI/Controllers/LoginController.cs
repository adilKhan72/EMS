using DemoEFBL.Login;
using DemoEFBO.Login;
using EMSAPI.Common;
using EMSAPI.Filters;
using EMSAPI.Utility.Nlog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EMSAPI.Controllers
{
    public class LoginObj
    {
        public string username  { get; set; }
        public string password { get; set; }
    }
    //[EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public class LoginController : ApiController
    {
        [HttpPost]
        //[AllowCrossSiteAttribute]
        [Route("api/Login/LoginAPI")]
        public HttpResponseMessage LoginAPI(LoginObj obj)
        {
            MyLogger.GetInstance().Info($"Entering the Login Controller.  LoginAPI Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}" );
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();
                // var resultss = System.Web.Hosting.HostingEnvironment.MapPath("~/UserImages/"); //Server.MapPath("~/UserImages/");
                LoginBL objBAL = new LoginBL();
                
                var result = objBAL.LoginUser(obj.username, obj.password);
                if (result.Item1 > 0)
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = true, Result = result.Item2,EncryptedRoleType = globalfunctions.Encrypt(result.Item2.Role) };
                    MyLogger.GetInstance().Info($"Exit the Login Controller.  LoginAPI Method(Success)");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else if(result.Item1 == -1)
                {
                    // invalid credentials
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = false, ResponseMsg = "Invalid Credential", Result = (string) null };
                    MyLogger.GetInstance().Info($"Exit the Login Controller.  LoginAPI Method(Success)");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else if(result.Item1 == -3)
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = false, ResponseMsg = "Required param missing.", Result = (string)null };
                    MyLogger.GetInstance().Info($"Exit the Login Controller.  LoginAPI Method(Success)");

                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else if (result.Item1 == -4)
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = false, ResponseMsg = "Due to Unavailability of the Email Address, User is marked as Inactive. Please Contact Super Admin.", Result = (string)null };
                    MyLogger.GetInstance().Info($"Exit the Login Controller.  LoginAPI Method(Success)");

                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else if (result.Item1 == -5)
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = false, ResponseMsg = "User is marked as Inactive. Please Contact Super Admin.", Result = (string)null };
                    MyLogger.GetInstance().Info($"Exit the Login Controller.  LoginAPI Method(Success)");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, isSuccess = false, ResponseMsg = "Error occured !", Result = (string)null };
                    MyLogger.GetInstance().Info($"Exit the Login Controller.  LoginAPI Method(Failed)");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Result");
                
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Login Controller!  "+ex.Message );
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        
    }
}