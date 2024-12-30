using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Tasks
{
    public class ddTaskOwners
    {
        public int ID { get; set; }
        public string TaskOwnerName { get; set; }
        public bool? IsActive { get; set; }
    }

    public class TaskOwnersName
    {
        public string TaskOwnerName { get; set; }
    }

}
