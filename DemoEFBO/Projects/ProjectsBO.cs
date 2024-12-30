using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Projects
{
    public class ProjectsBO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsExternal { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; }
        public float EstimatedHours { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProjectOwner { get; set; }
        public string ProjectDescription { get; set; }

        //For Contract Per Month 
        public bool isUpdateProjectBudget { get; set; }
        public DateTime? ProjectBudgetYear { get; set; }
        public float ContractHours { get; set; }
        public float JanContract { get; set; }
        public float FebContract { get; set; }
        public float MarContract { get; set; }
        public float AprContract { get; set; }
        public float MayContract { get; set; }
        public float JunContract { get; set; }
        public float JulContract { get; set; }
        public float AugContract { get; set; }
        public float SepContract { get; set; }
        public float OctContract { get; set; }
        public float NovContract { get; set; }
        public float DecContract { get; set; }
        public int ClientID { get; set; }
    }

    public class ProjectImageBO
    {
        public int ProjectID { get; set; }
        public string Imagefileobj { get; set; }
    }

    public class SearchProjectBO
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public bool isActive { get; set; }

    }

    public class LazyLoadingProjectBO
    {
        public int page { get; set; }
        public int recsPerPage { get; set; }

    }
}
