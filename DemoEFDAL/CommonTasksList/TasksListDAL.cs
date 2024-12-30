using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.Tasks;

namespace DemoEFDAL.CommonTasksList
{
    public class TasksListDAL //: ITasksList
    {
        public List<sp_getTaskOwners1_Result> GetTaskOwners()
        {
            try
            {
                return null;
                //using (var dbcontext = new EMSEntities())
                //{
                //    var result = dbcontext.sp_GetTaskOwners().ToList();
                //    if(result != null)
                //    {
                //        return result;
                //    }
                //    return null;
                //}
            }
            catch (Exception)
            {
                return null;
            }
        }

        /*public List<sp_GetTasksForProjects_Result> GetTasks(int projectID, int MainTaskID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetTasksForProjects(projectID, MainTaskID).ToList();
                    if(result != null)
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
        }*/

        public List<TaskTable> GetAllTasks()
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.TaskTables.SqlQuery("Select * FROM TaskTable").ToList<TaskTable>();
                    if(result != null)
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

        public List<sp_getTaskType_Result> GetTaskType()
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_getTaskType().ToList();
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

        public List<sp_GetTasks_Result> GetTasks(string TaskName,int ProjectID,int MainTaskID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetTasks(TaskName, ProjectID, MainTaskID).ToList();
                    if(result != null && result.Count >= 0)
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

        public bool AddTask(TasksBO TaskObj)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_AddTasks(
                                    TaskObj.ID,
                                    TaskObj.Phase,
                                    TaskObj.TaskName,
                                    TaskObj.ProjectName,
                                    TaskObj.TaskTypeName,
                                    TaskObj.EstimatedDuration,
                                    TaskObj.MainTaskID, TaskObj.Comments);
                    return true;
                }
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
                using (var dbContext = new EMSEntities())
                {
                    var obj = dbContext.TaskTables.SingleOrDefault(x => x.ID == ID);
                    if(obj != null)
                    {
                        dbContext.TaskTables.Remove(obj);
                        dbContext.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<sp_GetTaskOwners_Result> GetTaskOwners(string TaskOwnerName)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetTaskOwners(TaskOwnerName,false).ToList();
                    if(result != null)
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

        public bool AddTaskOwner(ddTaskOwners obj)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_AddTaskOwners(obj.ID, obj.TaskOwnerName,obj.IsActive,false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteTaskOwner(int id)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var obj = dbContext.TaskOwnerTables.SingleOrDefault(x => x.ID == id);
                    if (obj != null)
                    {
                        dbContext.TaskOwnerTables.Remove(obj);
                        dbContext.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<sp_GetMainTaskList_Update_Result> GetFilterMainTaskList(string MainTaskName,bool filter)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetMainTaskList_Update(MainTaskName, filter).ToList();
                    if(result != null && result.Count > 0)
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
        public List<sp_GetMappedMainTaskByProjectID_DepartmentID_Result> GetMainTaskMappedListDAL(int ProjectID,int UserID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    int DeptID = -1;
                    string AccountType = string.Empty;
                    if (UserID > 0)
                    {
                        var getDeptID = dbContext.UserProfileTables.Where(x => x.UserProfileTableID == UserID).FirstOrDefault();
                        if (getDeptID != null)
                        {
                            AccountType = getDeptID.AccountType;
                            DeptID = getDeptID.AccountType == "SuperAdmin" ? 0 : (int)getDeptID.Department_ID;
                        }
                    }
                    var result = dbContext.sp_GetMappedMainTaskByProjectID_DepartmentID(ProjectID,DeptID,AccountType).OrderBy(d=>d.MainTaskName).ToList();
                    if (result != null && result.Count > 0)
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
        public List<sp_GetMappedMainTask_Result> GetMainTaskMappedListDALByDepartmentID(int ProjectID, int DeparmentID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetMappedMainTask(ProjectID).OrderBy(d => d.MainTaskName).ToList();
                    if (result != null && result.Count > 0)
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
        public List<sp_GetMappedMainTaskinReportGrid_Result> GetMainTasByDepartmentID(int DeparmentID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetMappedMainTaskinReportGrid(DeparmentID).OrderBy(d => d.MainTaskName).ToList();
                    if (result != null && result.Count > 0)
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

        public List<sp_GetMappedMainTaskByProjectID_DepartmentID_Result> sp_GetMappedMainTaskByProjectID_DepartmentID(int ProjectID, int DeparmentID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetMappedMainTaskByProjectID_DepartmentID(ProjectID, DeparmentID,"Admin").OrderBy(d => d.MainTaskName).ToList();
                    if (result != null && result.Count > 0)
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
        public List<sp_GetMainTaskList1_Result> GetFilterMainTaskListDAL(string MainTaskName, bool isFilter)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_GetMainTaskList1(MainTaskName, isFilter).ToList();// .sp_GetMainTaskList(MainTaskName, isFilter).ToList();
                    if (result != null && result.Count > 0)
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

        public bool InsertMainTask(MainTaskBO obj,bool isUpdate)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_AddMainTask(obj.Id, obj.MainTaskName, obj.Active, isUpdate);
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
