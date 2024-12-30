using DemoEFBO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Users
{
    public class UserProfileDAL
    {
        //public List<sp_GetUserProfile_Result> GetUserProfile(int userID)
        //{
        //    try
        //    {
        //        using (var dbContext = new EMSEntities())
        //        {
        //            var result = dbContext.sp_GetUserProfile(userID).ToList();
        //            if (result != null)
        //            {
        //                return result;
        //            }
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //Get Profile From UserProfile Table and Manage hnages in Reset and Change Password
        public List<sp_GetUserProfileTable_Result> GetUserProfile(int userID,int DepartID = 0)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    if (DepartID == 0)
                    {
                        DepartID = -1;
                    }
                    var result = dbContext.sp_GetUserProfileTable(userID, DepartID).OrderBy(o => o.FullName).ToList();
                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<sp_FilterUserProfileTable_Result> GetFilterUserProfile(UserProfileFilterBO obj)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_FilterUserProfileTable(obj.Name,obj.Designation,obj.Department,obj.Status,obj.EMSRole,obj.Email).ToList();
                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool InsertUserProfileTable(UserProfileTableBO obj)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    dbContext.sp_InserUserProfileTable(obj.UserProfileID,obj.UserName,obj.FullName,obj.Designation,obj.Department,obj.IsActive,obj.AccountType,obj.Email,obj.JoiningDate);

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //public bool InsertUserProfile(UserProfileBO obj)
        //{
        //    try
        //    {
        //        using (var dbContext = new EMSEntities())
        //        {
        //            dbContext.sp_InsertUser(obj.LoginID,obj.Name,obj.Email,obj.Department,obj.JoiningDate,obj.Status,obj.EMSRole,obj.Designation);
                   
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public bool UpdateUserProfile(UserProfileBO obj)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    dbContext.sp_EditUserProfile(obj.LoginID, obj.Name, obj.Email, obj.Department, obj.JoiningDate, obj.Status, obj.EMSRole, obj.Designation,obj.Picture);

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
