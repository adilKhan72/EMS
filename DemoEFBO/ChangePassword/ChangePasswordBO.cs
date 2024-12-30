using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.ChangePassword
{
    public class ChangePasswordBO
    {
        public int UserId { get; set; }
        public string oldpassword { get; set; }
        public string newpassword { get; set; }
    }
}
