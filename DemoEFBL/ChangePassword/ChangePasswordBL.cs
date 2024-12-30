using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBL.Shared;
using DemoEFBO.ChangePassword;
using DemoEFDAL.ChangePassword;
using DemoEFDAL.Users;

namespace DemoEFBL.ChangePassword
{
    public class ChangePasswordBL
    {
        public ChangePassResponseBO ChangePassword(int Userid, string oldpassword, string newpassword)
        {
            try
            {
                if (Userid > 0)
                {
                    ChangePassResponseBO objBusinessObject = new ChangePassResponseBO();
                    ChangePasswordDAL objDal = new ChangePasswordDAL();
                    var objResult = objDal.ChangePassword(Userid, oldpassword, newpassword);
                    if (objResult != null && oldpassword != newpassword)
                    {
                        //objBusinessObject.UserId = objResult.UserId;
                        //objBusinessObject.oldpassword = objResult.oldpassword;
                        //objBusinessObject.newpassword = objResult.newpassword;
                        objBusinessObject.ResponseCode = objResult.responseCode;

                        if (objResult.responseCode == 1)
                        {
                            UserProfileDAL objDAL = new UserProfileDAL();
                            var result = objDAL.GetUserProfile(Userid);
                            if (result != null)
                            {
                                EmailMsg emailobj = new EmailMsg();
                                emailobj.EmailSendingFun(Convert.ToInt32(result[0].UserProfileTableID),result[0].EmailAddress, "Change Password", "Your password has been changed successfully");
                            }
                        }
                        objBusinessObject.ResponseMessage = objResult.responsemsg;

                        return objBusinessObject;
                    }
                    return null;
                }
                return null;
            }
            catch
            {
                return null;

            }
        }

    }
}
