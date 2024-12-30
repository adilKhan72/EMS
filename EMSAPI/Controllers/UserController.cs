using DemoEFBL.User;
using DemoEFBO.User;
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
    [Authorize]
    [CheckCustomFilter]
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("api/User/GetUserProfile")]
        public HttpResponseMessage GetUserProfile(UserProfileIDBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the User Controller.  GetUserProfile Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    UserProfileBL objUserProfileBL = new UserProfileBL();
                    var result = objUserProfileBL.GetUserProfileBL(obj.UserID, obj.DepartID);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the User Controller.  GetUserProfile Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the User Controller.  GetUserProfile Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the User Controller.  GetUserProfile Method(Failed)  ");
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
        [Route("api/User/GetFilterUserProfile")]
        public HttpResponseMessage GetFilterUserProfile(UserProfileFilterBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the User Controller.  GetFilterUserProfile Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    UserProfileBL objUserProfileBL = new UserProfileBL();
                    var result = objUserProfileBL.GetFilterUserProfileBL(obj);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the User Controller.  GetFilterUserProfile Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the User Controller.  GetFilterUserProfile Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the User Controller.  GetFilterUserProfile Method(Failed)  ");
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
        [Route("api/User/UploadFiles")]
        public HttpResponseMessage UploadFiles(UserProfileImageBO obj )
        {
            MyLogger.GetInstance().Info($"Entering the User Controller.  UploadFiles Method   Arguments:: base64 string of image");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    byte[] imageBytes = Convert.FromBase64String(obj.Imagefileobj);

                    // string base64 = Request.Form[hfImageData.UniqueID].Split(',')[1];
                    // byte[] bytes = Convert.FromBase64String(base64);
                    //File.Copy(bytes.ToString() + ".jpg", "\\\\192.168.2.9\\Web");

                    var fileDirectory = HttpContext.Current.Server.MapPath("~/UserImages/");
                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    string filePath = HttpContext.Current.Server.MapPath("~/UserImages/UserImgID" + obj.UserID + ".png");
                    File.WriteAllBytes(filePath, imageBytes);

                    UserProfileBL objUserProfileBL = new UserProfileBL();
                    UserProfileBO objPic = new UserProfileBO();
                    objPic.Picture = filePath;
                    objPic.LoginID = Convert.ToInt32(obj.UserID);
                    objPic.Status = true;
                    objUserProfileBL.UpdateUserBL(objPic);
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
                   

                //HttpResponseMessage result = null;
                //var httpRequest = HttpContext.Current.Request;
                //if (httpRequest.Files.Count > 0)
                //{
                //    var docfiles = new List<string>();
                //    foreach (string file in httpRequest.Files)
                //    {
                //        var postedFile = httpRequest.Files[file];
                //        if (postedFile != null && postedFile.ContentLength > 0)
                //        {
                //            int MaxContentLength = 1024 * 1024 * 3; //Size = 3 MB  
                //            //IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png" };
                //            //var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                //            //var extension = ext.ToLower();
                //            if (postedFile.ContentLength > MaxContentLength)
                //            {
                //                var message = string.Format("Please Upload a file upto 1 mb.");
                //                HttpError err = new HttpError(message);
                //                return Request.CreateResponse(HttpStatusCode.BadRequest, err);
                //            }
                //            var filePath = HttpContext.Current.Server.MapPath("~/UserImages/UserImgID" + postedFile.FileName + ".png");

                //            postedFile.SaveAs(filePath);
                //            docfiles.Add(filePath);

                //            UserProfileBL objUserProfileBL = new UserProfileBL();
                //            UserProfileBO objPic = new UserProfileBO();
                //            objPic.Picture = filePath;
                //            objPic.LoginID = Convert.ToInt32(postedFile.FileName);
                //            objPic.Status = true;
                //            objUserProfileBL.UpdateUserBL(objPic);
                //            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = "Image Upload Successfully" };
                //            return new HttpResponseMessage
                //            {
                //                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                //                System.Text.Encoding.UTF8, "application/json")
                //            };
                //        }
                //    }
                //    return Request.CreateResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                //}
                //else
                //{

                //    return result = Request.CreateResponse(HttpStatusCode.BadRequest , ResponseResultMsg.ErrorMsg);
                //}
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in User Controller!  " + ex.Message);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }

        }

        [HttpPost]
        [Route("api/User/InserUserProfile")]
        public HttpResponseMessage InserUserProfile(UserProfileTableBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the User Controller.  InserUserProfile Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    UserProfileBL objUserProfileBL = new UserProfileBL();
                    var result = objUserProfileBL.InsertUserTableBL(obj);
                    if (result == true)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the User Controller.  InserUserProfile Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the User Controller.  InserUserProfile Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the User Controller.  InserUserProfile Method(Failed)  ");
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

        //[HttpPost]
        //[Route("api/User/InserUserProfile")]
        //public HttpResponseMessage InserUserProfile(UserProfileBO obj)
        //{
        //    MyLogger.GetInstance().Info($"Entering the User Controller.  InserUserProfile Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
        //    try
        //    {
        //        UserProfileBL objUserProfileBL = new UserProfileBL();
        //        var result = objUserProfileBL.InsertUserBL(obj);
        //        if (result == true)
        //        {
        //            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
        //            MyLogger.GetInstance().Info($"Exit the User Controller.  InserUserProfile Method(Success)  ");
        //            return new HttpResponseMessage
        //            {
        //                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
        //                                    System.Text.Encoding.UTF8, "application/json")
        //            };
        //        }
        //        MyLogger.GetInstance().Info($"Exit the User Controller.  InserUserProfile Method(Failed)  ");
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Error("Exception in User Controller!  " + ex.Message);
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
        //    }
        //}

        [HttpPost]
        [Route("api/User/UpdateUserProfile")]
        public HttpResponseMessage UpdateUserProfile(UserProfileBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the User Controller.  UpdateUserProfile Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    UserProfileBL objUserProfileBL = new UserProfileBL();
                    var result = objUserProfileBL.UpdateUserBL(obj);
                    if (result == true)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the User Controller.  UpdateUserProfile Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                        MyLogger.GetInstance().Info($"Exit the User Controller.  UpdateUserProfile Method(Failed)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };

                    }
                    
                }
                MyLogger.GetInstance().Info($"Exit the User Controller.  UpdateUserProfile Method(Failed)  ");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in User Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }

}
