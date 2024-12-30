using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.Login;
using DemoEFDAL.Login;

namespace DemoEFBL.Login
{
    public class LoginBL
    {
        //public Tuple<int,LoginBO> LoginUser(string UserName, string Password)
        //{
        //    try
        //    {
        //        if(!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
        //        {
        //            LoginBO objBusinessObject = new LoginBO();
        //            LoginDAL objDal = new LoginDAL();
        //            var objResult = objDal.LoginUser(UserName, Password);
        //            if (objResult.Item1 > 0 && objResult.Item2.Id > 0)
        //            {
        //                objBusinessObject.FullName = objResult.Item2.FullName;
        //                objBusinessObject.Role = objResult.Item2.AccountType;
        //                objBusinessObject.UserId = objResult.Item2.Id;
        //                objBusinessObject.UserName = objResult.Item2.Username;
        //                objBusinessObject.UserImgURL = objResult.Item2.Picture;
        //                objBusinessObject.Designation = objResult.Item2.Designation;
        //                return new Tuple<int, LoginBO>(1,objBusinessObject);
        //            }
        //            if(objResult.Item1 == -1)
        //            {
        //                // invalid login 
        //                return new Tuple<int, LoginBO>(-1, null);
        //            }
        //            else
        //            {
        //                // exception from dal function or db side 
        //                return new Tuple<int, LoginBO>(-2, null);
        //            }

        //        }
        //        // empty username and password 
        //        return new Tuple<int, LoginBO>(-3, null);
        //    }
        //    catch(Exception ex)
        //    {
        //        // exception from BAL function
        //        return new Tuple<int, LoginBO>(-4, null);
        //    }
        //}
        public Tuple<int, LoginBO> LoginUser(string UserName, string Password)
        {
            try
            {
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    LoginBO objBusinessObject = new LoginBO();
                    LoginDAL objDal = new LoginDAL();
                    var objResult = objDal.LoginUser(UserName, Password);
                    if (objResult.Item1 == -1)
                    {
                        // invalid login 
                        return new Tuple<int, LoginBO>(-1, null);
                    }
                    if (objResult.Item2 != null && string.IsNullOrEmpty(objResult.Item2.EmailAddress))
                    {
                        if (objResult.Item2.IsActive==false)
                        {
                            return new Tuple<int, LoginBO>(-4, null);
                        }
                        var res = objDal.DeactiveUser(objResult.Item2.UserProfileTableID);
                        return new Tuple<int, LoginBO>(-4, null);
                    }
                    if (objResult.Item2.IsActive == false)
                    {
                        return new Tuple<int, LoginBO>(-5, null);
                    }
                    if (objResult.Item1 > 0 && objResult.Item2.UserID > 0)
                    {
                        objBusinessObject.FullName = objResult.Item2.FullName;
                        objBusinessObject.Role = objResult.Item2.AccountType;
                        objBusinessObject.UserId = objResult.Item2.UserProfileTableID;
                        objBusinessObject.UserName = objResult.Item2.Username;
                        objBusinessObject.UserImgURL = objResult.Item2.Picture;
                        objBusinessObject.Designation = objResult.Item2.Designation;

                        return new Tuple<int, LoginBO>(1, objBusinessObject);
                    }
                    if (objResult.Item1 == -1)
                    {
                        // invalid login 
                        return new Tuple<int, LoginBO>(-1, null);
                    }
                    else
                    {
                        // exception from dal function or db side 
                        return new Tuple<int, LoginBO>(-2, null);
                    }

                }
                // empty username and password 
                return new Tuple<int, LoginBO>(-3, null);
            }
            catch (Exception ex)
            {
                // exception from BAL function
                return new Tuple<int, LoginBO>(-4, null);
            }
        }
        public LoginBO GetLoggedUserInfo(int UserID)
        {
            try
            {
                if (UserID > 0)
                {
                    LoginBO objBusinessObject = new LoginBO();
                    LoginDAL objDal = new LoginDAL();
                    var objResult = objDal.GetLoginuserInfo(UserID);
                    if (objResult != null)
                    {
                        objBusinessObject.FullName = objResult.FullName;
                        objBusinessObject.Role = objResult.AccountType;
                        objBusinessObject.UserId = objResult.Id;
                        objBusinessObject.UserName = objResult.Username;
                        return objBusinessObject;
                    }
                    return null;
                }
                return null;
            }
            catch { return null; }
        }
    }
}
