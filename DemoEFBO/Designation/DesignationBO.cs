using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Designation
{
    public class DesignationBO
    {
        public int Id { get; set; }
        public string DesignationName { get; set; }
        public bool Active { get; set; }

        public string Case { get; set; }
    }
    public class GetDesignationBO
    {
        public int Id { get; set; }
        public string DesignationName { get; set; }
        public bool Active { get; set; }


    }

    public class DesignationParms
    {
        public string DesignationName { get; set; }
        public bool IsFilter { get; set; }
    }



}
