using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.User
{
    public class UserProfileIDBO
    {
        public int UserID { get; set; }
        public int DepartID { get; set; }
    }

    public class UserProfileFilterBO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public bool? Status { get; set; }
        public string EMSRole { get; set; }
        public string Designation { get; set; }

    }

    public class UserProfileTableBO
    {
        public int UserProfileID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public DateTime? JoiningDate { get; set; }
        public bool? IsActive { get; set; }
        public string AccountType { get; set; }
        public string Picture { get; set; }
        public string Creationdatetime { get; set; }
        public string Designation { get; set; }
        

    }
    public class UserProfileBO
    {
        public int UserProfileTableID { get; set; }
        public int UserID { get; set; }
        public int LoginID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string JoiningDate { get; set; }
        public bool Status { get; set; }
        public string EMSRole { get; set; }
        public string Picture { get; set; }
        public string Creationdatetime { get; set; }
        public string Designation { get; set; }
        public string Password { get; set; }
        public int Department_ID { get; set; }
    }

    public class UserProfileImageBO
    {
        public int UserID { get; set; }
        public string Imagefileobj { get; set; }
    }
}
