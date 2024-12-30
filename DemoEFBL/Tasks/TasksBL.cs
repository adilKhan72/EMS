using DemoEFBO.Tasks;
using DemoEFDAL.CommonTasksList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.Tasks
{
    public class TasksBL
    {
        public List<TasksBO> GetTasks(string TaskName,int ProjectID, int MainTaskID)
        {
            try
            {
                var tasks = new List<TasksBO>();
                TasksListDAL objDal = new TasksListDAL();
                var result = objDal.GetTasks(TaskName, ProjectID, MainTaskID);
                if(result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        var projectId = 0;
                        var projectName = "No Project Assigned";
                        var _MainTask = "No Main Task Assigned";

                        if (!string.IsNullOrEmpty(result[i].ProjectId.ToString()))
                        {
                            projectId = Convert.ToInt32(result[i].ProjectId);
                            projectName = result[i].ProjectName;
                        }
                        if (!string.IsNullOrEmpty(result[i].MainTask))
                        {
                            _MainTask = result[i].MainTask;
                        }
                        tasks.Add(new TasksBO
                        {
                            ID = result[i].ID,
                            TaskName = result[i].TaskName,
                            EstimatedDuration = Convert.ToDecimal(result[i].EstimatedDuration),
                            TaskTypeId = Convert.ToInt32(result[i].TaskTypeId),
                            TaskTypeName = result[i].TaskTypeName,
                            TaskTypePrefix = result[i].TaskTypePrefix,
                            Phase = Convert.ToInt32(result[i].Phase),
                            ProjectId = projectId,
                            ProjectName = projectName,
                            MainTask = _MainTask,
                            MainTaskID = Convert.ToInt32(result[i].MainTaskId),
                            Comments = result[i].Comments,
                        });
                    }
                    //tasks = tasks.OrderByDescending(x => x.ID).ToList();     //Sort in Descending Order by ID
                    return tasks;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool AddTask(TasksBO task)
        {
            try
            {
                if(task != null)
                {
                    TasksListDAL objDal = new TasksListDAL();
                    return objDal.AddTask(task);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteTask(int ID)
        {
            try
            {
                if(ID > 0)
                {
                    TasksListDAL objDal = new TasksListDAL();
                    return objDal.DeleteTask(ID);
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
