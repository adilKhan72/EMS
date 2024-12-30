using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using DemoEFBO.Reports;
using DemoEFBO.User;

namespace DemoEFDAL.Reports
{
    public class ReportDAL : IReport
    {
        private readonly string connectionString = WebConfigurationManager.AppSettings["connectionString"];
        private int[] abcd;

        //public List<sp_GetAssignments_Result> GetAllAssignments(AssignmentDTOBO assignment)
        //{
        //    try
        //    {
        //        using (var dbcontext = new EMSEntities())
        //        {
        //            var result = dbcontext.sp_GetAssignments(assignment.ProjectId,
        //                assignment.TaskName, assignment.TaskOwnerId, assignment.FromDate, 
        //                assignment.ToDate, assignment.LoggedInUserId, assignment.MainTaskID).ToList();
        //            if(result != null)
        //            {
        //                return result;
        //            }
        //        }
        //            return null;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public List<sp_GetAssignments_New_Result> GetAssignmentsDAL(GetAssignmentBO obj)
        {
            try
            {
                string myarry = "";
                if(obj.LoggedInUserId == null){
                    myarry = "-1";
                }
                else
                {
                    myarry = string.Join(", ", obj.LoggedInUserId);
                }
              
                
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_GetAssignments_New(obj.FromDate, obj.ToDate, obj.IsApproved, myarry.ToString(), obj.MainTaskID,obj.ProjectId,obj.SubTaskID) .ToList();
                    if (result != null)
                    {
                        return result;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<sp_TotalHoursOfProjects_Result> TotalHoursOfProjects(TotalHoursOfProjectParamBO ParamObj)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_TotalHoursOfProjects(
                        ParamObj.ProjectId, 
                        ParamObj.TaskName, 
                        ParamObj.TaskOwnerId, 
                        ParamObj.FromDate, 
                        ParamObj.ToDate, 
                        ParamObj.TaskType, 
                        ParamObj.Approved, 
                        ParamObj.MainTaskID).ToList();
                    if(result != null)
                    {
                        return result;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public  sp_DifferenceHours_Result DifferenceHoursDAL(ExportAssignmentBO obj)
        {
            try
            {
                string myarry = "";
                if (obj.TaskOwnerId == null)
                {
                    myarry = "-1";
                }
                else
                {
                    myarry = string.Join(", ", obj.TaskOwnerId);
                }

                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_DifferenceHours(obj.FromDate, obj.ToDate,false, myarry.ToString(), obj.TaskId,obj.ProjectId,obj.SubTaskId).FirstOrDefault();
                    if (result != null)
                    {
                        return result;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public bool SaveAssignemntToDb(AssignmentsBO obj)
        //{
        //    try
        //    {
        //        using (var dbContext = new EMSEntities())
        //        {
        //            var result = dbContext.sp_InsertAssignment(
        //                obj.ID, 
        //                obj.ProjectName, 
        //                obj.TaskName, 
        //                obj.SubTaskName, 
        //                obj.TaskOwnerName, 
        //                Convert.ToDateTime(obj.AssignmentDateTime), 
        //                Convert.ToDecimal(obj.ActualDuration), 
        //                obj.Type, 
        //                Convert.ToInt32(obj.Phase), 
        //                obj.TaskType, 
        //                obj.MainTaskID,
        //                Convert.ToDecimal(obj.BillableHours),
        //                obj.IsBillableApproved,
        //                obj.UserID,
        //                obj.IsActualApproved,
        //                obj.UserIDActual

        //                );
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public bool CheckAssigneduser(int UserID, int ProjectID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_CheckAssignedProject(UserID, ProjectID).FirstOrDefault();
                    if (result != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SaveAssignemntToDb(AddRecordBO obj)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_AddRecord(
                        obj.ID,
                        obj.ProjectID,
                        obj.TASKID,
                        obj.UserID,
                        Convert.ToDateTime(obj.AssignmentDateTime),
                        obj.ActualDuration,
                        obj.CommentText,
                        obj.BillableHours,
                        obj.IsBillableApproved,
                        obj.UserID,
                        obj.IsActualApproved,
                        obj.UserIDActual,
                        obj.MainTaskID
                        );
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteAssignment(int ID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_DeleteAssignments(ID);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ApproveAssignment(int ProjectID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    dbContext.sp_ApproveAssignments(ProjectID);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public DataTable DifferenceHourse(ExportAssignmentBO assignmentWeekly, ref DataTable BillableDT)
        {
            try
            {
                DataTable DT = new DataTable();
                //DataTable ActualDT = new DataTable();
                //DataTable BillableDT = new DataTable();
                using (var context = new EMSEntities())
                {
                    if (assignmentWeekly.Billable == false)
                    {
                        var ConnString = context.Database.Connection.ConnectionString;

                        using (SqlConnection Connec = new SqlConnection(ConnString))
                        {
                            Connec.Open();

                            SqlCommand cmd = new SqlCommand("sp_GetFormattedTaskDataForReportGriddifference", Connec);
                            cmd.Parameters.Add("@projectid", SqlDbType.Int).Value = assignmentWeekly.ProjectId;
                            cmd.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = assignmentWeekly.FromDate;
                            cmd.Parameters.Add("@todate", SqlDbType.DateTime).Value = assignmentWeekly.ToDate;
                            cmd.Parameters.Add("@resourceid", SqlDbType.NVarChar).Value = assignmentWeekly.commasepartedTaskOwnerids.ToString();
                            cmd.Parameters.Add("@taskid", SqlDbType.Int).Value = assignmentWeekly.TaskId;
                            cmd.Parameters.Add("@Subtaskid", SqlDbType.Int).Value = assignmentWeekly.SubTaskId;
                            cmd.Parameters.Add("@billable", SqlDbType.Bit).Value = 0;
                            cmd.Parameters.Add("@DateRangeFormat", SqlDbType.Bit).Value = assignmentWeekly.SettingDateRange;
                            cmd.Parameters.Add("@Actual", SqlDbType.Bit).Value = 1;
                            cmd.Parameters.Add("@Departmentid", SqlDbType.Int).Value = assignmentWeekly.DepartmentID;
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                            SDA.Fill(DT);
                            Connec.Close();
                        }
                        using (SqlConnection Connec = new SqlConnection(ConnString))
                        {
                            Connec.Open();
                            SqlCommand cmd = new SqlCommand("sp_GetFormattedTaskDataForReportGriddifference", Connec);
                            cmd.Parameters.Add("@projectid", SqlDbType.Int).Value = assignmentWeekly.ProjectId;
                            cmd.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = assignmentWeekly.FromDate;
                            cmd.Parameters.Add("@todate", SqlDbType.DateTime).Value = assignmentWeekly.ToDate;
                            cmd.Parameters.Add("@resourceid", SqlDbType.NVarChar).Value = assignmentWeekly.commasepartedTaskOwnerids.ToString();
                            cmd.Parameters.Add("@taskid", SqlDbType.Int).Value = assignmentWeekly.TaskId;
                            cmd.Parameters.Add("@Subtaskid", SqlDbType.Int).Value = assignmentWeekly.SubTaskId;
                            cmd.Parameters.Add("@billable", SqlDbType.Bit).Value = 1;
                            cmd.Parameters.Add("@DateRangeFormat", SqlDbType.Bit).Value = assignmentWeekly.SettingDateRange;
                            cmd.Parameters.Add("@Actual", SqlDbType.Bit).Value = 1;
                            cmd.Parameters.Add("@Departmentid", SqlDbType.Int).Value = assignmentWeekly.DepartmentID;
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                            SDA.Fill(BillableDT);
                            Connec.Close();

                        }

                    }
                    if (assignmentWeekly.Billable == true)
                    {
                        var ConnString = context.Database.Connection.ConnectionString;

                        using (SqlConnection Connec = new SqlConnection(ConnString))
                        {
                            Connec.Open();
                            SqlCommand cmd = new SqlCommand("sp_GetFormattedTaskDataForReportGriddifference", Connec);
                            cmd.Parameters.Add("@projectid", SqlDbType.Int).Value = assignmentWeekly.ProjectId;
                            cmd.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = assignmentWeekly.FromDate;
                            cmd.Parameters.Add("@todate", SqlDbType.DateTime).Value = assignmentWeekly.ToDate;
                            cmd.Parameters.Add("@resourceid", SqlDbType.NVarChar).Value = assignmentWeekly.commasepartedTaskOwnerids.ToString();
                            cmd.Parameters.Add("@taskid", SqlDbType.Int).Value = assignmentWeekly.TaskId;
                            cmd.Parameters.Add("@Subtaskid", SqlDbType.Int).Value = assignmentWeekly.SubTaskId;
                            cmd.Parameters.Add("@billable", SqlDbType.Bit).Value = 1;
                            cmd.Parameters.Add("@DateRangeFormat", SqlDbType.Bit).Value = assignmentWeekly.SettingDateRange;
                            //cmd.Parameters.Add("@Actual", SqlDbType.Bit).Value = 1;
                            cmd.Parameters.Add("@Departmentid", SqlDbType.Int).Value = assignmentWeekly.DepartmentID;
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                            SDA.Fill(BillableDT);
                            Connec.Close();

                        }
                        return BillableDT;
                    }

                   
                }

                return DT;


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable GetTimeSheetDataTable(ExportAssignmentBO assignmentWeekly, ref DataTable BillableDT)
        {
            try
            {
                DataTable DT = new DataTable();
                //DataTable ActualDT = new DataTable();
                //DataTable BillableDT = new DataTable();
                using (var context = new EMSEntities())
                {
                    if (assignmentWeekly.Billable == false)
                    {
                        var ConnString = context.Database.Connection.ConnectionString;

                        using (SqlConnection Connec = new SqlConnection(ConnString))
                        {
                            Connec.Open();

                            SqlCommand cmd = new SqlCommand("sp_GetFormattedTaskDataForReport", Connec);
                            cmd.Parameters.Add("@projectid", SqlDbType.Int).Value = assignmentWeekly.ProjectId;
                            cmd.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = assignmentWeekly.FromDate;
                            cmd.Parameters.Add("@todate", SqlDbType.DateTime).Value = assignmentWeekly.ToDate;
                            cmd.Parameters.Add("@resourceid", SqlDbType.NVarChar).Value = assignmentWeekly.commasepartedTaskOwnerids.ToString();
                            cmd.Parameters.Add("@taskid", SqlDbType.Int).Value = assignmentWeekly.TaskId;
                            cmd.Parameters.Add("@Subtaskid", SqlDbType.Int).Value = assignmentWeekly.SubTaskId;
                            cmd.Parameters.Add("@billable", SqlDbType.Bit).Value = 0;
                            cmd.Parameters.Add("@DateRangeFormat", SqlDbType.Bit).Value = assignmentWeekly.SettingDateRange;
                            cmd.Parameters.Add("@Actual", SqlDbType.Bit).Value = 1;
                            cmd.Parameters.Add("@Departmentid", SqlDbType.Int).Value = assignmentWeekly.DepartmentID;
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                            SDA.Fill(DT);
                            Connec.Close();
                        }
                        using (SqlConnection Connec = new SqlConnection(ConnString))
                        {
                            Connec.Open();
                            SqlCommand cmd = new SqlCommand("sp_GetFormattedTaskDataForReport", Connec);
                            cmd.Parameters.Add("@projectid", SqlDbType.Int).Value = assignmentWeekly.ProjectId;
                            cmd.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = assignmentWeekly.FromDate;
                            cmd.Parameters.Add("@todate", SqlDbType.DateTime).Value = assignmentWeekly.ToDate;
                            cmd.Parameters.Add("@resourceid", SqlDbType.NVarChar).Value = assignmentWeekly.commasepartedTaskOwnerids.ToString();
                            cmd.Parameters.Add("@taskid", SqlDbType.Int).Value = assignmentWeekly.TaskId;
                            cmd.Parameters.Add("@Subtaskid", SqlDbType.Int).Value = assignmentWeekly.SubTaskId;
                            cmd.Parameters.Add("@billable", SqlDbType.Bit).Value = 1;
                            cmd.Parameters.Add("@DateRangeFormat", SqlDbType.Bit).Value = assignmentWeekly.SettingDateRange;
                            cmd.Parameters.Add("@Actual", SqlDbType.Bit).Value = 1;
                            cmd.Parameters.Add("@Departmentid", SqlDbType.Int).Value = assignmentWeekly.DepartmentID;
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                            SDA.Fill(BillableDT);
                            Connec.Close();

                        }
                        
                    }
                    if (assignmentWeekly.Billable == true)
                    {
                        var ConnString = context.Database.Connection.ConnectionString;

                        using (SqlConnection Connec = new SqlConnection(ConnString))
                        {
                            Connec.Open();
                            SqlCommand cmd = new SqlCommand("sp_GetFormattedTaskDataForReport", Connec);
                            cmd.Parameters.Add("@projectid", SqlDbType.Int).Value = assignmentWeekly.ProjectId;
                            cmd.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = assignmentWeekly.FromDate;
                            cmd.Parameters.Add("@todate", SqlDbType.DateTime).Value = assignmentWeekly.ToDate;
                            cmd.Parameters.Add("@resourceid", SqlDbType.NVarChar).Value = assignmentWeekly.commasepartedTaskOwnerids.ToString() ;
                            cmd.Parameters.Add("@taskid", SqlDbType.Int).Value = assignmentWeekly.TaskId;
                            cmd.Parameters.Add("@Subtaskid", SqlDbType.Int).Value = assignmentWeekly.SubTaskId;
                            cmd.Parameters.Add("@billable", SqlDbType.Bit).Value = 1;
                            cmd.Parameters.Add("@DateRangeFormat", SqlDbType.Bit).Value = assignmentWeekly.SettingDateRange;
                            //cmd.Parameters.Add("@Actual", SqlDbType.Bit).Value = 1;
                            cmd.Parameters.Add("@Departmentid", SqlDbType.Int).Value = assignmentWeekly.DepartmentID;
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                            SDA.Fill(BillableDT);
                            Connec.Close();

                        }
                        return BillableDT;
                    }

                    //using (SqlConnection Connec = new SqlConnection(ConnString))
                    //{
                    //    Connec.Open();
                    //    SqlCommand cmd = new SqlCommand("sp_GetFormattedTaskDataForReport", Connec);
                    //    cmd.Parameters.Add("@projectid", SqlDbType.Int).Value = assignmentWeekly.ProjectId;
                    //    cmd.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = assignmentWeekly.FromDate;
                    //    cmd.Parameters.Add("@todate", SqlDbType.DateTime).Value = assignmentWeekly.ToDate;
                    //    cmd.Parameters.Add("@resourceid", SqlDbType.Int).Value = assignmentWeekly.TaskOwnerId;
                    //    cmd.Parameters.Add("@taskid", SqlDbType.Int).Value = assignmentWeekly.TaskId;
                    //    cmd.Parameters.Add("@billable", SqlDbType.Bit).Value = assignmentWeekly.Billable;
                    //    cmd.Parameters.Add("@DateRangeFormat", SqlDbType.Bit).Value = assignmentWeekly.SettingDateRange;
                    //    cmd.CommandType = CommandType.StoredProcedure;
                    //    SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                    //    SDA.Fill(DT);
                    //    Connec.Close();
                    //}
                }

                return DT;
                

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<sp_TotalHoursOfProjects_New_Result> GetReportHoursDAL(AssignmentDTO obj)
        {
            try
            {

                string check = "";
                if (obj.TaskOwnerId == null)
                {
                    check = "-1";
                }
                else
                {
                    check=string.Join(", ", obj.TaskOwnerId);
                }
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_TotalHoursOfProjects_New(obj.ProjectId, obj.TaskName,check.ToString(), obj.FromDate, obj.ToDate, 0, "All", obj.MainTaskID, obj.SubTaskID,obj.DepartmentID).ToList();
                    if (result != null)
                    {
                        return result;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public decimal CheckuserHours(int UserID, int ProjectID, DateTime date)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_CheckUserHours(UserID, ProjectID, date).FirstOrDefault();
                    return (decimal)result;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public bool CheckMainTaskSubtask(AddRecordBO obj)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_CheckMainTaskSubTask(obj.MainTaskID, obj.TASKID).FirstOrDefault();
                    if (result != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public DataTable GetTimeSheetDataTableList(ExportAssignmentBO assignmentWeekly)
        {
            try
            {
                DataTable DT = new DataTable();
                using (var context = new EMSEntities())
                {
                    if (assignmentWeekly.Billable == false)
                    {
                        var ConnString = context.Database.Connection.ConnectionString;
                        using (SqlConnection Connec = new SqlConnection(ConnString))
                        {
                            Connec.Open();
                            SqlCommand cmd = new SqlCommand("sp_GetFormattedTaskDataForReportGrid", Connec);
                            cmd.Parameters.Add("@projectid", SqlDbType.Int).Value = assignmentWeekly.ProjectId;
                            cmd.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = assignmentWeekly.FromDate;
                            cmd.Parameters.Add("@todate", SqlDbType.DateTime).Value = assignmentWeekly.ToDate;
                            cmd.Parameters.Add("@resourceid", SqlDbType.Char).Value = assignmentWeekly.commasepartedTaskOwnerids;
                            cmd.Parameters.Add("@taskid", SqlDbType.Int).Value = assignmentWeekly.TaskId;
                            cmd.Parameters.Add("@Subtaskid", SqlDbType.Int).Value = assignmentWeekly.SubTaskId;
                            cmd.Parameters.Add("@billable", SqlDbType.Bit).Value = 0;
                            cmd.Parameters.Add("@DateRangeFormat", SqlDbType.Bit).Value = assignmentWeekly.SettingDateRange;
                            cmd.Parameters.Add("@Actual", SqlDbType.Bit).Value = 1;
                            cmd.Parameters.Add("@Departmentid", SqlDbType.Int).Value = assignmentWeekly.DepartmentID;
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                            SDA.Fill(DT);
                            Connec.Close();
                        }
                        //using (SqlConnection Connec = new SqlConnection(ConnString))
                        //{
                        //    Connec.Open();
                        //    SqlCommand cmd = new SqlCommand("sp_GetFormattedTaskDataForReport", Connec);
                        //    cmd.Parameters.Add("@projectid", SqlDbType.Int).Value = assignmentWeekly.ProjectId;
                        //    cmd.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = assignmentWeekly.FromDate;
                        //    cmd.Parameters.Add("@todate", SqlDbType.DateTime).Value = assignmentWeekly.ToDate;
                        //    cmd.Parameters.Add("@resourceid", SqlDbType.Int).Value = assignmentWeekly.TaskOwnerId;
                        //    cmd.Parameters.Add("@taskid", SqlDbType.Int).Value = assignmentWeekly.TaskId;
                        //    cmd.Parameters.Add("@billable", SqlDbType.Bit).Value = 1;
                        //    cmd.Parameters.Add("@DateRangeFormat", SqlDbType.Bit).Value = assignmentWeekly.SettingDateRange;
                        //    cmd.Parameters.Add("@Actual", SqlDbType.Bit).Value = 1;
                        //    cmd.CommandType = CommandType.StoredProcedure;
                        //    SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                        //    SDA.Fill(BillableDT);
                        //    Connec.Close();
                        //}
                    }
                    //if (assignmentWeekly.Billable == true)
                    //{
                    //    var ConnString = context.Database.Connection.ConnectionString;
                    //    using (SqlConnection Connec = new SqlConnection(ConnString))
                    //    {
                    //        Connec.Open();
                    //        SqlCommand cmd = new SqlCommand("sp_GetFormattedTaskDataForReport", Connec);
                    //        cmd.Parameters.Add("@projectid", SqlDbType.Int).Value = assignmentWeekly.ProjectId;
                    //        cmd.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = assignmentWeekly.FromDate;
                    //        cmd.Parameters.Add("@todate", SqlDbType.DateTime).Value = assignmentWeekly.ToDate;
                    //        cmd.Parameters.Add("@resourceid", SqlDbType.Int).Value = assignmentWeekly.TaskOwnerId;
                    //        cmd.Parameters.Add("@taskid", SqlDbType.Int).Value = assignmentWeekly.TaskId;
                    //        cmd.Parameters.Add("@billable", SqlDbType.Bit).Value = 1;
                    //        cmd.Parameters.Add("@DateRangeFormat", SqlDbType.Bit).Value = assignmentWeekly.SettingDateRange;
                    //        cmd.Parameters.Add("@Actual", SqlDbType.Bit).Value = 1;
                    //        cmd.CommandType = CommandType.StoredProcedure;
                    //        SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                    //        SDA.Fill(BillableDT);
                    //        Connec.Close();
                    //    }
                    //    return BillableDT;
                    //}
                }
                return DT;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable GetBreakDownSheetDataTable(ExportAssignmentBO assignmentWeekly)
        {
            try
            {
                DataTable DT = new DataTable();
                using (var context = new EMSEntities())
                {
                        var ConnString = context.Database.Connection.ConnectionString;

                        using (SqlConnection Connec = new SqlConnection(ConnString))
                        {
                            Connec.Open();

                            SqlCommand cmd = new SqlCommand("sp_GetFormattedTaskBreakDown", Connec);
                            cmd.Parameters.Add("@projectid", SqlDbType.Int).Value = assignmentWeekly.ProjectId;
                            cmd.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = assignmentWeekly.FromDate;
                            cmd.Parameters.Add("@todate", SqlDbType.DateTime).Value = assignmentWeekly.ToDate;
                            cmd.Parameters.Add("@MaintaskID", SqlDbType.Int).Value = assignmentWeekly.TaskId;
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                            SDA.Fill(DT);
                            Connec.Close();
                        }
                }
                return DT;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public decimal GetProjectManagementPercentage()
        {
            decimal percentage = 0;

            try
            {
                using (var context = new EMSEntities())
                {
                    var CheckPercentage = context.Settings.AsNoTracking().Where(x => x.vSettingName.ToUpper().Trim() == "Project Management Percentage".ToUpper().Trim()).FirstOrDefault();
                    if (CheckPercentage != null)
                    {
                        string getvalue = 0 + "." + CheckPercentage.vSettingValue;
                        return Convert.ToDecimal(getvalue);
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

            return percentage;
        }
        public DataTable WBSExcelDataTable(int ProjectID)
        {
            try
            {
                DataTable DT = new DataTable();
                using (var context = new EMSEntities())
                {
                    var ConnString = context.Database.Connection.ConnectionString;

                    using (SqlConnection Connec = new SqlConnection(ConnString))
                    {
                        Connec.Open();
                        SqlCommand cmd = new SqlCommand("Get_AllMainTaskWithSubTaskInExcel", Connec);
                        cmd.Parameters.Add("@ProjectID", SqlDbType.Int).Value = ProjectID;
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                        SDA.Fill(DT);
                        Connec.Close();
                    }

                }

                return DT;


            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
