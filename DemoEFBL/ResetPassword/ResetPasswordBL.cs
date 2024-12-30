using DemoEFBL.Shared;
using DemoEFBL.User;
using DemoEFBO.ResetPassword;
using DemoEFDAL.ResetPassword;
using DemoEFDAL.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.ResetPassword
{
    public class ResetPasswordBL
    {
        public int ResetPasswordValidations(string globalID)
        {
            try
            {
                ResetPasswordDAL objDAL = new ResetPasswordDAL();
                var objResult = objDAL.ResetPasswordValidation(globalID);
                if (objResult != null)
                {
                    return objResult.Response;
                }
                return -1;
            }

            catch (Exception ex)
            {
                return -1;

            }
        }

        public ResetpasswordBO ResetPassword(int Loginid, string password)
        {
            try
            {
                if (Loginid > 0)
                {
                    ResetpasswordBO objBusinessObject = new ResetpasswordBO();
                    ResetPasswordDAL objDal = new ResetPasswordDAL();

                   
                    var objResult = objDal.ResetPassword(Loginid, password);
                    if (objResult != null)
                    {
                        objBusinessObject.ResponseCode = objResult.responseCode;

                        if (objResult.responseCode == 1)
                        {
                            UserProfileDAL objDAL = new UserProfileDAL();
                            var result = objDAL.GetUserProfile(Loginid);
                            if (result != null)
                            {
                                EmailMsg emailobj = new EmailMsg();
                                emailobj.EmailSendingFun(Loginid,result[0].EmailAddress, "Reset Password", "Your password has been reset successfully");
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
