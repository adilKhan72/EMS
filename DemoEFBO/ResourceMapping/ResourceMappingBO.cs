using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.ResourceMapping
{
    public class ResourceMappingBO
    {
        public int ResourceMappingId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectNames { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public decimal percentage { get; set; }
    }
    public class ResourceMaintaskMappingBO
    {
        public int ID { get; set; }
        public int MainTaskID { get; set; }
        public string MainTaskName { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string ProjectType { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }

    }
    public class SaveUpdateMappingBO
    {
        public int ProjectId { get; set; }
        public string ProjectNames { get; set; }
        public string MapString { get; set; }
      
    }

    public class FetchProjectUserBO
    {
        public int ProjectId { get; set; }

    }
    public class FetchProjectMappedMaintaskBO
    {
        public int MaintaskId { get; set; }

    }

    public class GetResourceMappingBO
    {
        public int UserId { get; set; }

    }
    public class SaveProjectMapping
    {
        public int ProjectId { get; set; }
        public int[] UserID { get; set; }
    }
    public class SaveMainTaskMapping
    {
        public int MainTaskID { get; set; }
        public int[] ProjectId { get; set; }
    }
    public class SaveUserMapping
    {
        public int[] ProjectId { get; set; }
        public int UserID { get; set; }
    }

    public class SaveDepartmentMapping
    {
        public List<DepartmentMappingList> MainTaskList { get; set; }
        public int DepartmentID { get; set; }
    }
    public class DepartmentMappingList
    {
        public int MainTaskIDs { get; set; }
        public bool IsActive { get; set; }
    }
}