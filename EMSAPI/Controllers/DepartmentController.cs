using DemoEFBL.Department;
using DemoEFBL.Designation;
using DemoEFBL.Reports;
using DemoEFBO.Department;
using DemoEFBO.Designation;
using DemoEFBO.Reports;
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
    public class DepartmentController : ApiController
    {
        [HttpPost]
        [Route("api/Department/GetDepartments")]
        public HttpResponseMessage GetDepartments()
        {
            MyLogger.GetInstance().Info($"Entering the Department Controller.  GetDepartments Method");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    DepartmentBL objDepBL = new DepartmentBL();
                    var result = objDepBL.GetDepartment();
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Department Controller.  GetDepartments Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        MyLogger.GetInstance().Info($"Exit the Department Controller.  GetDepartments Method(Failed)  ");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Department Controller.  GetDepartments Method(Failed)  ");
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
        [Route("api/Department/InsertDepartment")]
        public HttpResponseMessage InsertDepartment(DepartmentModel obj)
        {
            MyLogger.GetInstance().Info($"Entering the Designation Controller.  InsertDesignation Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    if (obj != null)
                    {
                        DepartmentBL department = new DepartmentBL();
                        var result = department.InsertDepartment(obj);
                        if (result)
                        {
                            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                            MyLogger.GetInstance().Info($"Exit the Designation Controller.  InsertDesignation Method(Success)  ");
                            return new HttpResponseMessage
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                    System.Text.Encoding.UTF8, "application/json")
                            };
                        }
                        else
                        {
                            var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                            MyLogger.GetInstance().Info($"Exit the Designation Controller.  InsertDesignation Method(Failed)   ");
                            return ResponseMsg;
                        }
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Designation Controller.  InsertDesignation Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Department Controller.  InsertDesignation Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Designation Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/Department/DeleteDepartment")]
        public HttpResponseMessage DeleteDepartment(DepartmentModel obj)
        {
            MyLogger.GetInstance().Info($"Entering the Designation Controller.  DeleteDepartment Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    if (obj != null)
                    {
                        DepartmentBL department = new DepartmentBL();
                        var result = department.DeleteDepartment(obj);
                        if (result)
                        {
                            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                            MyLogger.GetInstance().Info($"Exit the Designation Controller.  DeleteDepartment Method(Success)  ");
                            return new HttpResponseMessage
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                    System.Text.Encoding.UTF8, "application/json")
                            };
                        }
                        else
                        {
                            var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                            MyLogger.GetInstance().Info($"Exit the Designation Controller.  DeleteDepartment Method(Failed)   ");
                            return ResponseMsg;
                        }
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Designation Controller.  DeleteDepartment Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Designation Controller.  DeleteDepartment Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };

                }
                  
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Designation Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}
