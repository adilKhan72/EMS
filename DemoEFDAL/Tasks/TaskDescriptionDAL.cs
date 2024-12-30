using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Tasks
{
    public class TaskDescriptionDAL
    {
        public List<sp_GetTasksDescription_Result> TaskDescription(int projectID, int mainTaskID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.sp_GetTasksDescription(projectID, mainTaskID).OrderBy(o=>o.TaskName).ToList();

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
