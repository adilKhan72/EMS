using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.ProjectSchedule
{
    public interface IProjectSchedule
    {
        bool AddReportProject(int SchReportID,int projectid, string ProjectName, string ProjectEstimate);

        bool AddResource(int schRptResourceDetailID, int ResourceID, string Phase, Nullable<DateTime> startDate, Nullable<DateTime> endDate, string Duration, int projectID);

        bool AddProjectUpdate(int SchRptUpdateID, string updateNo, int resourceID, int ProjectID, string vComments, string vProjectPhase);

        List<sp_RetrieveScheduleReportProject_Result> GetScheduleProjects(int SchReport_ProjectID);

        List<sp_RetrieveScheduleReport_ResourceDetails_Result> GetScheduleResourceData(int ProjectID);

        List<sp_RetrieveScheduleReport_Updates_Result> GetScheduleUpdates(int ProjectID);

        List<sp_RetrieveScheduleReportProject_Result> GetScheduleProjectListExport(int SchRpt_ProjectID);

        List<sp_RetrieveScheduleReport_ResourceDetails_Result> GetScheduleResourceListExport(int ProjectID);

        List<sp_RetrieveScheduleReport_Updates_Result> GetScheduleUpdateListExport(int ProjectID);

        List<sp_getUpdateTableCount_Result> getScheduleProjectLength();

    }
}
