using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.ForgetPassword
{
    public class ForgetPasswordDAL
    {
        public sp_ForgetPassword_Result ForgetPassword(string Useremail)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_ForgetPassword(Useremail).FirstOrDefault();
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
