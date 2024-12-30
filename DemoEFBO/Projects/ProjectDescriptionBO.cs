using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Projects
{
   public  class ProjectDescriptionBO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ProjectDescription { get; set; }

        public bool isActive { get; set; }

    }
}
