
using DemoEFBO.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Dashboard
{
    public class TimeSpentDAL
    {
        public List<sp_GetSpentTaskTime_update_Result> GetTimeSpentUpdateDAL(TimeSpenBO obj)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_GetSpentTaskTime_update(obj.ProjectId,obj.StartDate,obj.EndDate).ToList();
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

        public List<sp_GetSubTaskTime_Result> GetSubTaskTimeDAL(TimeSubtaskBO obj)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_GetSubTaskTime(obj.ProjectId,obj.MaintaskName, obj.StartDate,obj.EndDate).ToList();
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

        public List<sp_GetSubTaskTimeOwner_Result> GetSubTaskTimeOwnerDAL(TimeSubtaskBO obj)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_GetSubTaskTimeOwner(obj.ProjectId, obj.MaintaskName, obj.StartDate, obj.EndDate).ToList();
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
        //public List<sp_GetSpentTaskTime_Result> GetTimeSpentDAL(int Projectid)
        //{
        //    try
        //    {
        //        using (var dbcontext = new EMSEntities())
        //        {
        //            var result = dbcontext.sp_GetSpentTaskTime(Projectid).ToList();
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
    }
}
