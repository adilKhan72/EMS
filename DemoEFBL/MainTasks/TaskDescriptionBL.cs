using DemoEFBO.Tasks;
using DemoEFDAL.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.MainTasks
{
    public class TaskDescriptionBL
    {
        public List<TaskDescriptionResponseBO> GetTaskDescriptions(int projectID, int mainTaskID)
        {

            try
            {
                if (projectID > 0 && mainTaskID > 0)
                {
                    List<TaskDescriptionResponseBO> TaskDescriptionResponseBO = new List<TaskDescriptionResponseBO>();
                    TaskDescriptionDAL objDAL = new TaskDescriptionDAL();
                    var objResult = objDAL.TaskDescription(projectID, mainTaskID);
                    if (objResult != null)
                    {
                        for (int i = 0; i < objResult.Count; i++)
                        {
                            TaskDescriptionResponseBO.Add(new TaskDescriptionResponseBO
                            {
                                taskTableID = Convert.ToInt32(objResult[i].ID),
                                taskName = objResult[i].TaskName,
                                isActive = Convert.ToInt32(objResult[i].IsActive),
                                estimatedDurations = Convert.ToInt32(objResult[i].EstimatedDuration),
                                taskTypeId = Convert.ToInt32(objResult[i].TaskTypeId),
                                phase = Convert.ToInt32(objResult[i].Phase),
                                projectID = Convert.ToInt32(objResult[i].ProjectId),
                                maintaskID = Convert.ToInt32(objResult[i].MainTaskId)

                            });
                        }
                        return TaskDescriptionResponseBO;
                    }
                }
                return null;
            }
            catch { return null; }
        }
    }
}
