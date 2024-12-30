using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.Projects;
using DemoEFDAL.CommonProjectsList;

namespace DemoEFBL.CommonLists
{
    public static class ProjectsListBL
    {
        public static List<ProjectsBO> GetAllProjects()
        {
            try
            {
                List<ProjectsBO> lst = new List<ProjectsBO>();
                ProjectsListDAL objDAL = new ProjectsListDAL();
                var result = objDAL.GetAllProjects();
                if(result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new ProjectsBO {
                            ID = result[i].ID,
                            Name = result[i].Name,
                            Type = result[i].Type
                        });
                    }
                    return lst;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<ddProjectsBO> GetProjects(string ProjectName)
        {
            try
            {
                List<ddProjectsBO> lst = new List<ddProjectsBO>();
                ProjectsListDAL objDAL = new ProjectsListDAL();
                var result = objDAL.GetProjects(ProjectName);
                if(result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new ddProjectsBO { ID = result[i].ID, Name = result[i].Name });
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public static ddProjectsBO GetProjectById(int projectId)
        {
            try
            {
                ProjectsListDAL objDAL = new ProjectsListDAL();
                ddProjectsBO objData = new ddProjectsBO();
                var result = objDAL.GetProjectById(projectId);
                if(result != null)
                {
                    objData.ID = result.ID;
                    objData.Name = result.Name;
                    return objData;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
