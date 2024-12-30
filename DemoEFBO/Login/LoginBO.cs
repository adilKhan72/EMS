using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Login
{
    public class LoginBO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string UserImgURL { get; set; }
        public string Designation { get; set; }
        
    }
}
