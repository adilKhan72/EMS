using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.Reports;


namespace DemoEFDAL.Reports
{
    public interface IReport
    {
        // List<sp_GetAssignments_Result> GetAllAssignments(AssignmentDTOBO assignment);
        List<sp_GetAssignments_New_Result> GetAssignmentsDAL(GetAssignmentBO obj);

        List<sp_TotalHoursOfProjects_Result> TotalHoursOfProjects(TotalHoursOfProjectParamBO ParamObj);

       /* bool SaveAssignemntToDb(AssignmentsBO obj);*/ 
        bool SaveAssignemntToDb(AddRecordBO obj);

        bool DeleteAssignment(int id);

        bool ApproveAssignment(int ProjectID);

        //List<sp_ActualVsEstimatedHoursComparisonReport> GetActualVsEstimatedComparedTasks(AssignmentDTOBO assignment);

    }
}
