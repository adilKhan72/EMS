using DemoEFBL.CommonLists;
using DemoEFBL.Tasks;
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
    public class gettaskclass
    {
        public string TaskName { get; set; }
        public int ProjectID { get; set; }
        public int MaintaskID { get; set; }
    }
    public class DeleteTaskId
    {
        public int ID { get; set; }
    }
    [Authorize]
    [CheckCustomFilter]
    public class TasksController : ApiController
    {
        [HttpPost]
        [Route("api/Tasks/GetTasks")]
        public HttpResponseMessage GetTasks(gettaskclass obj)
        {
            MyLogger.GetInstance().Info($"Entering the Tasks Controller.  GetTasks Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    TasksBL objTasksBL = new TasksBL();
                    var result = objTasksBL.GetTasks(obj.TaskName, obj.ProjectID, obj.MaintaskID);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Tasks Controller.  GetTasks Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the Tasks Controller.  GetTasks Method(Failed)  ");

                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the Tasks Controller.  GetTasks Method(Failed)  ");
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
        [Route("api/Tasks/AddTask")]
        public HttpResponseMessage AddTask(TasksBO task)
        {
            MyLogger.GetInstance().Info($"Entering the Tasks Controller.  AddTask Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(task)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    if (task != null)
                    {
                        TasksBL objTasksBL = new TasksBL();
                        var result = objTasksBL.AddTask(task);
                        if (result)
                        {
                            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                            MyLogger.GetInstance().Info($"Exit the Tasks Controller.  AddTask Method(Success)  ");
                            return new HttpResponseMessage
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                    System.Text.Encoding.UTF8, "application/json")
                            };
                        }
                        MyLogger.GetInstance().Info($"Exit the Tasks Controller.  AddTask Method(Failed)  ");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                    MyLogger.GetInstance().Info($"Exit the Tasks Controller.  AddTask Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Tasks Controller.  AddTask Method(Failed)  ");
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

        [HttpGet]
        public HttpResponseMessage GetTaskTypes()
        {
            MyLogger.GetInstance().Info($"Entering the Tasks Controller.  GetTaskTypes Method   Arguments:: null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    var result = TaskListBL.GetTaskTypes();
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Tasks Controller.  GetTaskTypes Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the Tasks Controller.  GetTaskTypes Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Tasks Controller.  GetTaskTypes Method(Failed)  ");
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
        [Route("api/Tasks/DeleteTask")]
        public HttpResponseMessage DeleteTask(DeleteTaskId obj)
        {
            MyLogger.GetInstance().Info($"Entering the Tasks Controller.  DeleteTask Method   Arguments:: {obj.ID}");

            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    if (obj.ID > 0)
                    {
                        TasksBL objTasksBL = new TasksBL();
                        var result = objTasksBL.DeleteTask(obj.ID);
                        if (result)
                        {
                            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                            MyLogger.GetInstance().Info($"Exit the Tasks Controller.  DeleteTask Method(Success)  ");
                            return new HttpResponseMessage
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                    System.Text.Encoding.UTF8, "application/json")
                            };
                        }
                        MyLogger.GetInstance().Info($"Exit the Tasks Controller.  DeleteTask Method(Failed)  ");

                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                    MyLogger.GetInstance().Info($"Exit the Tasks Controller.  DeleteTask Method(Failed)  ");

                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Tasks Controller.  DeleteTask Method(Failed)  ");
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