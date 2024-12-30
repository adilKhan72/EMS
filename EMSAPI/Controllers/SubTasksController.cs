using DemoEFBL.MainTasks;
using DemoEFBO.Tasks;
using DemoEFCommon.ApiResponseResult;
using EMSAPI.ActionFilter;
using EMSAPI.Common;
using EMSAPI.Utility.Nlog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace EMSAPI.Controllers
{
    [Authorize]
    [CheckCustomFilter]
    public class SubTasksController : ApiController
    {
        [HttpPost]
        [Route("api/SubTasks/GetSubTaskList")]
        public HttpResponseMessage GetSubTaskList(SubTasksBO objBO)
        {
            MyLogger.GetInstance().Info($"Entering the SubTasks Controller.  GetSubTaskList Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objBO)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    SubtasksBL objSubtaskBL = new SubtasksBL();
                    var result = objSubtaskBL.getsubtaskname(objBO.projectID, objBO.maintaskID);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the SubTasks Controller.  GetSubTaskList Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the SubTasks Controller.  GetSubTaskList Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the SubTasks Controller.  GetSubTaskList Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in SubTasks Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

    }
}
