using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Users
{
    public class UsersDAL : IUsers
    {
        public List<GetUserlist_Result> GetUserlist(string LoginName)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.GetUserlist(LoginName).ToList();
                    if(result != null && result.Count > 0)
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
