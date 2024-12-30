using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.Designation;
namespace DemoEFDAL.Designations
{
    public class Designationlist
    {
        public bool InsertDesignation(DesignationBO obj, bool isUpdate)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_AddDesignation(obj.Id, obj.DesignationName, obj.Active, isUpdate);
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
