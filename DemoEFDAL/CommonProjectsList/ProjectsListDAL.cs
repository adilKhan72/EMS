using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.Projects;
using DemoEFBO.Reports;

namespace DemoEFDAL.CommonProjectsList
{
    public class ProjectsListDAL //: IProjectsList
    {
        public sp_getProjectByID_Result GetProjectById(int iProjectID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_getProjectByID(iProjectID).SingleOrDefault();
                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<sp_GetProjects_Update_Result> GetProjects(string ProjectName)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetProjects_Update(ProjectName).ToList();
                    if(result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<sp_GetProjectsLazyLoading_Result> GetProjectsLazyLoading(int page ,int recsPerPage)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetProjectsLazyLoading(recsPerPage, page).ToList();
                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ProjectTable> GetAllProjects()
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.ProjectTables.SqlQuery("Select * FROM ProjectTable").ToList<ProjectTable>();
                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool InsertProject(ProjectsBO objProject)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_InsertProjects_update (
                        objProject.ID,
                        objProject.Name,
                        objProject.Type,
                        Convert.ToDecimal(objProject.EstimatedHours),
                        objProject.StartDate,
                        objProject.EndDate,
                        objProject.ProjectOwner,
                        objProject.ProjectDescription,
                        objProject.IsActive,
                        objProject.isUpdateProjectBudget,
                        objProject.ProjectBudgetYear,
                        Convert.ToDecimal(objProject.ContractHours),
                        Convert.ToDecimal(objProject.JanContract),
                        Convert.ToDecimal(objProject.FebContract),
                        Convert.ToDecimal(objProject.MarContract),
                        Convert.ToDecimal(objProject.AprContract),
                        Convert.ToDecimal(objProject.MayContract),
                        Convert.ToDecimal(objProject.JunContract),
                        Convert.ToDecimal(objProject.JulContract),
                        Convert.ToDecimal(objProject.AugContract),
                        Convert.ToDecimal(objProject.SepContract),
                        Convert.ToDecimal(objProject.OctContract),
                        Convert.ToDecimal(objProject.NovContract),
                        Convert.ToDecimal(objProject.DecContract),objProject.ClientID );

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<sp_GetProjectBudgetDetail_Result> GetProjectsBudgetDetail(ProjectsBO objProject)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetProjectBudgetDetail(objProject.ID, objProject.ProjectBudgetYear).ToList();
                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool DeleteProject(int ID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var obj = dbContext.ProjectTables.SingleOrDefault(x => x.ID == ID);
                    if (obj != null)
                    {
                        dbContext.ProjectTables.Remove(obj);
                        dbContext.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<sp_SearchProject_Result> SearchProjctDAL(string ProjectName)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_SearchProject(ProjectName).ToList();
                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }



    }
}
