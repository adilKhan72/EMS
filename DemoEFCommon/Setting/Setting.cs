using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace DemoEFCommon.Setting
{
    public static class Setting
    {
        public static string forget_password_link = WebConfigurationManager.AppSettings["ForgetPasswordLink"];

    }
}
