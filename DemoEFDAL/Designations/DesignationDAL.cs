using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DemoEFDAL.Designations
{
    public class DesignationDAL
    {

        public List<sp_GetDesignation_Result> designationDAL()
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.sp_GetDesignation().ToList();
            
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

        public bool DeleteDesignation(int ID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_DeleteDesignation(ID);
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
