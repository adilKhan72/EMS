using DemoEFBL.Notification;
using DemoEFBO.Notification;
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
    public class NotificationController : ApiController
    {
        [HttpPost]
        [Route("api/Notification/GetNotification")]
        public HttpResponseMessage GetNotification(notificationBO param)
        {
            MyLogger.GetInstance().Info($"Entering the Notification Controller.  GetNotification Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(param)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    int id = -1;
                    int.TryParse(param.UserId, out id);
                    notificationBL objnotificationBL = new notificationBL();
                    var result = objnotificationBL.getnotificationmessage(id);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Notification Controller.  GetNotification Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Notification Controller.  GetNotification Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Notification Controller.  GetNotification Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Notification/ReadNotification")]
        public HttpResponseMessage ReadNotification(notificationReadBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the Notification Controller.  ReadNotification Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    notificationBL objnotificationBL = new notificationBL();
                    var result = objnotificationBL.setReadNotificationBL(obj);
                    if (result != false)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Notification Controller.  ReadNotification Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Notification Controller.  ReadNotification Method(Failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Notification Controller.  ReadNotification Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Notification/SeenNotification")]
        public HttpResponseMessage SeenNotification(notificationReadBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the Notification Controller.  SeenNotification Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    notificationBL objnotificationBL = new notificationBL();
                    var result = objnotificationBL.setSeenNotificationBL(obj);
                    if (result != false)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Notification Controller.  SeenNotification Method(Success)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Notification Controller.  SeenNotification Method(Failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Notification Controller.  SeenNotification Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Notification/GetunReadNotification")]
        public HttpResponseMessage GetunReadNotification(notificationBO param)
        {
            MyLogger.GetInstance().Info($"Entering the Notification Controller.  GetunReadNotification Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(param)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    int id = -1;
                    int.TryParse(param.UserId, out id);
                    notificationBL objnotificationBL = new notificationBL();
                    var result = objnotificationBL.getunReadnotificationmessage(id);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Notification Controller.  GetunReadNotification Method(Success)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Notification Controller.  GetunReadNotification Method(Failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Notification Controller.  GetunReadNotification Method(Failed)   ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                  
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Notification/NotificationCount")]
        public HttpResponseMessage NotificationCount(notificationBO param)
        {
            MyLogger.GetInstance().Info($"Entering the Notification Controller.  NotificationCount Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(param)}");
            try
            {

                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    int id = -1;
                    int.TryParse(param.UserId, out id);

                    NotificationCountBL objBL = new NotificationCountBL();
                    var result = objBL.GetnotificationcountBL(id);
                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                    MyLogger.GetInstance().Info($"Exit the Notification Controller.  NotificationCount Method(Success) ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Notification Controller.  NotificationCount Method(Failed) ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Notification/DeleteNotification")]
        public HttpResponseMessage DeleteNotification(NotificationDeleteBO param)
        {
            MyLogger.GetInstance().Info($"Entering the Notification Controller.  DeleteNotification Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(param)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    notificationBL objnotificationBL = new notificationBL();
                    var result = objnotificationBL.NotificationDelete(param.NotificationID);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Notification Controller.  DeleteNotification Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Notification Controller.  DeleteNotification Method(Failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Notification Controller.  DeleteNotification Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    //int id = -1;
                    //int.TryParse(param.NotificationID, out id);

                  
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

    }
}

