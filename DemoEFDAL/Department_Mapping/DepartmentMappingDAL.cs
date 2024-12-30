using DemoEFBO.ResourceMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Department_Mapping
{
    public class DepartmentMappingDAL
    {
        public List<Get_DefaultMaintask_Result> DefaultMapping(bool CheckBoxStatus)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.Get_DefaultMaintask(CheckBoxStatus).ToList();

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
        public List<sp_GetDepartmentLazyLoading_Result> GetDepartmentLazyLoading(int page, int recsPerPage)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetDepartmentLazyLoading(recsPerPage, page).ToList();
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
        public List<Get_MapMainTaskinDepartmentMapping_Result> getMapMainTask(int DeptID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.Get_MapMainTaskinDepartmentMapping(DeptID).ToList();

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

        public bool InsertDepartmentMap(SaveDepartmentMapping mapping)
        {
            bool IsSuccess = false;
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    dbcontext.sp_DeleteDepartmentMapping(mapping.DepartmentID);
                    for (int i = 0; i < mapping.MainTaskList.Count; i++)
                    {
                        dbcontext.sp_InsertDepartmentMapping(mapping.DepartmentID, mapping.MainTaskList[i].MainTaskIDs, mapping.MainTaskList[i].IsActive);
                    }
                }
                return IsSuccess = true;
            }
            catch(Exception ex)
            {
                return IsSuccess = false;
            }
        }
    }
}
