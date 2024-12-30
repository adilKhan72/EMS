using DemoEFBL.Department;
using DemoEFBL.DepartmentMapping;
using DemoEFBL.MainTasks;
using DemoEFBL.ResourceMapping;
using DemoEFBO.ResourceMapping;
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
    public class DepartmentMappingController : ApiController
    {
        [HttpPost]
        [Route("api/DepartmentMapping/DepartmentMappingList")]
        public HttpResponseMessage DepartmentMappingList(bool CheckBoxStatus)
        {
            MyLogger.GetInstance().Info($"Entering the DepartmentMapping Controller.  DepartmentMappingList Method   Arguments::null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    DepartmentMappingBL DepartmentMappingBL = new DepartmentMappingBL();
                    var result = DepartmentMappingBL.getDefaultMapping(CheckBoxStatus);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  DepartmentMappingList Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  DepartmentMappingList Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  DepartmentMappingList Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in DepartmentMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/DepartmentMapping/GetDepartmentLazyLoading")]
        public HttpResponseMessage GetDepartmentLazyLoading(LazyLoadingDepartmentBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the DepartmentMapping Controller.  GetDepartmentLazyLoading Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    DepartmentMappingBL department = new DepartmentMappingBL();
                    var result = department.GetDepartmentLazyLoadingBL(obj.page, obj.recsPerPage);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  GetDepartmentLazyLoading Method(Success) ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  GetDepartmentLazyLoading Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  GetDepartmentLazyLoading Method(Failed) ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in DepartmentMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/DepartmentMapping/DepartmentMapList")]
        public HttpResponseMessage DepartmentMapList(int DeptID)
        {
            MyLogger.GetInstance().Info($"Entering the DepartmentMapping Controller.  DepartmentMappingList Method   Arguments::null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    DepartmentMappingBL DepartmentMappingBL = new DepartmentMappingBL();
                    var result = DepartmentMappingBL.getMapMainTask(DeptID);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  DepartmentMappingList Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  DepartmentMappingList Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  DepartmentMappingList Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };

                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in DepartmentMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/DepartmentMapping/DepartmentMaintaskInsertUpdate")]
        public HttpResponseMessage DepartmentMaintaskInsertUpdate(SaveDepartmentMapping mapping)
        {
            MyLogger.GetInstance().Info($"Entering the DepartmentMapping Controller.  DepartmentMaintaskInsertUpdate Method   Arguments::null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    DepartmentMappingBL DepartmentMappingBL = new DepartmentMappingBL();
                    var result = DepartmentMappingBL.InsertDepartmentMaintaskMap(mapping);
                    if (result)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK };
                        MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  DepartmentMaintaskInsertUpdate Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  DepartmentMaintaskInsertUpdate Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the DepartmentMapping Controller.  DepartmentMaintaskInsertUpdate Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };

                }

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in DepartmentMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}
