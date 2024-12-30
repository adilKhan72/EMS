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

namespace EMSAPI.Controllers
{
    [Authorize]
    [CheckCustomFilter]
    public class TasksDescriptionController : ApiController
    {
        [HttpPost]
        [Route("api/TasksDescription/GetTaskDescription")]
        public HttpResponseMessage GetTaskDescription (TaskDescriptionBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the TasksDescription Controller.  GetTaskDescription Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    TaskDescriptionBL objTaskDescritionBL = new TaskDescriptionBL();
                    var result = objTaskDescritionBL.GetTaskDescriptions(obj.projectID, obj.maintaskID);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the TasksDescription Controller.  GetTaskDescription Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                    System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                        MyLogger.GetInstance().Info($"Exit the TasksDescription Controller.  GetTaskDescription Method(Failed)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                    System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    
                }
                MyLogger.GetInstance().Info($"Exit the TasksDescription Controller.  GetTaskDescription Method(Failed)  ");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in TasksDescription Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

    }
}
