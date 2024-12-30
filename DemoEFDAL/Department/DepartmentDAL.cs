using DemoEFBO.Department;
using DemoEFBO.Designation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Department
{
    public class DepartmentDAL
    {
        public List<sp_GetDepartment_Result> departmentDAL()
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.sp_GetDepartment().ToList();

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
        public bool InsertUpdateDepartment(DepartmentModel obj)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_AddUpdateDepartment(obj.ID, obj.DepartmentName, obj.IsActive);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteDepartment(int ID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_DeleteDepartment(ID);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
