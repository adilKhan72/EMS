using DemoEFBO.Projects;
using DemoEFDAL;
using DemoEFDAL.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.Projects
{
    public class ProjectDescriptionBL
    {
        public List<ProjectDescriptionBO> GetProjectDescriptionBL()
        {
            try
            {
                projectDescriptionDAL objDAL = new projectDescriptionDAL();
                List<ProjectDescriptionBO> objBO = new List<ProjectDescriptionBO>();
                var objResult = objDAL.ProjectDescription();
                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        objBO.Add(new ProjectDescriptionBO
                        {
                            ID = Convert.ToInt32(objResult[i].ID),
                            Name = objResult[i].Name,
                            isActive=Convert.ToBoolean(objResult[i].IsActive),

                            ProjectDescription = objResult[i].ProjectDescription,
                            

                        });
                    }
                    return objBO;
                }
                return null;
            }

            catch (Exception)
            {
                return null;

            }

        }
    }

}