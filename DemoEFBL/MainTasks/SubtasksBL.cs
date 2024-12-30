using DemoEFBO.Tasks;
using DemoEFDAL.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.MainTasks
{
    public class SubtasksBL
    {
        public List<SubTasksResponseBO> getsubtaskname(int projectID, int mainTaskID)
        {

            try
            {
                
                    List<SubTasksResponseBO> SubTasksResponse = new List<SubTasksResponseBO>();
                    SubTaskDAL objDAL = new SubTaskDAL();
                    var objResult = objDAL.Subtask(projectID, mainTaskID);
                    if (objResult != null)
                    {
                        for (int i = 0; i < objResult.Count; i++)
                        {
                            SubTasksResponse.Add(new SubTasksResponseBO
                            {
                                SubtaskId = Convert.ToInt32(objResult[i].ID),
                                TaskName = objResult[i].TaskName,
                               
                            });
                        }
                        return SubTasksResponse;
                    }
                return null;
            }
            catch { return null; }
        }


    }
}

