using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Users
{
    public interface IUsers
    {
        List<GetUserlist_Result> GetUserlist(string LoginName);
    }
}
