using DemoEFBO.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.CommonProjectsList
{
    public interface IProjectsList
    {
        // For getting all project list
        List<sp_GetProjects_Result> GetProjects(String ProjectName);

        sp_getProjectByID_Result GetProjectById(int iProjectID);

        List<ProjectTable> GetAllProjects();

        bool InsertProject(ProjectsBO objProject);

        bool DeleteProject(int ID);

    }
}
