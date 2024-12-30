using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using DemoEFCommon.ApiResponseResult;
using EMSAPI.Utility.Nlog;
using DemoEFBO.Client;
using DemoEFDAL.Client;
using DemoEFBL.Client;
using EMSAPI.ActionFilter;
using EMSAPI.Common;
using System.Web.Http.Results;

namespace EMSAPI.Controllers
{
    [Authorize]
    [CheckCustomFilter]
    public class ClientController : ApiController
    {
        [HttpPost]
        [Route("api/Client/GetClientList")]
        public HttpResponseMessage GetClientList(ClientViewModel objGetState)
        {
            MyLogger.GetInstance().Info($"Entering the Client Controller.  GetClientList Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objGetState)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    var result = ClientBL.GetAllClient(objGetState);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Client Controller.  GetClientList Method(Success)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Client Controller.  GetClientList Method(failed)   ");
                        return ResponseMsg;
                    }

                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Client Controller.  GetClientList Method(failed)   ");
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
                MyLogger.GetInstance().Error("Exception in Client Controller!  GetClientList Method  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Client/InsertUpdateClient")]
        public HttpResponseMessage InsertUpdateClient(ClientViewModel objGetState)
        {
            MyLogger.GetInstance().Info($"Entering the Client Controller.  InsertUpdateClient Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objGetState)}");
            try
            {

                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ClientBL client = new ClientBL();
                    var result = client.InsertUpdateClient(objGetState);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Client Controller.  InsertUpdateClient Method(Success)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Client Controller.  InsertUpdateClient Method(failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the Client Controller.  InsertUpdateClient Method(failed)   ");
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
                MyLogger.GetInstance().Error("Exception in Client Controller!  InsertUpdateClient Method  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/Client/DeleteClient")]
        public HttpResponseMessage DeleteClient(ClientViewModel objGetState)
        {
            MyLogger.GetInstance().Info($"Entering the Client Controller.  DeleteClient Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objGetState)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ClientBL client = new ClientBL();
                    var result = client.DeleteClient(objGetState);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Client Controller.  DeleteClient Method(Success)   ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                        MyLogger.GetInstance().Info($"Exit the Client Controller.  DeleteClient Method(failed)   ");
                        return ResponseMsg;
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Client Controller.  DeleteClient Method(failed)   ");
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
                MyLogger.GetInstance().Error("Exception in Client Controller!  DeleteClient Method  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}
