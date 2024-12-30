using DemoEFBO.Dashboard;
using DemoEFDAL.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.Dashboard
{
   


    public class TimeSpentBL
    {
        public TestResponse GetTimeSpentUpdate(TimeSpenBO objBO)
        {
            try
            {
                TimeSpentDAL ObjDAL = new TimeSpentDAL();
                var result = ObjDAL.GetTimeSpentUpdateDAL(objBO);
                if (result != null)
                {
                    TestResponse obj = new TestResponse();
                    List<string> lstMainTaskName = new List<string>();
                    List<decimal> lstHours = new List<decimal>();
                    List<decimal> lstDays = new List<decimal>();

                    for (int i = 0; i < result.Count; i++)
                    {

                        lstHours.Add(Convert.ToDecimal(result[i].Actualduration));
                        lstDays.Add(Convert.ToDecimal(result[i].Actualduration / 8));
                        lstMainTaskName.Add(result[i].MainTaskName);
                    }
                    obj.DayArr = lstDays;
                    obj.HoursArr = lstHours;
                    obj.MainTaskNames = lstMainTaskName;
                    return obj;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public SubTaskTimeResponse GetSubTaskBL(TimeSubtaskBO objBO)
        {
            try
            {
                TimeSpentDAL ObjDAL = new TimeSpentDAL();
                var result = ObjDAL.GetSubTaskTimeDAL(objBO);
                if (result != null)
                {
                    SubTaskTimeResponse obj = new SubTaskTimeResponse();
                    List<string> lstSubTaskName = new List<string>();
                    List<decimal> lstHours = new List<decimal>();
                    List<decimal> lstDays = new List<decimal>();

                    for (int i = 0; i < result.Count; i++)
                    {

                        lstHours.Add(Convert.ToDecimal(result[i].Actualduration));
                        lstDays.Add(Convert.ToDecimal(result[i].Actualduration / 8));
                        lstSubTaskName.Add(result[i].TaskName);
                    }
                    obj.DayArr = lstDays;
                    obj.HoursArr = lstHours;
                    obj.SubTaskNames = lstSubTaskName;
                    return obj;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<SubTaskTimeOwnerResponse> GetSubTaskTimeOwnerBL(TimeSubtaskBO objBO)
        {
            try
            {
                TimeSpentDAL ObjDAL = new TimeSpentDAL();
                var result = ObjDAL.GetSubTaskTimeOwnerDAL(objBO);
                if (result != null)
                {
                    List<SubTaskTimeOwnerResponse> obj = new List<SubTaskTimeOwnerResponse>();
                  
                    for (int i = 0; i < result.Count; i++)
                    {
                        obj.Add(new SubTaskTimeOwnerResponse
                        {
                            TaskName = result[i].TaskName,
                            TaskOwnerName = result[i].FullName
                        });
                    }
                    return obj;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //public TestResponse GetTimeSpent(int ProjectId)
        //{
        //    try
        //    {
        //        TimeSpentDAL ObjDAL = new TimeSpentDAL();
        //        var result = ObjDAL.GetTimeSpentDAL(ProjectId);
        //        if (result != null)
        //        {
        //            TestResponse obj = new TestResponse();
        //            List<string> lstMainTaskName = new List<string>();
        //            List<decimal> lstHours = new List<decimal>();
        //            List<decimal> lstDays = new List<decimal>();

        //            for (int i = 0; i < result.Count; i++)
        //            {

        //                lstHours.Add(Convert.ToDecimal(result[i].Actualduration));
        //                lstDays.Add(Convert.ToDecimal(result[i].Actualduration / 8));
        //                lstMainTaskName.Add(result[i].MainTaskName);
        //            }
        //            obj.DayArr = lstDays;
        //            obj.HoursArr = lstHours;
        //            obj.MainTaskNames = lstMainTaskName;
        //            return obj;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

    }
}
