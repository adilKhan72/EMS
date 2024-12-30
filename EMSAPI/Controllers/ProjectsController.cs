using DemoEFBL.CommonLists;
using DemoEFBL.Dashboard;
using DemoEFBL.Projects;
using DemoEFBO.Dashboard;
using DemoEFBO.Projects;
using DemoEFCommon.ApiResponseResult;
using EMSAPI.ActionFilter;
using EMSAPI.Common;
using EMSAPI.Utility.Nlog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace EMSAPI.Controllers
{
    public class GetProjectParam
    {
        public string ProjectName { get; set; }
    }

    public class DeleteProject
    {
        public int ProjectID { get; set; }
    }


    [Authorize]
    [CheckCustomFilter]
    public class ProjectsController : ApiController
    {
        [HttpPost]
        [Route("api/Projects/GetProjects")]
        public HttpResponseMessage GetProjects(GetProjectParam obj)
        {
            MyLogger.GetInstance().Info($"Entering the Projects Controller.  GetProjects Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    var result = ProjectsBL.GetProjects(obj.ProjectName);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjects Method(Success) ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjects Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjects Method(Failed) ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Projects Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Projects/GetProjectsLazyLoading")]
        public HttpResponseMessage GetProjectsLazyLoading(LazyLoadingProjectBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the Projects Controller.  GetProjectsLazyLoading Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    var result = ProjectsBL.GetProjectsLazyLoadingBL(obj.page, obj.recsPerPage);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjectsLazyLoading Method(Success) ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjectsLazyLoading Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjectsLazyLoading Method(Failed) ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Projects Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/Projects/GetProjectsBudgetDetail")]
        public HttpResponseMessage GetProjectsBudgetDetail(ProjectsBO project)
        {
            MyLogger.GetInstance().Info($"Entering the Projects Controller.  GetProjects Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(project)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    var result = ProjectsBL.GetProjectsBudgetDetailBL(project);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjectsBudgetDetail Method(Success) ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.NoContent };
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjectsBudgetDetail Method(falied) ");
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
                    MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjectsBudgetDetail Method(falied) ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Projects Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Projects/InsertProject")]
        public HttpResponseMessage InsertProject(ProjectsBO project)
        {
            MyLogger.GetInstance().Info($"Entering the Projects Controller.  InsertProject Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(project)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    if (project != null)
                    {
                        ProjectsBL objProjectBL = new ProjectsBL();
                        var result = objProjectBL.InsertProject(project);
                        if (result)
                        {
                            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                            MyLogger.GetInstance().Info($"Exit the Projects Controller.  InsertProject Method(Success)   ");
                            return new HttpResponseMessage
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                    System.Text.Encoding.UTF8, "application/json")
                            };
                        }
                        else
                        {
                            var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                            MyLogger.GetInstance().Info($"Exit the Projects Controller.  InsertProject Method(Failed)   ");
                            return ResponseMsg;
                        }
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  InsertProject Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Projects Controller.  InsertProject Method(Failed)   ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Projects Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        //[HttpPost]
        //[Route("api/Projects/DeleteProject")]

        //public HttpResponseMessage DeleteProject(DeleteProject obj)
        //{
        //    try
        //    {
        //        int ID = -1;
        //        int.TryParse(obj.ProjectID.ToString(), out ID);
        //        if (ID > 0)
        //        {
        //            ProjectsBL objProjectBL = new ProjectsBL();
        //            var result = objProjectBL.DeleteProject(ID);
        //            if (result)
        //            {
        //                var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
        //                return new HttpResponseMessage
        //                {
        //                    Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
        //                                        System.Text.Encoding.UTF8, "application/json")
        //                };
        //            }
        //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
        //        }
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
        //    }
        //}

        [HttpPost]
        [Route("api/Projects/GetProjectsList")]
        public HttpResponseMessage GetProjectsList()
        {
            MyLogger.GetInstance().Info($"Entering the Projects Controller.  GetProjectsList Method   Arguments:: null");
            try
            {

                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ProjectDescriptionBL objProjectBL = new ProjectDescriptionBL();

                    var result = objProjectBL.GetProjectDescriptionBL();
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjectsList Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjectsList Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Projects Controller.  GetProjectsList Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
            
            }
               
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Projects Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }


        [HttpPost]
        [Route("api/Projects/ProjectPic")]
        public HttpResponseMessage ProjectPic(ProjectImageBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the Projects Controller.  ProjectPic Method   Arguments:: base64 string of image");
            try
            {

                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    var ProjectID = obj.ProjectID;
                    if (obj.ProjectID == 0)
                    {
                        ProjectDescriptionBL objProjectBL = new ProjectDescriptionBL();
                        var lstProjectID = objProjectBL.GetProjectDescriptionBL();
                        ProjectID = lstProjectID[lstProjectID.Count - 1].ID;
                    }
                    byte[] imageBytes = Convert.FromBase64String(obj.Imagefileobj);

                    var fileDirectory = HttpContext.Current.Server.MapPath("~/ProjectImages/");
                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    string filePath = HttpContext.Current.Server.MapPath("~/ProjectImages/ProjectImgID" + ProjectID + ".png");
                    File.WriteAllBytes(filePath, imageBytes);

                    var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = "Image Upload Successfully" };
                    MyLogger.GetInstance().Info($"Exit the User Controller.  UploadFiles Method(Success)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                        System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the User Controller.  UploadFiles Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                        System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in User Controller!  " + ex.Message);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }

        }

        [HttpPost]
        [Route("api/Projects/SearchProject")]
        public HttpResponseMessage SearchProject(GetProjectParam obj)
        {
            MyLogger.GetInstance().Info($"Entering the Projects Controller.  SearchProject Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ProjectsBL objProjectBL = new ProjectsBL();
                    var result = objProjectBL.SearchProjectBL(obj.ProjectName);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  SearchProject Method(Success) ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  SearchProject Method(Failed)  ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Projects Controller.  SearchProject Method(Failed) ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };

                }
                  
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Projects Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/Projects/AllProject")]
        public HttpResponseMessage AllProject(GetDashBoardStatsUpdateBO objGetState)
        {
            MyLogger.GetInstance().Info($"Entering the Projects Controller.  AllProject Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objGetState)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    DashboardStatsBL objDbSBL = new DashboardStatsBL();
                    var result = objDbSBL.GetAll_Project(objGetState);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  AllProject Method(Success)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Projects Controller.  AllProject Method(failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Projects Controller.  AllProject Method(failed)   ");
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
                MyLogger.GetInstance().Error("Exception in Projects Controller!  AllProject Method  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}