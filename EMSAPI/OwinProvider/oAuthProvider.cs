using DemoEFBL.Login;
using EMSAPI.Areas.App_Start;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Cors;

namespace EMSAPI.OwinProvider
{
    public class oAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                var username = context.UserName;
                var password = context.Password;

                LoginBL user = new LoginBL();
                var checkValidation = user.LoginUser(username, password);

                if (checkValidation.Item1 > 0 && checkValidation.Item2.UserId>0)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, checkValidation.Item2.UserName),
                        new Claim("UserID", Convert.ToString(checkValidation.Item2.UserId))
                    };

                    ClaimsIdentity oAutIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                    context.Validated(new AuthenticationTicket(oAutIdentity, new AuthenticationProperties() { }));
                }
                else
                {
                    context.SetError("invalid_grant", "Error");
                }
            });
        }
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }
            return Task.FromResult<object>(null);
        }
    }
}