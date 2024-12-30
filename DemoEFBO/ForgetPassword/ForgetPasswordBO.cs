using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.ForgetPassword
{
    public class ForgetPasswordBO
    {
        public int UserID { get; set; }
        public int LoginID { get; set; }
        public string GlobalID { get; set; }
        public string Email { get; set; }

    }
}
