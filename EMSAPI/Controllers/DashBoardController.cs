using DemoEFBL.Dashboard;
using DemoEFBO.Dashboard;
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
    public class DashBoardController : ApiController
    {
        [HttpPost]
        [Route("api/Dashboard/GetDashBoardStats")]
        public HttpResponseMessage GetDashBoardStats(GetDashBoardStatsUpdateBO objGetState)
        {
            MyLogger.GetInstance().Info($"Entering the DashBoard Controller.  GetDashBoardStats Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objGetState)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    DashboardStatsBL objDbSBL = new DashboardStatsBL();
                    var result = objDbSBL.GetDashBoardStatsUpdateBL(objGetState);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStats Method(Success)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStats Method(failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStats Method(failed)   ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                
                //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in DashBoard Controller!  GetDashBoardStats Method  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        //public HttpResponseMessage GetDashBoardStats(GetDashBoardStatsBO objGetState) //Old Controller
        //{
        //    try
        //    {
        //        DashboardStatsBL objDbSBL = new DashboardStatsBL();
        //        var result = objDbSBL.GetDashBoardStatsBL(objGetState);
        //        if (result != null)
        //        {
        //            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
        //            return new HttpResponseMessage
        //            {
        //                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
        //                                    System.Text.Encoding.UTF8, "application/json")
        //            };
        //        }
        //        else
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
        //    }
        //}

        [HttpPost]
        [Route("api/Dashboard/GetDashBoardStatsForStaff")]
        public HttpResponseMessage GetDashBoardStatsForStaff(GetDashBoardStatsForStaffBO objGetState)
        {
            MyLogger.GetInstance().Info($"Entering the DashBoard Controller.  GetDashBoardStatsForStaff Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objGetState)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    DashboardStatsBL objDbSBL = new DashboardStatsBL();
                    var result = objDbSBL.GetDashBoardStatsForStaffBL(objGetState);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStatsForStaff Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStatsForStaff Method(failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStatsForStaff Method(failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
               
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in DashBoard Controller! GetDashBoardStatsForStaff Method  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Dashboard/GetDashBoardStatsForStaffSearch")]
        public HttpResponseMessage GetDashBoardStatsForStaffSearch(GetDashBoardStatsForStaffSearchBO objGetState)
        {
            MyLogger.GetInstance().Info($"Entering the DashBoard Controller.  GetDashBoardStatsForStaffSearch Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objGetState)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    DashboardStatsBL objDbSBL = new DashboardStatsBL();
                    var result = objDbSBL.GetDashBoardStatsForStaffSearchBL(objGetState);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStatsForStaffSearch Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStatsForStaffSearch Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStatsForStaffSearch Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in DashBoard Controller! GetDashBoardStatsForStaffSearch Method  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Dashboard/GetSpentTaskTime")]
        public HttpResponseMessage GetSpentTaskTime(TimeSpenBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the DashBoard Controller.  GetSpentTaskTime Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {

                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    TimeSpentBL objTSB = new TimeSpentBL();
                    var result = objTSB.GetTimeSpentUpdate(obj);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetSpentTaskTime Method(Success)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetSpentTaskTime Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetSpentTaskTime Method(Failed)   ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in DashBoard Controller! GetSpentTaskTime Method  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Dashboard/GetSubTaskTime")]
        public HttpResponseMessage GetSubTaskTime(TimeSubtaskBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the DashBoard Controller.  GetSubTaskTime Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    TimeSpentBL objTSB = new TimeSpentBL();
                    var result = objTSB.GetSubTaskBL(obj);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetSubTaskTime Method(Success)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetSubTaskTime Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetSubTaskTime Method(Failed)   ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in DashBoard Controller! GetSubTaskTime Method  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }


        [HttpPost]
        [Route("api/Dashboard/GetSubTaskTimeOwner")]
        public HttpResponseMessage GetSubTaskTimeOwner(TimeSubtaskBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the DashBoard Controller.  GetSubTaskTimeOwner Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    TimeSpentBL objTSB = new TimeSpentBL();
                    var result = objTSB.GetSubTaskTimeOwnerBL(obj);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetSubTaskTimeOwner Method(Success)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetSubTaskTimeOwner Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetSubTaskTimeOwner Method(Failed)   ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in DashBoard Controller! GetSubTaskTimeOwner Method  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        //public HttpResponseMessage GetSpentTaskTime(TimeSpenBO obj)
        //{
        //    try
        //    {
        //        TimeSpentBL objTSB = new TimeSpentBL();
        //        var result = objTSB.GetTimeSpent(obj.ProjectId);
        //        if (result != null)
        //        {
        //            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
        //            return new HttpResponseMessage
        //            {
        //                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
        //                                    System.Text.Encoding.UTF8, "application/json")
        //            };
        //        }
        //        else
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
        //    }
        //}

        [HttpPost]
        [Route("api/Dashboard/GetDashBoardStats_byProjectID")]
        public HttpResponseMessage GetDashBoardStats_byProjectID(GetDashBoardStatsUpdateBO objGetState)
        {
            MyLogger.GetInstance().Info($"Entering the DashBoard Controller.  GetDashBoardStats_byProjectID Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objGetState)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    DashboardStatsBL objDbSBL = new DashboardStatsBL();
                    var result = objDbSBL.GetDashBoardStatsUpdateBL_ByProjectID(objGetState);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStats_byProjectID Method(Success)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStats_byProjectID Method(failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the DashBoard Controller.  GetDashBoardStats_byProjectID Method(failed)   ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
                //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in DashBoard Controller!  GetDashBoardStats_byProjectID Method  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

    }
}
