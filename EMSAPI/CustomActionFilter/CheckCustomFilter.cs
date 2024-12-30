using DemoEFCommon.ApiResponseResult;
using EMSAPI.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace EMSAPI.ActionFilter
{
    public class CheckCustomFilter : FilterAttribute, IActionFilter
    {
        public Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            //throw new NotImplementedException();
            GlobalFunctions globalfunctions = new GlobalFunctions();
           // var ResponseMsg = null;
            if (actionContext.Request.Headers.Contains("encrypted"))
            {
                string encrypted = actionContext.Request.Headers.GetValues("encrypted").First();
                string type = actionContext.Request.Headers.GetValues("Role_Type").First();
                var decrypt = globalfunctions.Decrypt(encrypted);
                if(!string.IsNullOrEmpty(decrypt)&& !string.IsNullOrEmpty(type))
                {
                    if (type.ToUpper() == decrypt.ToUpper())
                    {
                        if (decrypt != null)
                        {
                            if (decrypt.ToUpper() != "SuperAdmin" || decrypt.ToUpper() != "Admin".ToUpper() || decrypt.ToUpper() != "Staff".ToUpper())
                            {
                                //return Task.FromResult(actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg));
                                actionContext.Request.Headers.Add("AllowActionCheck", "true");
                            }
                        }
                        else
                        {
                            actionContext.Request.Headers.Add("AllowActionCheck", "false");
                        }
                    }
                    else
                    {
                        actionContext.Request.Headers.Add("AllowActionCheck", "false");
                    }
                }
                else
                {
                    actionContext.Request.Headers.Add("AllowActionCheck", "false");
                }



            }
            return continuation();
        }
    }
}