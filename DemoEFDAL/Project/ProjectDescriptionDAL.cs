using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Project
{
    public class projectDescriptionDAL
    {
        public List<Get_ProjectDescription_Result> ProjectDescription()
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.Get_ProjectDescription().OrderBy(p=>p.Name).ToList();// .sp_user sp_userNotificationCount(userID);

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
