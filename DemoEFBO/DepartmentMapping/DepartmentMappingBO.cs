using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.DepartmentMapping
{
    public class DepartmentMappingBO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
    public class MapMainTaskinDepartmentMapping
    {
        public int DepartID { get; set; }
        public int DepartMappingID { get; set; }
        public int MaintaskID { get; set; }
        public string Name { get; set; }
        public bool AdditionalCheck { get; set; }
    }
}
