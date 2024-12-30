using DemoEFBO.Tasks;
using DemoEFDAL.CommonTasksList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.TaskOwners
{
    public class TaskOwnersBL
    {
        public List<ddTaskOwners> GetTaskOwners(string TaskOwnerName)
        {
            try
            {
                var taskOwners = new List<ddTaskOwners>();
                TasksListDAL objDal = new TasksListDAL();
                var result = objDal.GetTaskOwners(TaskOwnerName);
                if(result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        taskOwners.Add(new ddTaskOwners
                        {
                            ID = result[i].ID,
                            TaskOwnerName = result[i].TaskOwnerName,
                            IsActive = Convert.ToBoolean(result[i].IsActive)
                        });
                    }
                    return taskOwners;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool AddTaskOwner(ddTaskOwners taskOwner)
        {
            try
            {
                if(taskOwner != null)
                {
                    TasksListDAL objDal = new TasksListDAL();
                    return objDal.AddTaskOwner(taskOwner);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteTaskOwner(int ID)
        {
            try
            {
                if(ID > 0)
                {
                    TasksListDAL objDal = new TasksListDAL();
                    return objDal.DeleteTaskOwner(ID);
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
