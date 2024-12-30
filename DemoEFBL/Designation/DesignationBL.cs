using DemoEFBO.Designation;
using DemoEFDAL.Designations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.Designation
{
    public class DesignationBL
    {
        public List<GetDesignationBO> GetDesignation()
        {
            try
            {
                List<GetDesignationBO> GetDesignation = new List<GetDesignationBO>();
                DesignationDAL objDal = new DesignationDAL();

                var result = objDal.designationDAL();
                if (result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                       
                            GetDesignation.Add(new GetDesignationBO
                            {
                                Id = result[i].ID,
                                DesignationName = result[i].DesignationName,
                                Active = Convert.ToBoolean(result[i].IsActive),
                               

                            });
                        
                      
                        
                    }
                    return GetDesignation;
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        

     
        //public List<DesignationBO> GetDesignationList(string DesignationName)
        //{
        //    try
        //    {
        //        var Designation = new List<DesignationBO>();
        //        Designationlist objDal = new Designationlist();
        //        var result = objDal.GetFilterDesignationList(DesignationName, false);
        //        if (result != null)
        //        {
        //            for (int i = 0; i < result.Count; i++)
        //            {
        //                Designation.Add(new DesignationBO
        //                {
        //                    Id = result[i].ID,
        //                   DesignationName = result[i].DesignationName,
        //                    Active = Convert.ToBoolean(result[i].IsActive)
        //                });
        //            }
        //            return Designation;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
       

        public bool InsertDesignation(DesignationBO desig)
        {
            try
            {
                if (desig != null)
                {
                    bool Param = false;
                    if (desig.Case == "add")
                    {
                        Param = false;
                    }
                    else
                    {
                        Param = true;
                    }
                    Designationlist objDal = new Designationlist();
                    return objDal.InsertDesignation(desig, Param);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
