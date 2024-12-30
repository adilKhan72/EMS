using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.Tasks;
using DemoEFDAL.CommonTasksList;

namespace DemoEFBL.CommonLists
{
    public static class TaskListBL
    {
        /*public static List<TasksBO> GetTasks(int projectID, int MainTaskID)
        {
            try
            {
                //bool _IsAdminUser = false;
                //if (UserRole == "Admin")
                //{
                //    _IsAdminUser = true;
                //}
                List<TasksBO> lst = new List<TasksBO>();
                TasksListDAL objDAL = new TasksListDAL();
                var result = objDAL.GetTasks(projectID, MainTaskID);
                if (result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new TasksBO { ID = result[i].ID, TaskName = result[i].TaskName });
                    }
                    return lst;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }*/

        public static List<ddTaskType> GetTaskTypes()
        {
            try
            {
                List<ddTaskType> lst = new List<ddTaskType>();
                TasksListDAL objDAL = new TasksListDAL();
                var result = objDAL.GetTaskType();
                if (result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new ddTaskType { TaskTypeId = result[i].Id, TaskTypeName = result[i].Name, TaskTypePrefix = result[i].Prefix });
                    }
                    return lst;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<ddTaskOwners> GetTaskOwners()
        {
            try
            {
                List<ddTaskOwners> lst = new List<ddTaskOwners>();
                TasksListDAL objDAL = new TasksListDAL();
                var result = objDAL.GetTaskOwners();
                if (result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new ddTaskOwners { ID = result[i].ID, TaskOwnerName = result[i].TaskOwnerName, IsActive = result[i].IsActive });
                    }
                    return lst;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<TasksBO> GetAllTasks()
        {
            try
            {
                List<TasksBO> lst = new List<TasksBO>();
                TasksListDAL objDal = new TasksListDAL();
                var result = objDal.GetAllTasks();
                if(result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new TasksBO {
                            ID = result[i].ID,
                            TaskName = result[i].TaskName,
                            EstimatedDuration = Convert.ToDecimal(result[i].EstimatedDuration)
                        });
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
