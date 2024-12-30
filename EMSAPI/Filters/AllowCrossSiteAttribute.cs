using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace EMSAPI.Filters
{
    public class AllowCrossSiteAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext HttpContext)
        {
            //HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            //HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        }

    }
}