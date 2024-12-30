using DemoEFBL.MainTasks;
using DemoEFBO.Tasks;
using DemoEFCommon.ApiResponseResult;
using EMSAPI.ActionFilter;
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
using System.Web.Http.Results;

namespace EMSAPI.Controllers
{
    [Authorize]
    [CheckCustomFilter]
    public class MainTaskController : ApiController
    {
        [HttpPost]
        [Route("api/MainTask/GetMainTasks")]
        public HttpResponseMessage GetMainTasks()
        {
            MyLogger.GetInstance().Info($"Entering the MainTask Controller.  GetMainTasks Method   Arguments:: null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    MainTasksBL objMainTaskBL = new MainTasksBL();
                    var result = objMainTaskBL.GetMainTask();
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTasks Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTasks Method(Failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTasks Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in MainTask Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/MainTask/GetFilterMainTaskList")]
        public HttpResponseMessage GetFilterMainTaskList(MainTaskParms obj)
        {
            MyLogger.GetInstance().Info($"Entering the MainTask Controller.  GetFilterMainTaskList Method   Arguments:: null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    MainTasksBL objMainTaskBL = new MainTasksBL();
                    var result = objMainTaskBL.GetFilterMainTaskList(obj.MainTaskName, obj.IsFilter);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetFilterMainTaskList Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetFilterMainTaskList Method(Failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetFilterMainTaskList Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in MainTask Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/MainTask/GetMainTaskMappedList")]
        public HttpResponseMessage GetMainTaskMappedList(MainTaskMappedParms obj)
        {
            MyLogger.GetInstance().Info($"Entering the MainTask Controller.  GetMainTaskMappedList Method   Arguments:: null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    MainTasksBL objMainTaskBL = new MainTasksBL();
                    var result = objMainTaskBL.GetMainTaskMappedListBL(obj.ProjectID, obj.UserID);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskMappedList Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskMappedList Method(Failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskMappedList Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in MainTask Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/MainTask/GetMainTaskMappedListByDepartmentID")]
        public HttpResponseMessage GetMainTaskMappedListByDepartmentID(MainTaskMappedParms obj)
        {
            MyLogger.GetInstance().Info($"Entering the MainTask Controller.  GetMainTaskMappedList Method   Arguments:: null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    MainTasksBL objMainTaskBL = new MainTasksBL();
                    var result = objMainTaskBL.GetMainTaskMappedListBLbyDepartment(obj.ProjectID, obj.UserID);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskMappedList Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskMappedList Method(Failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskMappedList Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in MainTask Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/MainTask/GetMainTaskLazyLoading")]
        public HttpResponseMessage GetMainTaskLazyLoading(LazyLoadingMainTaskBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the MainTask Controller.  GetMainTaskLazyLoading Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    MainTasksBL objMainTaskBL = new MainTasksBL();
                    var result = objMainTaskBL.GetMainTaskLazyLoadingBL(obj.page, obj.recsPerPage);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskLazyLoading Method(Success) ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskLazyLoading Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskLazyLoading Method(Failed) ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in MainTask Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        //[HttpPost]
        //[Route("api/MainTask/GetMainTaskList")]
        //public HttpResponseMessage GetMainTaskList([FromBody]string TaskName)
        //{
        //    MyLogger.GetInstance().Info($"Entering the MainTask Controller.  GetMainTaskList Method   Arguments:: {TaskName}");
        //    try
        //    {
        //        MainTasksBL objMainTaskBL = new MainTasksBL();
        //        var result = objMainTaskBL.GetMainTaskList(TaskName);
        //        if(result != null)
        //        {
        //            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
        //            MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskList Method(Success)  ");
        //            return new HttpResponseMessage
        //            {
        //                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
        //                                    System.Text.Encoding.UTF8, "application/json")
        //            };
        //        }
        //        else
        //        {
        //            var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
        //            MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskList Method(Failed)   ");
        //            return ResponseMsg;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Error("Exception in MainTask Controller!  " + ex.Message);
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
        //    }
        //}
        [HttpPost]
        [Route("api/MainTask/SearchMainTasks")]
        public HttpResponseMessage SearchMainTasks(SearchMainTaskBO OBJ)
        {
            MyLogger.GetInstance().Info($"Entering the MainTask Controller.  SearchMainTasks Method   Arguments:: null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    MainTasksBL objMainTaskBL = new MainTasksBL();
                    var result = objMainTaskBL.SearchMainTaskBL(OBJ.MainTaskName);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  SearchMainTasks Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  SearchMainTasks Method(Failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the MainTask Controller.  SearchMainTasks Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };

                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in MainTask Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/MainTask/InsertMainTask")]
        public HttpResponseMessage InsertMainTask(MainTaskBO task)
        {
            MyLogger.GetInstance().Info($"Entering the MainTask Controller.  InsertMainTask Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(task)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    if (task != null)
                    {
                        MainTasksBL objMainTaskBL = new MainTasksBL();
                        var result = objMainTaskBL.InsertMainTask(task);
                        if (result)
                        {
                            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                            MyLogger.GetInstance().Info($"Exit the MainTask Controller.  InsertMainTask Method(Success)  ");
                            return new HttpResponseMessage
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                    System.Text.Encoding.UTF8, "application/json")
                            };
                        }
                        else
                        {
                            var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                            MyLogger.GetInstance().Info($"Exit the Designation Controller.  InsertMainTask Method(Failed)   ");
                            return ResponseMsg;
                        }
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  InsertMainTask Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the MainTask Controller.  InsertMainTask Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in MainTask Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/MainTask/GetClientLazyLoading")]
        public HttpResponseMessage GetClientLazyLoading(LazyLoadingClientBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the MainTask Controller.  GetMainTaskLazyLoading Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    MainTasksBL objMainTaskBL = new MainTasksBL();
                    var result = objMainTaskBL.GetClientLazyLoadingBL(obj.page, obj.recsPerPage);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskLazyLoading Method(Success) ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskLazyLoading Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskLazyLoading Method(Failed) ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in MainTask Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/MainTask/SearchClient")]
        public HttpResponseMessage SearchClient(SearchMainTaskBO OBJ)
        {
            MyLogger.GetInstance().Info($"Entering the MainTask Controller.  SearchClient Method   Arguments:: null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    MainTasksBL objMainTaskBL = new MainTasksBL();
                    var result = objMainTaskBL.SearchClientBL(OBJ.MainTaskName);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the SearchClient Controller.  SearchMainTasks Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  SearchMainTasks Method(Failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the SearchClient Controller.  SearchMainTasks Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in MainTask Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/MainTask/CheckMainTask")]
        public HttpResponseMessage CheckMainTask(CheckMainTaskMappedParms obj)
        {
            MyLogger.GetInstance().Info($"Entering the MainTask Controller.  GetMainTaskMappedList Method   Arguments:: null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    MainTasksBL objMainTaskBL = new MainTasksBL();
                    var result = objMainTaskBL.CheckMainTask(obj);
                    if (result)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskMappedList Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.BadRequest, Result = result };
                        MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskMappedList Method(Failed)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                               System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the MainTask Controller.  GetMainTaskMappedList Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in MainTask Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}