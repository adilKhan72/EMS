using DemoEFBO.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Tasks
{
   public class MainTasksDAL
    {
        public List<sp_GetMaintasks_Result> MainTaskDAL()
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.sp_GetMaintasks().OrderBy(g => g.MainTaskName).ToList();

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
        public List<sp_GetMainTasksLazyLoading_Result> GetMainTasksLazyLoading(int page, int recsPerPage)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetMainTasksLazyLoading(recsPerPage, page).ToList();
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
        public List<sp_SearchMainTask_Result> SearchMainTaskDAL(string MainTaskName)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_SearchMainTask(MainTaskName).ToList();
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
        public List<sp_GetClientLazyLoading_Result> GetClientLazyLoading(int page, int recsPerPage)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetClientLazyLoading(recsPerPage, page).ToList();
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
        public List<ClientTable> SearchClientDAL(string MainTaskName)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.ClientTables.AsNoTracking().ToList();
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
        public bool CheckMainTaskID(CheckMainTaskMappedParms model)
        {
            try
            {
                bool IsSuccess = false;
                using (var dbContext = new EMSEntities())
                {
                    var getUser = dbContext.UserProfileTables.Where(x => x.UserProfileTableID == model.UserID).FirstOrDefault();
                    if (getUser == null)
                    {
                        IsSuccess = false;
                    }
                    if (getUser != null)
                    {
                        var checkMaintask = dbContext.DepartmentMappings.Where(g => g.DepartID == getUser.Department_ID && g.MaintaskID == model.MainTaskID).FirstOrDefault();
                        if (checkMaintask != null)
                        {
                            IsSuccess=true;
                        }
                        if (checkMaintask == null)
                        {
                            IsSuccess = false;
                        }
                    }
                }
                return IsSuccess;  
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
