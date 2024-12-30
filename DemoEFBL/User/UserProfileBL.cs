using DemoEFBO.User;
using DemoEFDAL.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DemoEFBL.User
{
  public class UserProfileBL
    {
        public List<UserProfileBO> GetUserProfileBL(int UserID,int DepartID)
        {

            try
            {
                    List<UserProfileBO> UserProfileResponse = new List<UserProfileBO>();
                    UserProfileDAL objDAL = new UserProfileDAL();
                    var objResult = objDAL.GetUserProfile(UserID, DepartID);
                    if (objResult != null)
                    {
                        for (int i = 0; i < objResult.Count; i++)
                        {
                        UserProfileResponse.Add(new UserProfileBO
                        {
                            UserProfileTableID =  Convert.ToInt32(objResult[i].UserProfileTableID),
                            UserID = Convert.ToInt32(objResult[i].TaskOwnerID),
                            LoginID = Convert.ToInt32(objResult[i].UserID),
                            UserName = objResult[i].Username,
                            Name = objResult[i].FullName,
                            Email = objResult[i].EmailAddress,
                            Department = objResult[i].DepartmentName,
                            JoiningDate = objResult[i].JoiningDate.ToString(),
                            Status = Convert.ToBoolean(objResult[i].IsActive),
                            EMSRole = objResult[i].AccountType,
                            Picture = objResult[i].Picture,
                            Creationdatetime = objResult[i].CreationDateTime.ToString(),
                            Designation = objResult[i].Designation,
                            Password= objResult[i].Password,
                            Department_ID = objResult[i].Department_ID > 0 ? Convert.ToInt32(objResult[i].Department_ID) : 0,
                        });
                        }
                        return UserProfileResponse;
                    }
                
                return null;
            }
            catch { return null; }
        }
        public List<UserProfileBO> GetFilterUserProfileBL(UserProfileFilterBO obj)
        {
            try
            {
                if (String.IsNullOrEmpty(obj.Name))
                {
                    obj.Name = null;
                }
                if (String.IsNullOrEmpty(obj.Department))
                {
                    obj.Department = null;
                }
                if (String.IsNullOrEmpty(obj.Designation))
                {
                    obj.Designation = null;
                }
                if (String.IsNullOrEmpty(obj.Email))
                {
                    obj.Email = null;
                }
                if (String.IsNullOrEmpty(obj.EMSRole))
                {
                    obj.EMSRole = null;
                }
               
                List<UserProfileBO> UserProfileResponse = new List<UserProfileBO>();
                UserProfileDAL objDAL = new UserProfileDAL();
                var objResult = objDAL.GetFilterUserProfile(obj);
                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        UserProfileResponse.Add(new UserProfileBO
                        {
                            UserProfileTableID = Convert.ToInt32(objResult[i].UserProfileTableID),
                            UserID = Convert.ToInt32(objResult[i].TaskOwnerID),
                            LoginID = Convert.ToInt32(objResult[i].UserID),
                            UserName = objResult[i].Username,
                            Name = objResult[i].FullName,
                            Email = objResult[i].EmailAddress,
                            Department = objResult[i].DepartmentName,
                            JoiningDate = objResult[i].JoiningDate.ToString(),
                            Status = Convert.ToBoolean(objResult[i].IsActive),
                            EMSRole = objResult[i].AccountType,
                            Picture = objResult[i].Picture,
                            Creationdatetime = objResult[i].CreationDateTime.ToString(),
                            Designation = objResult[i].Designation,
                            Department_ID = objResult[i].Department_ID>0 ? Convert.ToInt32(objResult[i].Department_ID) : 0 ,
                        });
                    }
                    return UserProfileResponse;
                }
                return null;
            }
            catch { return null; }
        }

        public bool InsertUserTableBL(UserProfileTableBO obj)
        {
            try
            {
                UserProfileDAL objDal = new UserProfileDAL();
                return objDal.InsertUserProfileTable(obj);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        //public bool InsertUserBL(UserProfileBO obj)
        //{
        //    try
        //    {
        //        UserProfileDAL objDal = new UserProfileDAL();
        //        return objDal.InsertUserProfile(obj);
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //}

        public bool UpdateUserBL(UserProfileBO obj)
        {
            try
            {
                UserProfileDAL objDal = new UserProfileDAL();
                return objDal.UpdateUserProfile(obj);
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
