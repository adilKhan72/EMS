using DemoEFBL.Settings;
using DemoEFBO.Setting;
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

namespace EMSAPI.Controllers
{
    [Authorize]
    [CheckCustomFilter]
    public class SettingController : ApiController
    {
        [HttpPost]
        [Route("api/Setting/GetSetting")]
        public HttpResponseMessage GetSetting(SettingRequestBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the Setting Controller.  GetSetting Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    SettingsBAL objSettingBL = new SettingsBAL();
                    var result = objSettingBL.GetSetting(obj);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result, SettingA = result.Where(x => x.SettingType.ToUpper() == "Setting A".ToUpper()).ToList(), SettingB = result.Where(x => x.SettingType.ToUpper() == "Setting B".ToUpper()).ToList(), SettingC = result.Where(x => x.SettingType.ToUpper() == "Setting C".ToUpper()).ToList() };
                        MyLogger.GetInstance().Info($"Exit the Setting Controller.  GetSetting Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = "" };
                        MyLogger.GetInstance().Info($"Exit the Setting Controller.  GetSetting Method(Failed)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the Setting Controller.  GetSetting Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
               
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Setting Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Setting/InsertSetting")]
        public HttpResponseMessage InsertSetting(SettingModelBO setting)
        {
            MyLogger.GetInstance().Info($"Entering the Setting Controller.  InsertSetting Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(setting)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    if (setting != null)
                    {
                        SettingsBAL objSettingBL = new SettingsBAL();
                        var result = objSettingBL.InsertSetting(setting);
                        if (result)
                        {
                            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                            MyLogger.GetInstance().Info($"Exit the Setting Controller.  InsertSetting Method(Success)  ");
                            return new HttpResponseMessage
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                    System.Text.Encoding.UTF8, "application/json")
                            };
                        }
                        MyLogger.GetInstance().Info($"Exit the Setting Controller.  InsertSetting Method(Failed)  ");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                    MyLogger.GetInstance().Info($"Exit the Setting Controller.  InsertSetting Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Setting Controller.  InsertSetting Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Setting Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Setting/InsertDeleteSetting")]
        public HttpResponseMessage InsertDeleteSetting([FromBody] int ID)
        {
            MyLogger.GetInstance().Info($"Entering the Setting Controller.  InsertDeleteSetting Method   Arguments:: {ID}");
            try
            {
                if(ID > 0)
                {
                    SettingsBAL objSettingBL = new SettingsBAL();
                    var result = objSettingBL.DeleteSetting(ID);
                    if (result)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                        MyLogger.GetInstance().Info($"Exit the Setting Controller.  InsertDeleteSetting Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    MyLogger.GetInstance().Info($"Exit the Setting Controller.  InsertDeleteSetting Method(Failed)  ");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                }
                MyLogger.GetInstance().Info($"Exit the Setting Controller.  InsertDeleteSetting Method(Failed)  ");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Setting Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

    }
}