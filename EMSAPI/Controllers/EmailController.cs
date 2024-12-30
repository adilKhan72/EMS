using DemoEFBL.Email;
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
    public class EmailController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage RunEmailBlocks()
        {
            MyLogger.GetInstance().Info($"Entering the Email Controller.  RunEmailBlocks Method   Arguments:: null");
            try
            {
                EmailSending objEmailSending = new EmailSending();
                objEmailSending.RunEmailFunctionality();
                var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = "Email Sent!" };
                MyLogger.GetInstance().Info($"Exit the Email Controller.  RunEmailBlocks Method(Success) ");
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                        System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Email Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.OK, ex.ToString());
            }
        }
        
    }
}