using DemoEFBO.Department;
using DemoEFBO.Designation;
using DemoEFDAL.Department;
using DemoEFDAL.Designations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.Department
{
    public class DepartmentBL
    {
        public List<DepartmentModel> GetDepartment()
        {
            try
            {
                DepartmentDAL dAL = new DepartmentDAL();
                List<DepartmentModel> ListModel = new List<DepartmentModel>();
                var result = dAL.departmentDAL();
                if (result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        ListModel.Add(new DepartmentModel
                        {
                            ID = result[i].ID,
                            DepartmentName = result[i].DepartmentName,
                            IsActive = Convert.ToBoolean(result[i].IsActive),
                        });
                    }
                    return ListModel;
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool InsertDepartment(DepartmentModel obj)
        {
            try
            {
                DepartmentDAL dAL = new DepartmentDAL();
                return dAL.InsertUpdateDepartment(obj);   
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteDepartment(DepartmentModel obj)
        {
            try
            {
                DepartmentDAL dAL = new DepartmentDAL();
                return dAL.DeleteDepartment(obj.ID);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
