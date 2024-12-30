using DemoEFBO.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Dashboard
{
    public class DashboardStatsDAL
    {
        //public List<sp_GetDashboardStats1_Result> GetDashboardStatsDAL(DateTime dtRequired, bool _IsManagementTimeExclude)
        //{
        //    try
        //    {
        //        using (var dbcontext = new EMSEntities())
        //        {
        //            var result = dbcontext.sp_GetDashboardStats1(dtRequired, _IsManagementTimeExclude).ToList();
        //            if (result != null)
        //            {
        //                return result;
        //            }
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public List<sp_GetDashboardStats_Update_Result> GetDashboardStatsUpdateDAL(DateTime? RequestStartDate, DateTime? RequestEndDate, int ProjectID,bool IsManagementTime)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_GetDashboardStats_Update(RequestStartDate, RequestEndDate, ProjectID, IsManagementTime).ToList();
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


        public List<sp_GetDashboardStats_Staff_Update_Result> GetDashboardStatsForStaffDAL(DateTime RequestDate, string iStaffID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_GetDashboardStats_Staff_Update(RequestDate, iStaffID).ToList();
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

        public List<sp_GetDashboardStats_Staff_Search_Result> GetDashboardStatsForStaffSearchDAL(DateTime StartDate, DateTime EndDate, string iStaffID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_GetDashboardStats_Staff_Search(StartDate, EndDate, iStaffID).ToList();
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
        public List<sp_GetDashboardStats_Update_AllProject_Result> GetAllProjects(int projectID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_GetDashboardStats_Update_AllProject(projectID).OrderBy(o => o.Name).ToList();
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

        public List<sp_GetProjectDatabyID_Result> GetProjectByID(DateTime? RequestStartDate, DateTime? RequestEndDate, int ProjectID, bool IsManagementTime)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_GetProjectDatabyID(RequestStartDate, RequestEndDate, ProjectID, IsManagementTime).ToList();
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
