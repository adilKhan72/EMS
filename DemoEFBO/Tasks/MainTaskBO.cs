using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Tasks
{
    public class MainTaskBO
    {
        public int Id { get; set; }
        public string MainTaskName { get; set; }
        public string ProjectIDs { get; set; }
        public bool Active { get; set; }
        public string Case { get; set; }
    }
    public class GetMainTaskBO
    {
        public int Id { get; set; }
        public string MainTaskName { get; set; }
        public bool Active { get; set; }
        public DateTime CreationDate { get; set; }
        public string ProjectIDs { get; set; }
        public string ProjectType { get; set; }

    }

    public class MainTaskParms
    {
        public string MainTaskName { get; set; }
        public bool IsFilter { get; set; }
    }

    public class MainTaskMappedParms
    {
        public int ProjectID { get; set; }
        public int UserID { get; set; }

    }
    public class SearchMainTaskBO
    { 
       public string MainTaskName { get; set; }
    }
    public class LazyLoadingMainTaskBO
    {
        public int page { get; set; }
        public int recsPerPage { get; set; }

    }
    public class LazyLoadingClientBO
    {
        public int page { get; set; }
        public int recsPerPage { get; set; }

    }
    public class LazyLoadingDepartmentBO
    {
        public int page { get; set; }
        public int recsPerPage { get; set; }

    }

    public class GetClientBO
    {
        public int ID { get; set; }
        public string ClientName { get; set; }
        public bool Active { get; set; }
        public string ProjectIDs { get; set; }
        public string ProjectType { get; set; }

    }
    public class CheckMainTaskMappedParms
    {
        public int ProjectID { get; set; }
        public int UserID { get; set; }
        public int MainTaskID { get; set; }

    }
}
