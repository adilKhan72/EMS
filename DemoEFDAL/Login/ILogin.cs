using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Login
{
    interface ILogin
    {
        // this is complex type object we can return Login table object as well if you want
        //Tuple<int,sp_UserLogin_Result> LoginUser(string UserName, String Password);
        Tuple<int, sp_UserProfileTableLogin_Result> LoginUser(string UserName, String Password);

        sp_getLoginUserInfo1_Result GetLoginuserInfo(int UserID); 
    }
}
