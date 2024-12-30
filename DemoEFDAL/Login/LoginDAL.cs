using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Login
{
    public class LoginDAL : ILogin
    {
        public sp_GetMultipleSelect_Result TestFunc()
        {
            try
            {
                using (var context = new EMSEntities())
                {
                    var loginResult = context.sp_GetMultipleSelect();
                    //var login = loginResult.ToList();

                    //Read Data From Second ResultSet
                   // var NotificationResultqq = context.sp_GetMultipleSelect().GetNextResult<LoginTable>();
                    var NotificationResult = loginResult.GetNextResult<NotificationTable>();
                    var Notification = NotificationResult.ToList();
                }

                return null;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public sp_GetFullName1_Result TestFunction()
        {
            try
            {
                using (var context = new EMSEntities())
                {
                    ObjectParameter outParam1 = new ObjectParameter("Count", typeof(int));
                    var obj = context.sp_GetFullName1(1, "Admin", outParam1, "rehan_siddiqui").ToList();
                    var tempvalue = outParam1.Value;
                }

                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public sp_getLoginUserInfo1_Result GetLoginuserInfo(int UserID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var obj = dbContext.sp_getLoginUserInfo1(UserID).FirstOrDefault();// .sp_getLoginUserInfo(UserID).FirstOrDefault();
                    if (obj != null)
                    {
                        return obj;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null; // also we can log exception through common layer }
            }
        }

        //public Tuple<int,sp_UserLogin_Result> LoginUser(String UserName, String Password)
        //{
        //    try
        //    {
        //        //TestFunc();
        //        using (var dbContext = new EMSEntities())
        //        {
        //            var obj = dbContext.sp_UserLogin(UserName, Password).FirstOrDefault();
        //            if (obj != null )
        //            {
        //                return new Tuple<int, sp_UserLogin_Result>(1,obj);
        //            }
        //            else
        //            {
        //                return new Tuple<int, sp_UserLogin_Result>(-1, null);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        // return null; // also we can log exception through common layer }
        //        return new Tuple<int, sp_UserLogin_Result>(-2, null);
        //    }
        //}

       //Login With UserProfileTable
        public Tuple<int, sp_UserProfileTableLogin_Result> LoginUser(String UserName, String Password)
        {
            try
            {
                //TestFunc();
                using (var dbContext = new EMSEntities())
                {
                    var obj = dbContext.sp_UserProfileTableLogin(UserName, Password).FirstOrDefault();
                    if (obj != null)
                    {
                        return new Tuple<int, sp_UserProfileTableLogin_Result>(1, obj);
                    }
                    else
                    {
                        return new Tuple<int, sp_UserProfileTableLogin_Result>(-1, null);
                    }
                }

            }
            catch (Exception ex)
            {
                // return null; // also we can log exception through common layer }
                return new Tuple<int, sp_UserProfileTableLogin_Result>(-2, null);
            }
        }
        public bool DeactiveUser(int UserID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var obj = dbContext.sp_DeactiveUser(UserID);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false; // also we can log exception through common layer }
            }
        }
    }
}
