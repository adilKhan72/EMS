using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.ResetPassword
{
    public class ResetPasswordDAL
    {
        public sp_ResetPassword_Result ResetPassword(int LoginId, string changePassword)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_ResetPassword(LoginId, changePassword).SingleOrDefault();
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
        public sp_ResetPasswordValidation_Result ResetPasswordValidation(string globalID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_ResetPasswordValidation(globalID).SingleOrDefault();
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
    }
}
