using DemoEFBL.TaskOwners;
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
    public class TaskOwnersController : ApiController
    {
        [HttpPost]
        [Route("api/TaskOwners/GetTaskOwners")]
        public HttpResponseMessage GetTaskOwners(TaskOwnersName objTOName)
        {
            MyLogger.GetInstance().Info($"Entering the TaskOwners Controller.  GetTaskOwners Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objTOName)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    TaskOwnersBL objTaskOwnerBL = new TaskOwnersBL();
                    var result = objTaskOwnerBL.GetTaskOwners(objTOName.TaskOwnerName);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the TaskOwners Controller.  GetTaskOwners Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the TaskOwners Controller.  GetTaskOwners Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the TaskOwners Controller.  GetTaskOwners Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };

                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in TaskOwners Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/TaskOwners/AddTaskOwner")]
        public HttpResponseMessage AddTaskOwner(ddTaskOwners taskOwner)
        {
            MyLogger.GetInstance().Info($"Entering the TaskOwners Controller.  AddTaskOwner Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(taskOwner)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    if (taskOwner != null)
                    {
                        TaskOwnersBL objTaskOwnerBL = new TaskOwnersBL();
                        var result = objTaskOwnerBL.AddTaskOwner(taskOwner);
                        if (result)
                        {
                            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                            MyLogger.GetInstance().Info($"Exit the TaskOwners Controller.  AddTaskOwner Method(Success)  ");
                            return new HttpResponseMessage
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                    System.Text.Encoding.UTF8, "application/json")
                            };
                        }
                        MyLogger.GetInstance().Info($"Exit the TaskOwners Controller.  AddTaskOwner Method(Failed)  ");

                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                    MyLogger.GetInstance().Info($"Exit the TaskOwners Controller.  AddTaskOwner Method(Failed)  ");

                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the TaskOwners Controller.  AddTaskOwner Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in TaskOwners Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteTaskOwner(int ID)
        {
            MyLogger.GetInstance().Info($"Entering the TaskOwners Controller.  DeleteTaskOwner Method   Arguments:: {ID}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    if (ID > 0)
                    {
                        TaskOwnersBL objTaskOwnerBL = new TaskOwnersBL();
                        var result = objTaskOwnerBL.DeleteTaskOwner(ID);
                        if (result)
                        {
                            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                            MyLogger.GetInstance().Info($"Exit the TaskOwners Controller.  DeleteTaskOwner Method(Success)  ");
                            return new HttpResponseMessage
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                    System.Text.Encoding.UTF8, "application/json")
                            };
                        }
                        MyLogger.GetInstance().Info($"Exit the TaskOwners Controller.  DeleteTaskOwner Method(Failed)  ");

                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                    MyLogger.GetInstance().Info($"Exit the TaskOwners Controller.  DeleteTaskOwner Method(Failed)  ");

                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the TaskOwners Controller.  DeleteTaskOwner Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in TaskOwners Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}