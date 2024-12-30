using EMSAPI.OwinProvider;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;

[assembly: OwinStartup(typeof(EMSAPI.Areas.App_Start.Startup))]
namespace EMSAPI.Areas.App_Start
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        static Startup()
        {
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/token"),
                Provider = new oAuthProvider(),
             // AccessTokenExpireTimeSpan = TimeSpan.FromDays(2),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(8),
                AllowInsecureHttp = true
            };
        }
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseOAuthBearerTokens(OAuthOptions); 
        }
    }
}