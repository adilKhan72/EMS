using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.ChangePassword;


namespace DemoEFDAL.ChangePassword
{
    public class ChangePasswordDAL
    {
        public sp_ChangePassword_Update_Result ChangePassword(int UserId, String oldPassword, String NewPassword)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_ChangePassword_Update(UserId, oldPassword, NewPassword).SingleOrDefault();
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
