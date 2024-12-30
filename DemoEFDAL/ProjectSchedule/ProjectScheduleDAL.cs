using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.ProjectSchedule
{
    public class ProjectScheduleDAL : IProjectSchedule
    {
        public bool AddReportProject(int SchReportID, int projectid, string ProjectName, string ProjectEstimate)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_InsertUpdateScheduleReportProject(SchReportID, projectid, ProjectName, ProjectEstimate);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddResource(int schRptResourceDetailID, int ResourceID, string Phase, DateTime? startDate, DateTime? endDate, string Duration, int projectID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_InsertUpdateScheduleReport_ResourceDetails(schRptResourceDetailID, ResourceID, Phase, startDate, endDate, Duration, projectID);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddProjectUpdate(int SchRptUpdateID, string updateNo, int resourceID, int ProjectID, string vComments, string vProjectPhase)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_InsertUpdateScheduleReport_Updates(SchRptUpdateID, updateNo, resourceID, ProjectID, vComments, vProjectPhase);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<sp_RetrieveScheduleReportProject_Result> GetScheduleProjects(int SchReport_ProjectID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_RetrieveScheduleReportProject(SchReport_ProjectID).ToList();
                    if(result != null && result.Count > 0)
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

        public List<sp_RetrieveScheduleReport_ResourceDetails_Result> GetScheduleResourceData(int ProjectID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_RetrieveScheduleReport_ResourceDetails(ProjectID).ToList();
                    if(result != null && result.Count > 0)
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

        public List<sp_RetrieveScheduleReport_Updates_Result> GetScheduleUpdates(int ProjectID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_RetrieveScheduleReport_Updates(ProjectID).ToList();
                    if (result != null && result.Count > 0)
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

        public List<sp_RetrieveScheduleReportProject_Result> GetScheduleProjectListExport(int SchRpt_ProjectID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_RetrieveScheduleReportProject(SchRpt_ProjectID).ToList();
                    if(result != null && result.Count > 0)
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

        public List<sp_RetrieveScheduleReport_ResourceDetails_Result> GetScheduleResourceListExport(int ProjectID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_RetrieveScheduleReport_ResourceDetails(ProjectID).ToList();
                    if (result != null && result.Count > 0)
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

        public List<sp_RetrieveScheduleReport_Updates_Result> GetScheduleUpdateListExport(int ProjectID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_RetrieveScheduleReport_Updates(ProjectID).ToList();
                    if (result != null && result.Count > 0)
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

        public List<sp_getUpdateTableCount_Result> getScheduleProjectLength()
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_getUpdateTableCount().ToList();
                    if (result != null && result.Count > 0)
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
