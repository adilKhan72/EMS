using DemoEFBL.ResourceMapping;
using DemoEFBO.ResourceMapping;
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
    public class ResourceMappingController : ApiController
    {
        [HttpPost]
        [Route("api/ResourceMapping/GetResourceMappingList")]
        public HttpResponseMessage GetResourceMappingList(GetResourceMappingBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the ResourceMapping Controller.  GetResourceMappingList Method   Arguments::null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ResourceMappingBL objResourceMappingBL = new ResourceMappingBL();
                    var result = objResourceMappingBL.getResourceMapping(obj);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  GetResourceMappingList Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  GetResourceMappingList Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  GetResourceMappingList Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ResourceMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        //[HttpPost]
        //[Route("api/ResourceMapping/SaveUpdateMapping")]
        //public HttpResponseMessage SaveUpdateMapping(SaveUpdateMappingBO Obj)
        //{
        //    MyLogger.GetInstance().Info($"Entering the ResourceMapping Controller.  SaveUpdateMapping Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(Obj)}");
        //    try
        //    {
        //        ResourceMappingBL objResourceMappingBL = new ResourceMappingBL();

        //        var result = objResourceMappingBL.SaveUpdateMapping(Obj);
        //        if (result == true)
        //        {
        //            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
        //            MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveUpdateMapping Method(Success)  ");
        //            return new HttpResponseMessage
        //            {
        //                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
        //                                    System.Text.Encoding.UTF8, "application/json")
        //            };
        //        }
        //        MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveUpdateMapping Method(Failed)  ");
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Error("Exception in ResourceMapping Controller!  " + ex.Message);
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
        //    }
        //}

        [HttpPost]
        [Route("api/ResourceMapping/FetchProjectUser")]
        public HttpResponseMessage FetchProjectUser(FetchProjectUserBO Obj)
        {
            MyLogger.GetInstance().Info($"Entering the ResourceMapping Controller.  FetchProjectUser Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(Obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ResourceMappingBL objResourceMappingBL = new ResourceMappingBL();

                    var result = objResourceMappingBL.FetchProjectUserBL(Obj.ProjectId);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  FetchProjectUser Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  FetchProjectUser Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  FetchProjectUser Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ResourceMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/ResourceMapping/SaveProjectMapping")]
        public HttpResponseMessage SaveProjectMapping(SaveProjectMapping Obj)
        {
            MyLogger.GetInstance().Info($"Entering the ResourceMapping Controller.  SaveProjectMapping Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(Obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ResourceMappingBL objResourceMappingBL = new ResourceMappingBL();

                    var result = objResourceMappingBL.SaveProjectMappingBL(Obj);
                    if (result == true)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveProjectMapping Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveProjectMapping Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveProjectMapping Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ResourceMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/ResourceMapping/FetchProjectMappedMainTask")]
        public HttpResponseMessage FetchProjectMappedMainTask(FetchProjectMappedMaintaskBO Obj)
        {
            MyLogger.GetInstance().Info($"Entering the ResourceMapping Controller.  FetchProjectMappedMainTask Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(Obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ResourceMappingBL objResourceMappingBL = new ResourceMappingBL();

                    var result = objResourceMappingBL.FetchMappedProjectMaintaskBL(Obj.MaintaskId);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  FetchProjectMappedMainTask Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  FetchProjectMappedMainTask Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  FetchProjectMappedMainTask Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ResourceMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/ResourceMapping/SaveMainTaskMapping")]
        public HttpResponseMessage SaveMainTaskMapping(SaveMainTaskMapping Obj)
        {
            MyLogger.GetInstance().Info($"Entering the ResourceMapping Controller.  SaveMainTaskMapping Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(Obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ResourceMappingBL objResourceMappingBL = new ResourceMappingBL();

                    var result = objResourceMappingBL.SaveMainTaskMappingBL(Obj);
                    if (result == true)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveMainTaskMapping Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveMainTaskMapping Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveMainTaskMapping Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ResourceMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/ResourceMapping/SaveUserMapping")]
        public HttpResponseMessage SaveUserMapping(SaveUserMapping Obj)
        {

            MyLogger.GetInstance().Info($"Entering the ResourceMapping Controller.  SaveUserMapping Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(Obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ResourceMappingBL objResourceMappingBL = new ResourceMappingBL();
                    var result = objResourceMappingBL.SaveUserMappingBL(Obj);
                    if (result == true)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveUserMapping Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveUserMapping Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveUserMapping Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ResourceMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/ResourceMapping/ClientunMapping")]
        public HttpResponseMessage ClientunMapping()
        {

            MyLogger.GetInstance().Info($"Entering the ResourceMapping Controller.  SaveUserMapping Method   Arguments:: null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ResourceMappingBL objResourceMappingBL = new ResourceMappingBL();
                    var result = objResourceMappingBL.FetchProject();
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveUserMapping Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveUserMapping Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  SaveUserMapping Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ResourceMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/ResourceMapping/GetProjectWithPercentage")]
        public HttpResponseMessage GetProjectWithPercentage(int TaskOWnerID)
        {
            MyLogger.GetInstance().Info($"Entering the ResourceMapping Controller.  GetResourceMappingList Method   Arguments::null");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ResourceMappingBL objResourceMappingBL = new ResourceMappingBL();
                    var result = objResourceMappingBL.getProjectwithPercentage(TaskOWnerID);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  GetResourceMappingList Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  GetResourceMappingList Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the ResourceMapping Controller.  GetResourceMappingList Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in ResourceMapping Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}
