using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DemoEFDAL.Designations
{
    public class DesignationUserDAL
    {

        public List<sp_GetDesignation_Result> designationDAL()
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.sp_GetDesignation().ToList();
                    result = result.Where(x => x.IsActive == true).ToList();

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

